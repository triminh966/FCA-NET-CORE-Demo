using System;

namespace FCA.Data.Entities.FCA
{
    public class ChallengeMetricEntry
    {
        public int ChallengeMetricEntryId { get; set; }
        public int ChallengeId { get; set; }
        public int MetricEntryId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
