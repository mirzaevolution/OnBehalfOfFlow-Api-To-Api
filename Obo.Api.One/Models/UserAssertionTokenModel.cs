namespace Obo.Api.One.Models
{
    public class UserAssertionTokenModel
    {
        public string AccessToken { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset ExpiredDateUtc { get; set; }
    }
}
