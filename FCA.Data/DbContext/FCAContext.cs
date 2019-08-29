using System;
using FCA.Data.Entities;
using FCA.Data.Entities.FCA;
using Microsoft.EntityFrameworkCore;

namespace FCA.Data
{
    public class FCAContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public FCAContext()
        {
        }

        public FCAContext(DbContextOptions<FCAContext> options) : base(options)
        {
        }

        public virtual DbSet<Challenge> Challenge { get; set; }
        public virtual DbSet<StudioChalllengeResultAvg> StudioChalllengeResultAvg { get; set; }
        public virtual DbSet<ChallengeMetricEntry> ChallengeMetricEntry { get; set; }
        public virtual DbSet<MetricEntry> MetricEntry { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Challenge>(entity =>
            {
                entity.ToTable("Challenge");
                entity.HasIndex(e => e.ChallengeUUId).HasName("Idx_ChallengeUUId").IsUnique();
                entity.HasIndex(e => e.ChallengeCategoryId).HasName("Challenge_ChallengeCategoryId_idx");
                entity.HasIndex(e => e.StudioId).HasName("Challenge_Studio_Idx");
                entity.Property(e => e.ChallengeId).HasColumnType("int(11)");
                entity.Property(e => e.ChallengeUUId).IsRequired().HasColumnType("char(36)");
                entity.Property(e => e.ChallengeCategoryId).HasColumnType("int(11)");
                entity.Property(e => e.StudioId).HasColumnType("int(11)");
                entity.Property(e => e.CountryId).HasColumnType("int(11)");
                entity.Property(e => e.MetricEntries).HasColumnType("text").HasDefaultValueSql(null);
                entity.Property(e => e.StartDate).HasColumnType("datetime").HasDefaultValueSql(null);
                entity.Property(e => e.EndDate).HasColumnType("datetime").HasDefaultValueSql(null);
                entity.Property(e => e.DateCreated).HasColumnType("datetime").HasDefaultValueSql($"{DateTime.UtcNow}");
                entity.Property(e => e.DateUpdated).HasColumnType("datetime").HasDefaultValueSql($"{DateTime.UtcNow}");
                entity.HasOne(a => a.ChallengeCategory)
                    .WithMany(b => b.Challenge)
                    .HasForeignKey(c => c.ChallengeCategoryId)
                    .HasConstraintName("Challenge_ChallengeCategoryId_fk");
            });

            modelBuilder.Entity<StudioChalllengeResultAvg>(entity =>
                {
                    entity.ToTable("StudioChallengeResultAvg");
                    entity.HasIndex(e => e.CreatedBy).HasName("StudioChallengeResultAvg_CreatedBy_idx");
                    entity.HasIndex(e => e.UpdatedBy).HasName("StudioChallengeResultAvg_UpdatedBy_idx");
                    entity.HasIndex(e => e.ChallengeId).HasName("StudioChallengeResultAvg_Challeng_ChallengeId_idx");
                    entity.Property(e => e.StudioChallengeResultAvgId).HasColumnType("int(10)");
                    entity.Property(e => e.ChallengeId).HasColumnType("int(11)").IsRequired();
                    entity.Property(e => e.AverageResult).HasColumnType("float").HasDefaultValueSql(null);
                    entity.Property(e => e.CreatedBy).HasColumnType("char(36)").HasDefaultValueSql(null);
                    entity.Property(e => e.UpdatedBy).HasColumnType("char(36)").HasDefaultValueSql(null);
                    entity.Property(e => e.CreatedDate).HasColumnType("datetime").IsRequired().HasDefaultValueSql($"{DateTime.UtcNow}");
                    entity.Property(e => e.UpdatedDate).HasColumnType("datetime").IsRequired().HasDefaultValueSql($"{DateTime.UtcNow}");
                    entity.Property(e => e.IsDeleted).HasColumnType("tinyint(1)").HasDefaultValueSql("0");
                }
            );

            modelBuilder.Entity<ChallengeMetricEntry>(entity =>
                {
                    entity.ToTable("ChallengeMetricEntry");
                    entity.HasIndex(e => e.ChallengeId).HasName("ChallengeMetricEntry_ChallengeId_Uq").IsUnique();
                    entity.HasIndex(e => e.ChallengeId).HasName("ChallengeMetricEntry_Challenge_FK_idx");
                    entity.HasIndex(e => e.MetricEntryId).HasName("ChallengeMetricEntry_MetricEntry_FK_idx");
                    entity.Property(e => e.ChallengeMetricEntryId).HasColumnType("int(11)").IsRequired();
                    entity.Property(e => e.ChallengeId).HasColumnType("int(11)").IsRequired();
                    entity.Property(e => e.MetricEntryId).HasColumnType("int(11)").IsRequired();
                    entity.Property(e => e.DateCreated).HasColumnType("datetime").IsRequired().HasDefaultValueSql($"{DateTime.UtcNow}");
                    entity.Property(e => e.DateUpdated).HasColumnType("datetime").IsRequired().HasDefaultValueSql($"{DateTime.UtcNow}");
                }
            );

            modelBuilder.Entity<MetricEntry>(entity =>
                {
                    entity.ToTable("MetricEntry");
                    entity.HasIndex(e => new { e.MetricKey, e.EntryTypeId, e.EquipmentId }).HasName("MetricEntry_MetKey_EqId_EntTypeId_Uq").IsUnique();
                    entity.Property(e => e.MetricEntryId).HasColumnType("int(11)").IsRequired();
                    entity.Property(e => e.MetricKey).HasColumnType("varchar(36)").IsRequired();
                    entity.Property(e => e.EquipmentId).HasColumnType("int(11)").IsRequired();
                    entity.Property(e => e.EntryTypeId).HasColumnType("tinyint(4)").IsRequired();
                    entity.Property(e => e.MetricTitle).HasColumnType("varchar(50)").IsRequired();
                    entity.Property(e => e.MinValue).HasColumnType("varchar(100)").IsRequired().HasDefaultValueSql(null);
                    entity.Property(e => e.MaxValue).HasColumnType("varchar(100)").IsRequired().HasDefaultValueSql(null);
                    entity.Property(e => e.DateCreated).HasColumnType("datetime").IsRequired().HasDefaultValueSql($"{DateTime.UtcNow}");
                    entity.Property(e => e.DateUpdated).HasColumnType("datetime").IsRequired().HasDefaultValueSql($"{DateTime.UtcNow}");
                }
            );
        }
    }
}
