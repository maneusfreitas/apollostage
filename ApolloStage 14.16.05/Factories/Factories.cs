using ApolloStage.DTO;

namespace ApolloStage.Factories
{
    public class Factories : IFactories.IFactories
    {
        public AuthenticationBody GetAuthentication(IConfiguration configuration)
        {
            return new AuthenticationBody(configuration);
        }

        public FormUrlEncodedContent GetFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content)
        {
           return new FormUrlEncodedContent(content);
        }
    }
}
