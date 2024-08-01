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

            //var tb = new DataTable(typeof(TEntity).Name);

            //PropertyInfo[] props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //foreach (var prop in props)
            //{
            //    tb.Columns.Add(prop.Name, prop.PropertyType);
            //}

            //foreach (var item in entities)
            //{
            //    var values = new object[props.Length];
            //    for (var i = 0; i < props.Length; i++)
            //    {
            //        values[i] = props[i].GetValue(item, null);
            //    }

            //    tb.Rows.Add(values);
            //}


            //string connection = "Server=.;Database=FlightSchedule.Temp;User ID=sa;Password=123123;MultipleActiveResultSets=true;TrustServerCertificate=True";


        }

        public async Task BulkInsertAsync1(IEnumerable<TEntity> entities)
        {
            var objBulk = new BulkUploadToSql<TEntity>()
            {
                InternalStore = entities.ToList(),
                TableName = "Flights",
                CommitBatchSize = 1000,
                ConnectionString = "Server=.;Database=FlightSchedule.Temp;User ID=sa;Password=123123;MultipleActiveResultSets=true;TrustServerCertificate=True"
            };
            objBulk.Commit();
        }


        public class BulkUploadToSql<T>
        {
            public IList<T> InternalStore { get; set; }
            public string TableName { get; set; }
            public int CommitBatchSize { get; set; } = 1000;
            public string ConnectionString { get; set; }

            public void Commit()
            {
                if (InternalStore.Count > 0)
                {
                    DataTable dt;
                    int numberOfPages = (InternalStore.Count / CommitBatchSize) + (InternalStore.Count % CommitBatchSize == 0 ? 0 : 1);
                    for (int pageIndex = 0; pageIndex < numberOfPages; pageIndex++)
                    {
                        dt = InternalStore.Skip(pageIndex * CommitBatchSize).Take(CommitBatchSize).ToDataTable();
                        BulkInsert(dt);
                    }
                }
            }

            public void BulkInsert(DataTable dt)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    // make sure to enable triggers
                    // more on triggers in next post
                    SqlBulkCopy bulkCopy =
                        new SqlBulkCopy
                        (
                        connection,
                        Microsoft.Data.SqlClient.SqlBulkCopyOptions.TableLock |
                        Microsoft.Data.SqlClient.SqlBulkCopyOptions.FireTriggers |
                        Microsoft.Data.SqlClient.SqlBulkCopyOptions.UseInternalTransaction,
                        null
                        );

                    // set the destination table name
                    bulkCopy.DestinationTableName = TableName;
                    bulkCopy.ColumnMappings.Add("Id", "Id");
                    bulkCopy.ColumnMappings.Add("RouteId", "RouteId");
                    bulkCopy.ColumnMappings.Add("DepartureTime", "DepartureTime");
                    bulkCopy.ColumnMappings.Add("ArrivalTime", "ArrivalTime");
                    bulkCopy.ColumnMappings.Add("AirlineId", "AirlineId");
                    connection.Open();

                    // write the data in the "dataTable"
                    bulkCopy.WriteToServer(dt);
                    connection.Close();
                }
                // reset
                //this.dataTable.Clear();
            }

        }

       
    }

    public static class BulkUploadToSqlHelper
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            table.Columns.Remove("Route");
            return table;
        }
    }
}