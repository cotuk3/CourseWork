using System.Runtime.Serialization;

namespace BusinessLogicLayer.Exceptions;
[Serializable]
public class QuestionException : Exception
{
    public QuestionException()
        : base("Question is not valid!")
    {
    }
    public QuestionException(string? message) : base(message)
    {
    }

    public QuestionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected QuestionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}