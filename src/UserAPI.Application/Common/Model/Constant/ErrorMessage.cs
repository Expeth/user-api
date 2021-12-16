namespace UserAPI.Application.Common.Model.Constant
{
    public static class ErrorMessage
    {
        public static readonly string InternalError = "Server error";
        public static readonly string ValidationFail = "Validation fail";
        public static readonly string InvalidCredentials = "Invalid credentials";
        public static readonly string UserNotUnique = "User must be unique";
        public static readonly string InvalidRefreshToken = "Invalid refresh token";
        public static readonly string InvalidJWT = "Invalid JWT";
    }
}