using FCA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCA.Data.DbContext
{
    public class OTbaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public OTbaseContext()
        {
        }

        public OTbaseContext(DbContextOptions<OTbaseContext> options) : base(options)
        {
        }
        
        public virtual DbSet<Studio> Studio { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<StudioLocation> StudioLocation { get; set; }
        public virtual DbSet<Country> Country { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");
            
            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("Class");
                entity.HasIndex(e => e.ClassUuid)
                    .HasName("Idx_UNQ_Class_ClassUUId")
                    .IsUnique();
                entity.HasIndex(e => e.CoachId)
                    .HasName("Class_Coach_CoachId_FK_idx");
                entity.HasIndex(e => e.CreatedBy)
                    .HasName("Class_User_CreatedBy_FK_idx");
                entity.HasIndex(e => e.LocationId)
                    .HasName("Class_Location_LocationId_FK_idx");
                entity.HasIndex(e => e.StudioId)
                    .HasName("Class_Studio_StudioId_FK_idx");
                entity.HasIndex(e => e.UpdatedBy)
                    .HasName("Class_User_UpdatedBy_FK_idx");
                entity.HasIndex(e => new { e.MbostudioId, e.MboclassId })
                    .HasName("Class_MBOStudioId_MBOClassId_Idx");
                entity.HasIndex(e => new { e.MbostudioId, e.StartTime })
                    .HasName("Class_MBOStudioId_StartTime_Idx");
                entity.HasIndex(e => new { e.StudioId, e.MboclassId, e.RoomNumber, e.StartTime })
                    .HasName("Idx_UNQ_Class_MBO1")
                    .IsUnique();
                entity.Property(e => e.ClassId).HasColumnType("bigint(20)");
                entity.Property(e => e.ClassDescription)
                    .HasMaxLength(5000)
                    .IsUnicode(false);
                entity.Property(e => e.ClassName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ClassUuid)
                    .IsRequired()
                    .HasColumnName("ClassUUId")
                    .HasColumnType("char(36)");
                entity.Property(e => e.CoachId).HasColumnType("int(11)");
                entity.Property(e => e.CreatedBy).HasColumnType("char(36)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EndTime).HasColumnType("datetime(6)");
                entity.Property(e => e.IsActive)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");
                entity.Property(e => e.IsAvailable)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");
                entity.Property(e => e.IsCancelled).HasColumnType("tinyint(1)");
                entity.Property(e => e.IsDeleted)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");
                entity.Property(e => e.IsEnrolled).HasColumnType("tinyint(1)");
                entity.Property(e => e.IsHideCancel).HasColumnType("tinyint(1)");
                entity.Property(e => e.IsSubstitute).HasColumnType("tinyint(1)");
                entity.Property(e => e.IsUnscheduled)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");
                entity.Property(e => e.IsWaitlistAvailable).HasColumnType("tinyint(1)");
                entity.Property(e => e.LocationId).HasColumnType("int(11)");
                entity.Property(e => e.MaxCapacity).HasColumnType("int(11)");
                entity.Property(e => e.MboclassId)
                    .HasColumnName("MBOClassId")
                    .HasColumnType("int(11)");
                entity.Property(e => e.MboclassScheduleId)
                    .HasColumnName("MBOClassScheduleId")
                    .HasColumnType("int(11)");
                entity.Property(e => e.MboprogramId)
                    .HasColumnName("MBOProgramId")
                    .HasColumnType("int(11)");
                entity.Property(e => e.MbostudioId)
                    .HasColumnName("MBOStudioId")
                    .HasColumnType("int(11)");
                entity.Property(e => e.ProgramCancelOffset).HasColumnType("int(11)");
                entity.Property(e => e.ProgramName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ProgramScheduleType)
                    .HasMaxLength(45)
                    .IsUnicode(false);
                entity.Property(e => e.RoomNumber).HasColumnType("int(11)");
                entity.Property(e => e.StartTime).HasColumnType("datetime(6)");
                entity.Property(e => e.StudioId).HasColumnType("int(11)");
                entity.Property(e => e.TotalBooked).HasColumnType("int(11)");
                entity.Property(e => e.TotalBookedWaitlist).HasColumnType("int(11)");
                entity.Property(e => e.UpdatedBy).HasColumnType("char(36)");
                entity.Property(e => e.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Studio)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.StudioId)
                    .HasConstraintName("Class_Studio_StudioId_FK");
            });
            
            modelBuilder.Entity<Studio>(entity =>
            {
                entity.ToTable("Studio");
                entity.HasIndex(e => e.AreaId)
                    .HasName("Studio_Area_AreaId_FK_idx");
                entity.HasIndex(e => e.CreatedBy)
                    .HasName("Studio_User_CreatedBy_FK_idx");
                entity.HasIndex(e => e.LegacyStudioNumber)
                    .HasName("Studio_LegacyStudioNumber_uindex");
                entity.HasIndex(e => e.MarketId)
                    .HasName("Studio_Market_MarketId_FK_idx");
                entity.HasIndex(e => e.MbostudioId)
                    .HasName("Studio_MBOStudioId_idx")
                    .IsUnique();
                entity.HasIndex(e => e.PosTypeId)
                    .HasName("Studio_POSType_PosTypeId_FK_idx");
                entity.HasIndex(e => e.StateId)
                    .HasName("Studio_State_StateId_FK_idx");
                entity.HasIndex(e => e.StudioName)
                    .HasName("Studio_StudioName_uindex")
                    .IsUnique();
                entity.HasIndex(e => e.StudioPhysicalLocationId)
                    .HasName("Studio_StudioPhysicalLocation_StudioPhysicalLocationId_FK_idx");
                entity.HasIndex(e => e.StudioTypeId)
                    .HasName("Studio_StudioType_TypeId_FK_idx");
                entity.HasIndex(e => e.StudioUuid)
                    .HasName("Idx_UNQ_Studio_StudioUUId")
                    .IsUnique();
                entity.HasIndex(e => e.UpdatedBy)
                    .HasName("Studio_User_UpdatedBy_FK_idx");
                entity.Property(e => e.StudioId).HasColumnType("int(11)");
                entity.Property(e => e.AcceptsAmex)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");
                entity.Property(e => e.AcceptsDiscover)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");
                entity.Property(e => e.AcceptsVisaMasterCard)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");
                entity.Property(e => e.AllowsDashboardAccess).HasColumnType("tinyint(1)");
                entity.Property(e => e.AreaId).HasColumnType("int(11)");
                entity.Property(e => e.CommissionPercent).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedBy).HasColumnType("char(36)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Description)
                    .HasMaxLength(5000)
                    .IsUnicode(false);
                entity.Property(e => e.Environment)
                    .IsRequired()
                    .HasColumnType("enum('PROD','UAT','SIT','DEV')")
                    .HasDefaultValueSql("PROD");
                entity.Property(e => e.IsCorporate)
                    .HasColumnType("tinyint(1) unsigned")
                    .HasDefaultValueSql("0");
                entity.Property(e => e.IsDeleted)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");
                entity.Property(e => e.IsDomestic).HasColumnType("tinyint(1) unsigned");
                entity.Property(e => e.LegacyStudioNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);
                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.MarketId).HasColumnType("int(11)");
                entity.Property(e => e.MarketingFundRate).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MbostudioId)
                    .HasColumnName("MBOStudioId")
                    .HasColumnType("int(11)");
                entity.Property(e => e.PageColor1)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.PageColor2)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.PageColor3)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.PageColor4)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.PosTypeId).HasColumnType("int(11)");
                entity.Property(e => e.PricingLevel)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.RoyaltyRate).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SmspackageEnabled)
                    .HasColumnName("SMSPackageEnabled")
                    .HasColumnType("tinyint(1)");
                entity.Property(e => e.SoftwareTokenId)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.StateId).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("enum('Coming Soon','PreSale','Active','Inactive','Hidden','Temporarily Closed','Permanently Closed')");
                entity.Property(e => e.StudioName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.StudioPhysicalLocationId).HasColumnType("int(11)");
                entity.Property(e => e.StudioTypeId).HasColumnType("int(11)");
                entity.Property(e => e.StudioUuid)
                    .IsRequired()
                    .HasColumnName("StudioUUId")
                    .HasColumnType("char(36)");
                entity.Property(e => e.TaxInclusivePricing)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");
                entity.Property(e => e.TaxRate)
                    .HasColumnType("decimal(5,2)")
                    .HasDefaultValueSql("0.00");
                entity.Property(e => e.TimeZone).HasColumnType("enum('Africa/Abidjan','Africa/Accra','Africa/Addis_Ababa','Africa/Algiers','Africa/Asmara','Africa/Bamako','Africa/Bangui','Africa/Banjul','Africa/Bissau','Africa/Blantyre','Africa/Brazzaville','Africa/Bujumbura','Africa/Cairo','Africa/Casablanca','Africa/Ceuta','Africa/Conakry','Africa/Dakar','Africa/Dar_es_Salaam','Africa/Djibouti','Africa/Douala','Africa/El_Aaiun','Africa/Freetown','Africa/Gaborone','Africa/Harare','Africa/Johannesburg','Africa/Juba','Africa/Kampala','Africa/Khartoum','Africa/Kigali','Africa/Kinshasa','Africa/Lagos','Africa/Libreville','Africa/Lome','Africa/Luanda','Africa/Lubumbashi','Africa/Lusaka','Africa/Malabo','Africa/Maputo','Africa/Maseru','Africa/Mbabane','Africa/Mogadishu','Africa/Monrovia','Africa/Nairobi','Africa/Ndjamena','Africa/Niamey','Africa/Nouakchott','Africa/Ouagadougou','Africa/Porto-Novo','Africa/Sao_Tome','Africa/Tripoli','Africa/Tunis','Africa/Windhoek','America/Adak','America/Anchorage','America/Anguilla','America/Antigua','America/Araguaina','America/Argentina/Buenos_Aires','America/Argentina/Catamarca','America/Argentina/Cordoba','America/Argentina/Jujuy','America/Argentina/La_Rioja','America/Argentina/Mendoza','America/Argentina/Rio_Gallegos','America/Argentina/Salta','America/Argentina/San_Juan','America/Argentina/San_Luis','America/Argentina/Tucuman','America/Argentina/Ushuaia','America/Aruba','America/Asuncion','America/Atikokan','America/Bahia','America/Bahia_Banderas','America/Barbados','America/Belem','America/Belize','America/Blanc-Sablon','America/Boa_Vista','America/Bogota','America/Boise','America/Cambridge_Bay','America/Campo_Grande','America/Cancun','America/Caracas','America/Cayenne','America/Cayman','America/Chicago','America/Chihuahua','America/Costa_Rica','America/Creston','America/Cuiaba','America/Curacao','America/Danmarkshavn','America/Dawson','America/Dawson_Creek','America/Denver','America/Detroit','America/Dominica','America/Edmonton','America/Eirunepe','America/El_Salvador','America/Fort_Nelson','America/Fortaleza','America/Glace_Bay','America/Godthab','America/Goose_Bay','America/Grand_Turk','America/Grenada','America/Guadeloupe','America/Guatemala','America/Guayaquil','America/Guyana','America/Halifax','America/Indianapolis','America/Havana','America/Hermosillo','America/Indiana/Indianapolis','America/Indiana/Knox','America/Indiana/Marengo','America/Indiana/Petersburg','America/Indiana/Tell_City','America/Indiana/Vevay','America/Indiana/Vincennes','America/Indiana/Winamac','America/Inuvik','America/Iqaluit','America/Jamaica','America/Juneau','America/Kentucky/Louisville','America/Kentucky/Monticello','America/Kralendijk','America/La_Paz','America/Lima','America/Los_Angeles','America/Lower_Princes','America/Maceio','America/Managua','America/Manaus','America/Marigot','America/Martinique','America/Matamoros','America/Mazatlan','America/Menominee','America/Merida','America/Metlakatla','America/Mexico_City','America/Miquelon','America/Moncton','America/Monterrey','America/Montevideo','America/Montserrat','America/Nassau','America/New_York','America/Nipigon','America/Nome','America/Noronha','America/North_Dakota/Beulah','America/North_Dakota/Center','America/North_Dakota/New_Salem','America/Ojinaga','America/Panama','America/Pangnirtung','America/Paramaribo','America/Phoenix','America/Port-au-Prince','America/Port_of_Spain','America/Porto_Velho','America/Puerto_Rico','America/Punta_Arenas','America/Rainy_River','America/Rankin_Inlet','America/Recife','America/Regina','America/Santa_Isabel','America/Resolute','America/Rio_Branco','America/Santarem','America/Santiago','America/Santo_Domingo','America/Sao_Paulo','America/Scoresbysund','America/Sitka','America/St_Barthelemy','America/St_Johns','America/St_Kitts','America/St_Lucia','America/St_Thomas','America/St_Vincent','America/Swift_Current','America/Tegucigalpa','America/Thule','America/Thunder_Bay','America/Tijuana','America/Toronto','America/Tortola','America/Vancouver','America/Whitehorse','America/Winnipeg','America/Yakutat','America/Yellowknife','Antarctica/Casey','Antarctica/Davis','Antarctica/DumontDUrville','Antarctica/Macquarie','Antarctica/Mawson','Antarctica/McMurdo','Antarctica/Palmer','Antarctica/Rothera','Antarctica/Syowa','Antarctica/Troll','Antarctica/Vostok','Arctic/Longyearbyen','Asia/Aden','Asia/Almaty','Asia/Amman','Asia/Anadyr','Asia/Aqtau','Asia/Aqtobe','Asia/Ashgabat','Asia/Atyrau','Asia/Baghdad','Asia/Bahrain','Asia/Baku','Asia/Bangkok','Asia/Barnaul','Asia/Beirut','Asia/Bishkek','Asia/Brunei','Asia/Chita','Asia/Choibalsan','Asia/Colombo','Asia/Damascus','Asia/Dhaka','Asia/Dili','Asia/Dubai','Asia/Dushanbe','Asia/Famagusta','Asia/Gaza','Asia/Hebron','Asia/Ho_Chi_Minh','Asia/Hong_Kong','Asia/Hovd','Asia/Irkutsk','Asia/Jakarta','Asia/Jayapura','Asia/Jerusalem','Asia/Kabul','Asia/Kamchatka','Asia/Karachi','Asia/Kathmandu','Asia/Khandyga','Asia/Kolkata','Asia/Krasnoyarsk','Asia/Kuala_Lumpur','Asia/Kuching','Asia/Kuwait','Asia/Macau','Asia/Magadan','Asia/Makassar','Asia/Manila','Asia/Muscat','Asia/Nicosia','Asia/Novokuznetsk','Asia/Novosibirsk','Asia/Omsk','Asia/Oral','Asia/Phnom_Penh','Asia/Pontianak','Asia/Pyongyang','Asia/Qatar','Asia/Qyzylorda','Asia/Riyadh','Asia/Sakhalin','Asia/Samarkand','Asia/Seoul','Asia/Shanghai','Asia/Singapore','Asia/Srednekolymsk','Asia/Taipei','Asia/Tashkent','Asia/Tbilisi','Asia/Tehran','Asia/Thimphu','Asia/Tokyo','Asia/Tomsk','Asia/Ulaanbaatar','Asia/Urumqi','Asia/Ust-Nera','Asia/Vientiane','Asia/Vladivostok','Asia/Yakutsk','Asia/Yangon','Asia/Yekaterinburg','Asia/Yerevan','Atlantic/Azores','Atlantic/Bermuda','Atlantic/Canary','Atlantic/Cape_Verde','Atlantic/Faroe','Atlantic/Madeira','Atlantic/Reykjavik','Atlantic/South_Georgia','Atlantic/St_Helena','Atlantic/Stanley','Australia/Adelaide','Australia/Brisbane','Australia/Broken_Hill','Australia/Currie','Australia/Darwin','Australia/Eucla','Australia/Hobart','Australia/Lindeman','Australia/Lord_Howe','Australia/Melbourne','Australia/Perth','Australia/Sydney','Europe/Amsterdam','Europe/Andorra','Europe/Astrakhan','Europe/Athens','Europe/Belgrade','Europe/Berlin','Europe/Bratislava','Europe/Brussels','Europe/Bucharest','Europe/Budapest','Europe/Busingen','Europe/Chisinau','Europe/Copenhagen','Europe/Dublin','Europe/Gibraltar','Europe/Guernsey','Europe/Helsinki','Europe/Isle_of_Man','Europe/Istanbul','Europe/Jersey','Europe/Kaliningrad','Europe/Kiev','Europe/Kirov','Europe/Lisbon','Europe/Ljubljana','Europe/London','Europe/Luxembourg','Europe/Madrid','Europe/Malta','Europe/Mariehamn','Europe/Minsk','Europe/Monaco','Europe/Moscow','Europe/Oslo','Europe/Paris','Europe/Podgorica','Europe/Prague','Europe/Riga','Europe/Rome','Europe/Samara','Europe/San_Marino','Europe/Sarajevo','Europe/Saratov','Europe/Simferopol','Europe/Skopje','Europe/Sofia','Europe/Stockholm','Europe/Tallinn','Europe/Tirane','Europe/Ulyanovsk','Europe/Uzhgorod','Europe/Vaduz','Europe/Vatican','Europe/Vienna','Europe/Vilnius','Europe/Volgograd','Europe/Warsaw','Europe/Zagreb','Europe/Zaporozhye','Europe/Zurich','Indian/Antananarivo','Indian/Chagos','Indian/Christmas','Indian/Cocos','Indian/Comoro','Indian/Kerguelen','Indian/Mahe','Indian/Maldives','Indian/Mauritius','Indian/Mayotte','Indian/Reunion','Pacific/Apia','Pacific/Auckland','Pacific/Bougainville','Pacific/Chatham','Pacific/Chuuk','Pacific/Easter','Pacific/Efate','Pacific/Enderbury','Pacific/Fakaofo','Pacific/Fiji','Pacific/Funafuti','Pacific/Galapagos','Pacific/Gambier','Pacific/Guadalcanal','Pacific/Guam','Pacific/Honolulu','Pacific/Kiritimati','Pacific/Kosrae','Pacific/Kwajalein','Pacific/Majuro','Pacific/Marquesas','Pacific/Midway','Pacific/Nauru','Pacific/Niue','Pacific/Norfolk','Pacific/Noumea','Pacific/Pago_Pago','Pacific/Palau','Pacific/Pitcairn','Pacific/Pohnpei','Pacific/Port_Moresby','Pacific/Rarotonga','Pacific/Saipan','Pacific/Tahiti','Pacific/Tarawa','Pacific/Tongatapu','Pacific/Wake','Pacific/Wallis')");
                entity.Property(e => e.UpdatedBy).HasColumnType("char(36)");
                entity.Property(e => e.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Version)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.HasOne(d => d.StudioLocation)
                    .WithMany(p => p.Studio)
                    .HasForeignKey(d => d.StudioPhysicalLocationId)
                    .HasConstraintName("Studio_StudioPhysicalLocation_StudioLocationId_FK");
            });
            
            modelBuilder.Entity<StudioLocation>(entity =>
            {
                entity.ToTable("StudioLocation");

                entity.HasIndex(e => e.BillToCountryId)
                    .HasName("StudioPhysicalLocation_FK1_IDX");

                entity.HasIndex(e => e.CreatedBy)
                    .HasName("StudioPhysicalLocation_FK3_IDX");

                entity.HasIndex(e => e.ShipToCountryId)
                    .HasName("StudioPhysicalLocation_FK2_IDX");

                entity.HasIndex(e => e.UpdatedBy)
                    .HasName("StudioPhysicalLocation_FK4_IDX");

                entity.Property(e => e.StudioLocationId).HasColumnType("int(11)");

                entity.Property(e => e.BillToAddress)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.BillToAddress2)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.BillToCity)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BillToCountry)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BillToCountryId).HasColumnType("int(11)");

                entity.Property(e => e.BillToPostalCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BillToRegion)
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.BillToState)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasColumnType("char(36)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDeleted)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipToAddress)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ShipToCity)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipToCountry)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipToCountryId).HasColumnType("int(11)");

                entity.Property(e => e.ShipToPostalCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipToRegion)
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.ShipToState)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy).HasColumnType("char(36)");

                entity.Property(e => e.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.BillToCountryNavigation)
                    .WithMany(p => p.StudioLocationBillToCountryNavigation)
                    .HasForeignKey(d => d.BillToCountryId)
                    .HasConstraintName("StudioPhysicalLocation_Country_BillToCountryId_FK");
            });
            
            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.HasIndex(e => e.CreatedBy)
                    .HasName("Country_User_CreatedBy_FK");

                entity.HasIndex(e => e.UpdatedBy)
                    .HasName("Country_User_UpdatedBy_FK");

                entity.Property(e => e.CountryId).HasColumnType("int(11)");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.CountryCurrencyCode)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasColumnType("char(36)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.UpdatedBy).HasColumnType("char(36)");

                entity.Property(e => e.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}
