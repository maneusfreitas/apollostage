namespace ApolloStage
{
    public sealed class Singleton :ISingleton
    {
        private static Singleton _instance;


        private string token;

        public Singleton()
        {
        }


        public Singleton GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Singleton();
            }
            return _instance;
        }

        public string GetToken()
        {
            return token;
        }

        public void SetToken(string token)
        {
            this.token = token;
        }


    }
}
