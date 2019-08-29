using System;

namespace FCA.Data.Entities
{
    public class Class
    {
        public long ClassId { get; set; }
        public string ClassUuid { get; set; }
        public int? StudioId { get; set; }
        public int? CoachId { get; set; }
        public int? LocationId { get; set; }
        public int? MboclassId { get; set; }
        public int? MbostudioId { get; set; }
        public int? MboprogramId { get; set; }
        public int? MboclassScheduleId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public int RoomNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ProgramName { get; set; }
        public string ProgramScheduleType { get; set; }
        public int? ProgramCancelOffset { get; set; }
        public int? MaxCapacity { get; set; }
        public int? TotalBooked { get; set; }
        public int? TotalBookedWaitlist { get; set; }
        public byte? IsWaitlistAvailable { get; set; }
        public byte? IsSubstitute { get; set; }
        public byte? IsCancelled { get; set; }
        public byte? IsActive { get; set; }
        public byte? IsAvailable { get; set; }
        public byte? IsEnrolled { get; set; }
        public byte? IsHideCancel { get; set; }
        public byte IsUnscheduled { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public byte? IsDeleted { get; set; }

        public virtual Studio Studio { get; set; }
    }
}
