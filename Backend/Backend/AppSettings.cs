namespace Backend
{
    public class AppSettings
    {
        public static string ConnectionStrings { get; private set; }
        public static string Secret { get; set; }
        public static List<string> CORS { get; private set; }

        // refresh token time to live (in days), inactive tokens are
        // automatically deleted from the database after this time
        public static int RefreshTokenTTL { get; set; }

        public static string EmailFrom { get; set; }
        public static string SmtpHost { get; set; }
        public static int SmtpPort { get; set; }
        public static string SmtpUser { get; set; }
        public static string SmtpPass { get; set; }
    }
}