namespace MusicFestival.Backend.Models
{
    public class ApiSettings
    {
        public string ApiTileUrl { get; set; }
        public string ApiSearchUrl { get; set; }
        public string Identifier { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthType { get; set; }
        public double DefaultLatitude { get; set; }
        public double DefaultLongitude { get; set; }
        public int DefaultZoom { get; set; }
        public int MaxZoom { get; set; }
        public int MinZoom { get; set; }
    }
}