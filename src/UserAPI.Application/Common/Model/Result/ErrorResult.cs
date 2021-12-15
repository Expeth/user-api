using System;
using System.Linq;
using FluentValidation.Results;

namespace UserAPI.Application.Common.Model.Result
{
    public abstract class ErrorResult
    {
        public string Error { get; }

        public ErrorResult(string error)
        {
            Error = error;
        }
    }

    public class ValidationFail : ErrorResult
    {
        public ValidationFail(string error) : base(error)
        {
        }

        public static ValidationFail FromValidationResult(ValidationResult result) =>
            new ValidationFail(result?.Errors?.FirstOrDefault()?.ErrorMessage);
    }

    public class InternalError : ErrorResult
    {
        public InternalError(string error) : base(error)
        {
        }

        public static InternalError FromException(Exception e) => FromMessage(e.Message);
        
        public static InternalError FromMessage(string msg) => new InternalError(msg);
    }

    public class InvalidCredentials : ValidationFail
    {
        public InvalidCredentials() : base("Invalid Credentials")
        {
        }
    }
}