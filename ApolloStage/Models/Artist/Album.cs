using ApolloStage;
using ApolloStage.DTO;

namespace ApolloStage
{
    public class Album
    {
        public string album_type { get; set; }
        public List<Artist> artists { get; set; }
        public List<string> available_markets { get; set; }
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public int total_tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
        public string label { get; set; }
        public string popularity { get; set; }
        public string duration_ms_album { get; set; }
        public List<string> Genres { get; set; }
        public int classificacaoEspecifica { get; set; }
        public List<AlbumItem> Albums { get; set; }
        public DateTime ReleaseDate { get; set; }



        public void SetReleaseDate()
        {
            try
            {
                if (release_date_precision == "day")
                {
                    ReleaseDate = DateTime.Parse(release_date);
                }
                else
                {
                    ReleaseDate = new DateTime(Convert.ToInt32(release_date), 01, 01);
                }
            }
            catch (Exception e)
            {

                throw;
            }



        }
    }


}
