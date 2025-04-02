public class AutoCompleteResult
{
    public string Address { get; set; }

    //Props for Photon/OSM, autoComplete gives detailed response with coordinates, city, PropertyNumber etc. 
    public string? City { get; set; }
    public string? PropertyNumber { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
} 