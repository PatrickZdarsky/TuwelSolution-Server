using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TuwelSolution.Models;
using TuwelSolution.Services;

namespace TuwelSolution.Endpoints;

public class SaveSolution : EndpointBaseAsync
    .WithRequest<QuizSolution>
    .WithActionResult
{
    private readonly ISolutionService _solutionService;

    public SaveSolution(ISolutionService solutionService)
    {
        _solutionService = solutionService;
    }


    [HttpPut("/solution/save")]
    [SwaggerOperation(Summary = "Save the solution for a quiz",
        OperationId = "Solution.Save",
        Tags = new[] { "Solution" })]
    public override async Task<ActionResult> HandleAsync(QuizSolution request, CancellationToken cancellationToken = default)
    {
        await _solutionService.SaveSolution(request, cancellationToken);

        return Ok();
    }
}