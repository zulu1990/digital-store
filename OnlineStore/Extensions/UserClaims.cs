namespace OnlineStore.Extensions
{
    public static class UserClaims
    {
        public static Guid GetUserId(this HttpContext context)
        {
            var idAsString = context.User.Claims.ToList().FirstOrDefault(x => x.Type.Contains("nameidentifier"))?.Value;

            var id = string.IsNullOrEmpty(idAsString) ? Guid.Empty : Guid.Parse(idAsString);

            return id;
        }
    }
}
