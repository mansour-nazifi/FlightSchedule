using EFCore.BulkExtensions;
using FlightSchedule.Domain.Services;
using FlightSchedule.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace FlightSchedule.Infrastructure.Services
{
    public class BulkService<TEntity> : IBulkService<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;

        public BulkService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            int batchSize = 1_000_000;

            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                await _context.BulkInsertAsync(entities, x =>
                {
                    x.BatchSize = batchSize;
                });

                await transaction.CommitAsync();
            }

            _context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}