using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace OpenMapsEditor
{
	[ContentType(
		GUID = "A92D6E39-7BF5-4EE1-80A0-DB49FC62EC29",
		DisplayName = "Open Maps coordinates",
		Description = "BlockData for storing latitude & longitude coordinates.",
		AvailableInEditMode = false)]
	public class MapsCoordinates : BlockData
	{
		[Display(Order = 1)]
		public virtual double? Latitude { get; set; }

		[Display(Order = 2)]
		public virtual double? Longitude { get; set; }
	}
}
