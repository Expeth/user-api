using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using UserAPI.Application.Common.Model.Result;
using UserAPI.Application.Common.Validation;
using OneOf;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Application.Common.Abstraction.Service;
using UserAPI.Domain.Entity;
using UserAPI.Domain.Enum;

namespace UserAPI.Application.Handler.Command
{
    public static class RegisterUser
    {
        public sealed class Request : IRequest<OneOf<Response, ValidationFail, InternalError>>
        {
            public string Email { get; }
            public string Username { get; }
            public string Password { get; }
            public string FirstName { get; }
            public string LastName { get; }
            public string MiddleName { get; }

            public Request(string email, string username, string password, string firstName, string lastName,
                string middleName)
            {
                Email = email;
                Username = username;
                Password = password;
                FirstName = firstName;
                LastName = lastName;
                MiddleName = middleName;
            }
        }

        public sealed class Response
        {
            public string Id { get; }

            public Response(string id)
            {
                Id = id;
            }
        }

        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(i => i.Email)
                    .NotNull()
                    .NotEmpty()
                    .Must(ValidationRule.Email);

                RuleFor(i => i.Username)
                    .NotNull()
                    .NotEmpty()
                    .Must(ValidationRule.Username);

                RuleFor(i => i.Password)
                    .NotNull()
                    .NotEmpty()
                    .Must(ValidationRule.Password);

                //TODO: add validations for FirstName, LastName, MiddleName
            }
        }

        public sealed class Handler : IRequestHandler<Request, OneOf<Response, ValidationFail, InternalError>>
        {
            private readonly IValidator<Request> _validator;
            private readonly IUserRepository _userRepository;
            private readonly IPasswordService _passwordService;

            public Handler(IValidator<Request> validator, IUserRepository userRepository,
                IPasswordService passwordService)
            {
                _validator = validator;
                _userRepository = userRepository;
                _passwordService = passwordService;
            }

            public async Task<OneOf<Response, ValidationFail, InternalError>> Handle(Request request,
                CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid) 
                    return ValidationFail.FromValidationResult(validationResult);

                try
                {
                    var passwordHash = _passwordService.GenerateHash(request.Password);
                    var entity = new UserEntity(Guid.NewGuid().ToString(), request.FirstName, request.MiddleName,
                        request.LastName, null, request.Username, passwordHash.Hash, passwordHash.Salt,
                        (int)HashingAlgorithm.Pbkdf2,
                        request.Email);
                    
                    var isCreated = await _userRepository.CreateAsync(entity);
                    return isCreated ? new Response(entity.Id) : new ValidationFail("User should be unique");
                }
                catch (Exception e)
                {
                    //TODO: logging
                    return InternalError.FromException(e);
                }
            }
        }
    }
}