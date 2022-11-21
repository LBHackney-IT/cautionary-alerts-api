using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using CautionaryAlertsApi.Services.Interfaces;

namespace CautionaryAlertsApi.V1.Infrastructure
{

    public class UhContext : DbContext
    {
        private readonly IUserResolverService _userService;

        public UhContext(DbContextOptions options, IUserResolverService userService) : base(options)
        {
            _userService = userService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlertDescriptionLookup>()
                .HasKey(lookup => new
                {
                    lookup.AlertCode,
                    lookup.PickType
                });
        }

        public DbSet<PersonAlert> PeopleAlerts { get; set; }

        public DbSet<AlertDescriptionLookup> AlertDescriptionLookups { get; set; }

        public DbSet<ContactLink> ContactLinks { get; set; }

        public DbSet<AddressLink> Addresses { get; set; }

        public DbSet<PropertyAlert> PropertyAlerts { get; set; }

        public DbSet<PropertyAlertNew> PropertyAlertsNew { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            TrackChanges();
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public Func<DateTime> TimestampProvider { get; set; } = () =>
        {
            var utcDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            return DateTime.SpecifyKind(utcDate,
                DateTimeKind.Utc);
        };

        public string UserProvider
        {
            get
            {
                return _userService.GetUserName();
            }
        }

        private void TrackChanges()
        {
            foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (entry.Entity is IAuditable auditable && entry.State == EntityState.Added)
                {
                    auditable.DateModified = auditable.DateModified = TimestampProvider();
                    auditable.ModifiedBy = auditable.ModifiedBy = UserProvider;
                }
            }
        }
    }
}
