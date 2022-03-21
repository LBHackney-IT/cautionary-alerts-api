using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CautionaryAlertsApi.V1.Infrastructure
{

    public class UhContext : DbContext
    {
        public UhContext(DbContextOptions options) : base(options)
        {
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
    }
}
