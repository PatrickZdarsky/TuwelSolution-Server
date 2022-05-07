using System.Security.Principal;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ServiceLibrary;
using TuwelSolution.Models;

namespace TuwelSolution.Services;

public class SolutionService : ISolutionService
{
    private readonly IMongoCollection<QuizSolution> _collection;
    private readonly FilterDefinitionBuilder<QuizSolution> _filterBuilder = Builders<QuizSolution>.Filter;

    public SolutionService(IOptions<MongoDbSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        
        _collection = database.GetCollection<QuizSolution>("tuwelsolutions");
    }

    public async Task<QuizSolution?> GetSolution(string identifier, CancellationToken token = default)
    {
        return await _collection.Find(solution => solution.Quiz == identifier).FirstOrDefaultAsync(token);
    }

    public async Task<SolutionUpdateResult> SaveSolution(QuizSolution solution, CancellationToken token = default)
    {
        var existingSolution = await GetSolution(solution.Quiz, token);

        if (existingSolution is null)
        {
            await _collection.InsertOneAsync(solution, default, token);
            return new SolutionUpdateResult{ Solution = solution, Status = SolutionUpdateResult.UpdateStatus.Created };
        }
        
        //Add missing questions and merge existing ones
        solution.Questions.ForEach(question =>
        {
            var existingQuestion = existingSolution.Questions
                .FirstOrDefault(q => q.Text == question.Text && q.ImgHash == question.ImgHash);


            if (existingQuestion == null)
            {
                existingSolution.Questions.Add(question);
            }
            else
            {
                //Merge the answers
                question.Answers.Where(answer => !existingQuestion.Answers.Contains(answer))
                    .ToList().ForEach(answer => existingQuestion.Answers.Add(answer));
            }
        });

        //Increase version
        existingSolution.Version += 1;
        
        Console.WriteLine("Version: "+(existingSolution.Version-1));
        
        //Save
        var result = await _collection.ReplaceOneAsync(
            _filterBuilder.And(
                _filterBuilder.Eq(q => q.Quiz, existingSolution.Quiz), 
                _filterBuilder.Eq(q => q.Version, existingSolution.Version-1)), existingSolution,
            new ReplaceOptions(), token);

        return new SolutionUpdateResult
        {
            Solution = existingSolution,
            Status = result.IsAcknowledged && result.ModifiedCount == 1
                ? SolutionUpdateResult.UpdateStatus.Merged
                : SolutionUpdateResult.UpdateStatus.Error
        };
    }
}