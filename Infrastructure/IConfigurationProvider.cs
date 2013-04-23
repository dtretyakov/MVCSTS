namespace MvcSTSApplication.Infrastructure
{
    public interface IConfigurationProvider
    {
        string Get(string key);
    }
}