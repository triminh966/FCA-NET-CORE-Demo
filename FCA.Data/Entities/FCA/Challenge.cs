using System;

namespace FCA.Data.Entities.FCA
{
    public class Challenge
    {
        public int ChallengeId { get; set; }
        public string ChallengeUUId { get; set; }
        public int ChallengeCategoryId { get; set; }
        public int CountryId { get; set; }
        public int StudioId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MetricEntries { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual ChallengeCategory ChallengeCategory { get; set; }
    }
}
