namespace TuwelSolution.Models;

public class SolutionUpdateResult
{
    public QuizSolution Solution { get; set; }

    public UpdateStatus Status { get; set; }
    
    public enum UpdateStatus
    {
        Created, Merged, Error
    }
}