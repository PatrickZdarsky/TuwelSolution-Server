using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Swashbuckle.AspNetCore.Annotations;
using TuwelSolution.Models;
using TuwelSolution.Services;

namespace TuwelSolution.Endpoints;

public class SaveSolution : EndpointBaseAsync
    .WithRequest<QuizSolution>
    .WithResult<SolutionUpdateResult>
{
    private readonly ILogger<SaveSolution> _logger;
    private readonly ISolutionService _solutionService;

    public SaveSolution(ISolutionService solutionService, ILogger<SaveSolution> logger)
    {
        _logger = logger;
        _solutionService = solutionService;
    }


    [HttpPut("/solution/save")]
    [SwaggerOperation(Summary = "Save the solution for a quiz",
        OperationId = "Solution.Save",
        Tags = new[] { "Solution" })]
    public override async Task<SolutionUpdateResult> HandleAsync(QuizSolution request, CancellationToken cancellationToken = default)
    {
        var policy = Policy<SolutionUpdateResult>
            .Handle<HttpRequestException>()
            .OrResult(updateResult  => updateResult.Status == SolutionUpdateResult.UpdateStatus.Error)
            .RetryAsync(5, onRetry: (result, retryCount) =>
            {
                _logger.LogWarning($"Solution save retry #{retryCount} for {result.Result.Solution.Quiz}");
            });
        
        var result = await policy.ExecuteAsync(async () => 
            await _solutionService.SaveSolution(request, cancellationToken));

        return result;
    }
}