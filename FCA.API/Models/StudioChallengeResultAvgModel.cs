using FCA.Data.Entities;

namespace FCA.API.Models
{
    public class StudioChallengeResultAvgModel : StudioChalllengeResultAvg
    {
        public int Rank { get; set; }
        public dynamic Difference { get; set; }
        public int TotalNumberOfRecords { get; set; }
        public int TotalNumberOfPage { get; set; }
    }

    public class StudioChallengeResultAvgParameters
    {
        public string Division { get; set; }
        public string StateCode { get; set; }
        public string CountryCode { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
