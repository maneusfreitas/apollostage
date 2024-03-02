namespace ApolloStage
{
    public interface ISingleton
    {
        public  Singleton GetInstance();
        string GetToken();
        void SetToken(string token);
    }
}