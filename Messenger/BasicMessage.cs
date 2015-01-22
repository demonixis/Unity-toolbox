using System;
using UnityEngine;

public class BasicMessage
{
    public static BasicMessage Empty 
    {
        get { return new BasicMessage(); }
    }

    public string Message { get; protected set; }

    public BasicMessage()
    {
        Message = string.Empty;
    }

    public BasicMessage(string message)
    {
        Message = message;
    }
}

public class GenericMessage<T> : BasicMessage
{
    public T Value { get; protected set; }

    public GenericMessage(T value)
    {
        Value = value;
    }

    public GenericMessage(string message, T value)
        : base(message)
    {
        Value = value;
    }
}