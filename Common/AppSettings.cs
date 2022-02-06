using System;

namespace Common
{
    public class AppSettings
    {
        public JwtSettings JWT { get; set; }
        public SmtpSettings SMTP { get; set; }

    }
    public class JwtSettings
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string SecretKey { get; set; }
        public int TokenExpiresIn { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
    }

    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string From { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
    }
}
