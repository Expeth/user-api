using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Application.Common.Model.Result;
using OneOf;
using UserAPI.Application.Common.Abstraction.Service;
using UserAPI.Domain.Entity;

namespace UserAPI.Application.Handler.Command
{
    public static class AuthenticateUser
    {
        public sealed class Request : IRequest<OneOf<Response, ValidationFail, InvalidCredentials, InternalError>>
        {
            public string Login { get; }
            public string Password { get; }

            public Request(string login, string password)
            {
                Login = login;
                Password = password;
            }
        }

        public sealed class Response
        {
            public string Jwt { get; }
            public string RefreshToken { get; }

            public Response(string jwt, string refreshToken)
            {
                Jwt = jwt;
                RefreshToken = refreshToken;
            }
        }

        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(i => i.Login)
                    .NotNull()
                    .NotEmpty()
                    .Must(i => i.Length < 30);
                
                RuleFor(i => i.Password)
                    .NotNull()
                    .NotEmpty()
                    .Must(i => i.Length < 30);
            }
        }

        public sealed class Handler : IRequestHandler<Request,
            OneOf<Response, ValidationFail, InvalidCredentials, InternalError>>
        {
            private readonly IJwtFactory _jwtFactory;
            private readonly IValidator<Request> _validator;
            private readonly IRefreshTokenFactory _refreshTokenFactory;
            private readonly IRefreshTokenRepository _refreshTokenRepository;
            private readonly IUserRepository _userRepository;
            private readonly IPasswordService _passwordService;

            public Handler(
                IJwtFactory jwtFactory,
                IUserRepository userRepository,
                IPasswordService passwordService,
                IRefreshTokenRepository refreshTokenRepository,
                IRefreshTokenFactory refreshTokenFactory,
                IValidator<Request> validator)
            {
                _jwtFactory = jwtFactory;
                _userRepository = userRepository;
                _passwordService = passwordService;
                _refreshTokenRepository = refreshTokenRepository;
                _refreshTokenFactory = refreshTokenFactory;
                _validator = validator;
            }

            public async Task<OneOf<Response, ValidationFail, InvalidCredentials, InternalError>> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid) 
                    return ValidationFail.FromValidationResult(validationResult);

                try
                {
                    var user = await _userRepository.GetAsync(request.Login.ToLower());
                    if (user == null)
                    {
                        //User not found
                        return new InvalidCredentials();
                    }
                    
                    if (!_passwordService.ValidateHash(user.PasswordHash, request.Password, user.PasswordSalt))
                    {
                        //Password doesn't match
                        return new InvalidCredentials();
                    }

                    var jwt = await _jwtFactory.CreateAsync(new SessionEntity(user.Id));
                    var refreshToken = _refreshTokenFactory.Create(user.Id);
                    var isCreated = await _refreshTokenRepository.CreateAsync(refreshToken);

                    if (!isCreated)
                    {
                        return InternalError.FromMessage("Refresh token failed to create.");
                    }
                    
                    return new Response(jwt, refreshToken.Id);
                }
                catch (Exception e)
                {
                    //TODO: add logging
                    return InternalError.FromException(e);
                }
            }
        }
    }
}