using System;
using System.Collections.Generic;

namespace FCA.Data.Entities
{
    public class StudioLocation
    {
        public StudioLocation()
        {
            Studio = new HashSet<Studio>();
        }

        public int StudioLocationId { get; set; }
        public string BillToAddress { get; set; }
        public string BillToAddress2 { get; set; }
        public string BillToCity { get; set; }
        public string BillToState { get; set; }
        public string BillToPostalCode { get; set; }
        public string BillToRegion { get; set; }
        public int? BillToCountryId { get; set; }
        public string BillToCountry { get; set; }
        public string ShipToAddress { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToState { get; set; }
        public string ShipToPostalCode { get; set; }
        public string ShipToRegion { get; set; }
        public int? ShipToCountryId { get; set; }
        public string ShipToCountry { get; set; }
        public string PhoneNumber { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public byte? IsDeleted { get; set; }
        
        public virtual Country BillToCountryNavigation { get; set; }
        public ICollection<Studio> Studio { get; set; }
    }
}
