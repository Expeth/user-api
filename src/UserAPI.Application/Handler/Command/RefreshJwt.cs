using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using UserAPI.Application.Common.Model.Result;
using OneOf;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Application.Common.Extension;
using UserAPI.Domain.Entity;
using UserAPI.Domain.ValueObject;

namespace UserAPI.Application.Handler.Command
{
    public static class RefreshJwt
    {
        public sealed class Request : IRequest<OneOf<Response, ValidationFail, InternalError>>
        {
            public JwtClaims JwtClaims { get; }
            public string RefreshToken { get; }

            public Request(JwtClaims jwtClaims, string refreshToken)
            {
                JwtClaims = jwtClaims;
                RefreshToken = refreshToken;
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

        public sealed class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(i => i.RefreshToken)
                    .NotNull()
                    .NotEmpty();

                RuleFor(i => i.JwtClaims.UserId)
                    .NotNull()
                    .NotEmpty();

                RuleFor(i => i.JwtClaims.IssuedAt)
                    .NotNull()
                    .Must(i => i.IsPassed())
                    .WithMessage("Invalid JWT.");
                
                RuleFor(i => i.JwtClaims.ExpiresAt)
                    .NotNull()
                    .Must(i => i.IsPassed())
                    .WithMessage("Invalid JWT.");
            }
        }

        public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenEntity>
        {
            public RefreshTokenValidator()
            {
                RuleFor(i => i.IsUsed)
                    .Equal(_ => false)
                    .WithMessage("Invalid refresh token.");
                
                RuleFor(i => i.IsDeclined)
                    .Equal(_ => false)
                    .WithMessage("Invalid refresh token.");
                
                RuleFor(i => i.ExpiresAt)
                    .Must(i => !i.IsPassed())
                    .WithMessage("Invalid refresh token.");
                
                RuleFor(i => i.IssuedAt)
                    .Must(i => i.IsPassed())
                    .WithMessage("Invalid refresh token.");
            }
        }

        public sealed class Handler : IRequestHandler<Request,
            OneOf<Response, ValidationFail, InternalError>>
        {
            private readonly IRefreshTokenRepository _refreshTokenRepository;
            private readonly IRefreshTokenFactory _refreshTokenFactory;
            private readonly IValidator<RefreshTokenEntity> _rtValidator;
            private readonly IValidator<Request> _requestValidator;
            private readonly IJwtFactory _jwtFactory;
            
            public Handler(
                IRefreshTokenRepository refreshTokenRepository,
                IRefreshTokenFactory refreshTokenFactory,
                IJwtFactory jwtFactory,
                IValidator<Request> requestValidator,
                IValidator<RefreshTokenEntity> rtValidator)
            {
                _refreshTokenRepository = refreshTokenRepository;
                _refreshTokenFactory = refreshTokenFactory;
                _jwtFactory = jwtFactory;
                _requestValidator = requestValidator;
                _rtValidator = rtValidator;
            }

            public async Task<OneOf<Response, ValidationFail, InternalError>> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);
                if (!requestValidationResult.IsValid) 
                    return ValidationFail.FromValidationResult(requestValidationResult);
                
                try
                {
                    var refreshToken = await _refreshTokenRepository.GetAsync(request.RefreshToken);
                    
                    var rtValidationResult = await _rtValidator.ValidateAsync(refreshToken, cancellationToken);
                    if (!rtValidationResult.IsValid) 
                        return ValidationFail.FromValidationResult(rtValidationResult);

                    if (refreshToken.UserId != request.JwtClaims.UserId)
                    {
                        return new ValidationFail("Invalid refresh token.");
                    }
                    
                    var isUpdated = await _refreshTokenRepository.SetUsedAsync(refreshToken.Id);

                    if (!isUpdated)
                    {
                        return InternalError.FromMessage("Refresh token failed to update.");
                    }
                    
                    var jwt = await _jwtFactory.CreateAsync(new SessionEntity(request.JwtClaims.UserId));
                    var newRefreshToken = _refreshTokenFactory.Create(request.JwtClaims.UserId);
                    var isCreated = await _refreshTokenRepository.CreateAsync(newRefreshToken);

                    if (!isCreated)
                    {
                        return InternalError.FromMessage("Refresh token failed to create.");
                    }

                    return new Response(jwt, newRefreshToken.Id);
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