using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using UserAPI.Application.Common.Model.Constant;

namespace UserAPI.Application.Common.Model.Result
{
    public abstract class ErrorResult
    {
        public string Error { get; }
        public IEnumerable<ErrorData> Data { get; }

        public ErrorResult(string error, IEnumerable<ErrorData> data = null)
        {
            Error = error;
            Data = data ?? Enumerable.Empty<ErrorData>();
        }
    }

    public class ErrorData
    {
        public string Key { get; }
        public string Value { get; }

        public ErrorData(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public class ValidationFail : ErrorResult
    {
        protected ValidationFail(string error) : base(error)
        {
        }

        protected ValidationFail(string error, IEnumerable<ErrorData> data) : base(error, data)
        {
        }

        public static ValidationFail FromMessage(string msg) => new(msg);

        public static ValidationFail FromValidationResult(ValidationResult result) =>
            new(ErrorMessage.ValidationFail, result.Errors.Select(i => new ErrorData(i.PropertyName, i.ErrorMessage)));
    }

    public class ConflictResult : ErrorResult
    {
        protected ConflictResult(string error) : base(error)
        {
        }

        public static ConflictResult FromMessage(string msg) => new(msg);
    }

    public class InternalError : ErrorResult
    {
        private InternalError(string error) : base(error)
        {
        }

        public static InternalError Default => FromMessage(ErrorMessage.InternalError);

        public static InternalError FromMessage(string msg) => new(msg);
    }
}