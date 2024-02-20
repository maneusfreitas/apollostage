namespace ApolloStage
{
    public interface IHttpClientHelper
    {
        Task<string> SendAysnc(string uri, string token);
        Task<string> PostAsync(string uri, IEnumerable<KeyValuePair<string, string>> content);

    }
}
