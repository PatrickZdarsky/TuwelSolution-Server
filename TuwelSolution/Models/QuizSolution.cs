using MongoDB.Bson.Serialization.Attributes;

namespace TuwelSolution.Models;

public class QuizSolution
{
    [BsonId]
    public string Quiz { get; set; }

    public List<Question> Questions { get; set; }

    public class Question
    {
        public string Text { get; set; }

        public string ImgHash { get; set; }

        public List<string> Answers { get; set; }
    }
}