using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TuwelSolution.Models;

public class QuizSolution
{
    [BsonId]
    public string Quiz { get; set; }
    
    public int Version { get; set; }

    public List<Question> Questions { get; set; }

    public class Question
    {
        public string Text { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public QuestionType Type { get; set; }

        public string? ImgHash { get; set; }

        public List<string> Answers { get; set; }


        protected bool Equals(Question other)
        {
            return Text == other.Text && Type == other.Type && ImgHash == other.ImgHash && Answers.Equals(other.Answers);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Question)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, (int)Type, ImgHash, Answers);
        }
    }

    public enum QuestionType
    {
        // ReSharper disable once InconsistentNaming, IdentifierTypo
        multichoice, multianswer
    }
}