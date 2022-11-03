﻿using System.Runtime.Serialization;

namespace BusinessLogicLayer.Exceptions;
[Serializable]
public class UserException : Exception
{
    public UserException()
    {
    }

    public UserException(string? message) 
        : base($"{message} was in wrong format")
    {
    }
    public UserException(string? message, string? message1)
        : base($"{message} and {message1} were in wrong format")
    {
    }

    public UserException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected UserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}