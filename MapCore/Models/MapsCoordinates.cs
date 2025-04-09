using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace MapCore.Models
{
    [ContentType(
        GUID = "A92D6E39-7BF5-4EE1-80A0-DB49FC62EC29",
        DisplayName = "Editor Map Widget with optional attributes",
        Description = "BlockData for storing latitude & longitude coordinates.",
        AvailableInEditMode = false)]
    public class MapsCoordinates : BlockData
    {
        [Display(Order = 1)]
        public virtual double? Latitude { get; set; }

        [Display(Order = 2)]
        public virtual double? Longitude { get; set; }
    }

    [ContentType(
        GUID = "8047e7fe-a2f6-4fdc-86aa-711ef4271a25",
        DisplayName = "Editor Map Widget with required attributes",
        Description = "BlockData for storing latitude & longitude coordinates.",
        AvailableInEditMode = false)]
    public class RequiredMapsCoordinates : BlockData //Since we can't apply [Required] annotation to block properties.
    {
        [Required]
        [Display(Order = 1)]
        public virtual double? Latitude { get; set; }

        [Required]
        [Display(Order = 2)]
        public virtual double? Longitude { get; set; }
    }
}
