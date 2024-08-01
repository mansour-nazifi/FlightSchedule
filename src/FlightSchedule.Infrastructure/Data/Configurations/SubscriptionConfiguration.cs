using FlightSchedule.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Infrastructure.Data.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => new { s.AgencyId, s.OriginCityId, s.DestinationCityId });
           
            builder.Property(s => s.AgencyId).IsRequired();
            builder.Property(s => s.OriginCityId).IsRequired();
            builder.Property(s => s.DestinationCityId).IsRequired();

            builder.HasIndex(s => s.AgencyId);
        }
    }
}
