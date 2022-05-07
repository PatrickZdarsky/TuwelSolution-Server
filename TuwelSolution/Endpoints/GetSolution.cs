using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TuwelSolution.Models;
using TuwelSolution.Services;

namespace TuwelSolution.Endpoints;

public class GetSolution : EndpointBaseAsync
    .WithRequest<string>
    .WithActionResult<QuizSolution>
{
    private readonly ISolutionService _solutionService;

    public GetSolution(ISolutionService solutionService)
    {
        _solutionService = solutionService;
    }

    [HttpGet("/solution/{identifier}")]
    [SwaggerOperation(Summary = "Get the solution for a quiz",
         OperationId = "Solution.Get",
         Tags = new[] { "Solution" })]
    public override async Task<ActionResult<QuizSolution>> HandleAsync(string identifier, CancellationToken cancellationToken = new CancellationToken())
    {
        return await _solutionService.GetSolution(identifier, cancellationToken);
    }
}