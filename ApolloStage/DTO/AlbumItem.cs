namespace ApolloStage.DTO
{
    public class AlbumItem
    {
        public string Album_Type { get; set; }
        public int Total_Tracks { get; set; }
        public string Release_Date { get; set; }
        public string Release_Date_Precision { get; set; }

        //public DateTime ReleaseDate { get { return ReleaseDate; } set {
        //        try
        //        {
        //            if (Release_Date_Precision == "day")
        //            {
        //                ReleaseDate = DateTime.Parse(Release_Date);
        //            }
        //            else
        //            {
        //                ReleaseDate = new DateTime(Convert.ToInt32(Release_Date), 01, 01);
        //            }
        //        }
        //        catch (Exception e)
        //        {

        //            throw e;
        //        }
        //    //} }

        public DateTime ReleaseDate { get; set; }
       


        public Image[] Images { get; set; }
        public string Name { get; set; }

        public void SetReleaseDate()
        {
            try
            {
                if (Release_Date_Precision == "day")
                {
                    ReleaseDate = DateTime.Parse(Release_Date);
                }
                else
                {
                    ReleaseDate = new DateTime(Convert.ToInt32(Release_Date), 01, 01);
                }
            }
            catch (Exception e)
            {

                throw;
            }
            


        }

    }

   
}
