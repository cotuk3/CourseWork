using System.Runtime.Serialization;

namespace BusinessLogicLayer.Exceptions;
[Serializable]
public class AnswerException : Exception
{
    public AnswerException()
        : base("You has exceeded max count of answers!")
    {
    }

    public AnswerException(string? message)
        : base(message)
    {
    }

    public AnswerException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected AnswerException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}