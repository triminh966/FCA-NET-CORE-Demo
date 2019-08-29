using System;
using System.Collections.Generic;

namespace FCA.Data.Entities
{
    public class Country
    {
        public Country()
        {
            StudioLocationBillToCountryNavigation = new HashSet<StudioLocation>();
        }

        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string Description { get; set; }
        public string CountryCurrencyCode { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public byte? IsDeleted { get; set; }
        
        public virtual ICollection<StudioLocation> StudioLocationBillToCountryNavigation { get; set; }
    }
}
