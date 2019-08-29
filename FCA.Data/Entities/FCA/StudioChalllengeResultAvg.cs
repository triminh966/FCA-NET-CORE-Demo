using System;

namespace FCA.Data.Entities
{
    public class StudioChalllengeResultAvg
    {
        public int StudioChallengeResultAvgId { get; set; }
        public int ChallengeId { get; set; }
        public double? AverageResult { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
