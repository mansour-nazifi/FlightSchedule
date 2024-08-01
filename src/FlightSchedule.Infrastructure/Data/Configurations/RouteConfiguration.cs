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
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(f => f.Id).ValueGeneratedNever();

            builder.Property(r => r.OriginCityId).IsRequired();
            builder.Property(r => r.DestinationCityId).IsRequired();
            builder.Property(r => r.DepartureDate).IsRequired();

            builder.HasMany(x => x.Flights).WithOne(x => x.Route).HasForeignKey(x => x.RouteId);

            builder.HasIndex(r => new { r.OriginCityId, r.DestinationCityId, r.DepartureDate });             
        }
    }
}
