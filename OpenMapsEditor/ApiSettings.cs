namespace MusicFestival.Backend.Models
{
    public class ApiSettings
    {
        public string ApiUrl { get; set; }
        public double DefaultLatitude { get; set; }
        public double DefaultLongitude { get; set; }
        public int DefaultZoom { get; set; }
        public int MaxZoom { get; set; }
        public int MinZoom { get; set; }
    }
}
