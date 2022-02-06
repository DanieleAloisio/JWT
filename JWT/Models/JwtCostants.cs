using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JWT.Models
{
    public class JwtCostants
    {
        public const string Issuer = "MVS";
        public const string Audience = "ApiUser";
        public const string Key = "X4zPlmIP2nl9noCLPZyxVAUJEue4qb4cOcX9kdjxnMZq3nk0qT2madQkwYS71OT";
        public const double TokenExpiresIn = 10;
        public const double RefreshTokenExpiresIn = 7;

        public const string AuthSchemes =
            "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
    }
}
