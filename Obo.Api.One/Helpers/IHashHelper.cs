namespace Obo.Api.One.Helpers
{
    public interface IHashHelper
    {
        Task<string> SHA256Hash(string input);
    }
}
