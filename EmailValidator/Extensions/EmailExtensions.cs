namespace EmailValidator.Extensions
{
    public static class EmailExtensions
    {
        public static string GetEmailDomain(this string email)
        {
            return email.Split('@')[1];
        }
    }
}