using ApolloStage;
using ApolloStage.DTO;

namespace ApolloStage
{
    public class Track
    {
        public string TrackId { get; set; }
        public string TrackName { get; set; }
        public string AlbumId { get; set; }
        public int TrackNumber { get; set; }
        public int DiscNumber { get; set; }
        public string Href { get; set; }
        public bool IsLocal { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
        public string DurationMinutes { get; set; }
    }


}
