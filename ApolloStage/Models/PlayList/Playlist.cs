using System;
using ApolloStage.DTO;

namespace ApolloStage
{
	public class Playlist
	{
		
        public bool Collaborative { get; set; }
        public string Description { get; set; }
        public ExternalUrls ExternalUrls { get; set; }
        public string Href { get; set; }
        public string Id { get; set; }
        public List<Image> Images { get; set; }
        public string Name { get; set; }
        public Owner Owner { get; set; }
        public bool Public { get; set; }
        public string SnapshotId { get; set; }
        public Tracks Tracks { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
        public string Total { get; set; }
    }
}

