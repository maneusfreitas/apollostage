using System;
using ApolloStage.DTO;

namespace ApolloStage
{
    public class Artist
    {
        public Dictionary<string, string> external_urls { get; set; }
        public Followers followers { get; set; }
        public List<string> genres { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
        public Albums Albums { get; set; }
    }
}
