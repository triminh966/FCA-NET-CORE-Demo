using System;

namespace FCA.Data.Entities.FCA
{
    public class MetricEntry
    {
        public int MetricEntryId { get; set; }
        public string MetricKey { get; set; }
        public int EquipmentId { get; set; }
        public string MetricTitle { get; set; }
        public int EntryTypeId { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
