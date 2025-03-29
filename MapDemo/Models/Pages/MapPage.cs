using MapCore.Models;
using System.ComponentModel.DataAnnotations;

namespace MapDemo.Models.Pages
{
    [ContentType(GUID = "5049a00b-8ed1-49c5-92ab-a6d273b8fcd6", AvailableInEditMode = true)]
    public class MapPage : PageData
    {
        [Display(GroupName = SystemTabNames.Content, Order = 1)]
        public virtual MapsCoordinates Map { get; set; }
    }
}
