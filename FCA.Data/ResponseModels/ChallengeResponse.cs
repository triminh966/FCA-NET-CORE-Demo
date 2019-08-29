using System;
using FCA.Data.Entities.FCA;

namespace FCA.Data.ResponseModels
{
    public class ChallengeResponse
    {
        public int ChallengeId { get; set; }
        public string ChallengeUUId { get; set; }
        public int ChallengeCategoryId { get; set; }
        public int CountryId { get; set; }
        public int StudioId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MetricEntry MetricEntry { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
