using System;
using System.Collections.Generic;

namespace FCA.Data.Entities
{
    public class Studio
    {
        public Studio()
        {
            Class = new HashSet<Class>();
        }

        public int StudioId { get; set; }
        public string StudioUuid { get; set; }
        public int? MbostudioId { get; set; }
        public string LegacyStudioNumber { get; set; }
        public string StudioName { get; set; }
        public int? StudioPhysicalLocationId { get; set; }
        public byte? IsDomestic { get; set; }
        public byte? IsCorporate { get; set; }
        public string TimeZone { get; set; }
        public string ContactEmail { get; set; }
        public string SoftwareTokenId { get; set; }
        public string Environment { get; set; }
        public string PricingLevel { get; set; }
        public short? TaxInclusivePricing { get; set; }
        public decimal? TaxRate { get; set; }
        public short? AcceptsVisaMasterCard { get; set; }
        public short? AcceptsAmex { get; set; }
        public short? AcceptsDiscover { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public DateTime? OpenDate { get; set; }
        public int? StudioTypeId { get; set; }
        public int? PosTypeId { get; set; }
        public int? MarketId { get; set; }
        public int? AreaId { get; set; }
        public int? StateId { get; set; }
        public string LogoUrl { get; set; }
        public string PageColor1 { get; set; }
        public string PageColor2 { get; set; }
        public string PageColor3 { get; set; }
        public string PageColor4 { get; set; }
        public byte? SmspackageEnabled { get; set; }
        public byte? AllowsDashboardAccess { get; set; }
        public decimal? RoyaltyRate { get; set; }
        public decimal? MarketingFundRate { get; set; }
        public decimal? CommissionPercent { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public byte? IsDeleted { get; set; }
        
        public virtual StudioLocation StudioLocation { get; set; }
        public virtual ICollection<Class> Class { get; set; }
    }
}
