using System;
using UnityEngine;

public class BasicMessage
{
    public static BasicMessage Empty 
    {
        get { return new BasicMessage(); }
    }

    public string Message { get; set; }

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
    public T Value { get; set; }

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

public class IntegerMessage : GenericMessage<int>
{
    public IntegerMessage(string message, int value)
        : base(message, value)
    {
    }

    public void Set(string message, int value)
    {
        Message = message;
        Value = value;
    }
}