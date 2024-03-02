using ApolloStage.DTO;

namespace ApolloStage.Factories.IFactories
{
    public interface IFactories
    {
        AuthenticationBody GetAuthentication(IConfiguration configuration);
        FormUrlEncodedContent GetFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content);
    }
}
