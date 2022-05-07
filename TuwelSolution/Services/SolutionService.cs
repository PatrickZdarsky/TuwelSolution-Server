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

    public async Task<QuizSolution> GetSolution(string identifier, CancellationToken token = default)
    {
        return await _collection.Find(solution => solution.Quiz == identifier).FirstOrDefaultAsync(token);
    }

    public async Task SaveSolution(QuizSolution solution, CancellationToken token = default)
    {
       await _collection.ReplaceOneAsync(_filterBuilder.Eq(quizSolution => quizSolution.Quiz, solution.Quiz), solution,
            new ReplaceOptions() {IsUpsert = true}, token);
    }
}