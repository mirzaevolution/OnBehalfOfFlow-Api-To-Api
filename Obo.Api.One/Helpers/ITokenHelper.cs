namespace Obo.Api.One.Helpers
{
    public interface ITokenHelper
    {
        Task<string> GetAndRefreshToken();
    }
}
