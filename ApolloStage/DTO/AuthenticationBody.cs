namespace ApolloStage.DTO
{
    public class AuthenticationBody
    {
        private readonly IConfiguration configuration;

        public AuthenticationBody(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private string client_id => configuration["ClientId"];
        private string client_secret => configuration["ClientSecret"];
        private string grant_type => "client_credentials";

        public KeyValuePair<string, string>[] GetBody()
        {

            var Body = new KeyValuePair<string, string>[]
            {
                     new KeyValuePair<string, string>("client_id", client_id),
                    new KeyValuePair<string, string>("client_secret", client_secret),
                    new KeyValuePair<string, string>("grant_type", grant_type),
            };

            return Body;
        }
    }
}
