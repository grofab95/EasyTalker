using System;

namespace EasyTalker.Core.Exceptions;

public class PasswordValidatorException : Exception
{
    public string[] Errors { get; }

    public PasswordValidatorException(string[] errors)
    {
        Errors = errors;
    }
}