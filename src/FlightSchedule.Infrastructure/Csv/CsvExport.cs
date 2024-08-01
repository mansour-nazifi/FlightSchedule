using CsvHelper;
using FlightSchedule.Domain.Dto;
using FlightSchedule.Domain.Entities;
using FlightSchedule.Domain.Services;
using System.Globalization;
using System.Threading.Channels;
namespace FlightSchedule.Infrastructure.Csv
{
    public class CsvExport
    {        
        public async Task<string> Export<Header>(IEnumerable<Header> entities)
        {
            var path = $"{DateTime.Now.Ticks}.csv";
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<Header>();
                await csv.NextRecordAsync();

                foreach (var change in entities)
                {
                    csv.WriteRecord(change);
                    await csv.NextRecordAsync();
                }
            }

            
            return new System.IO.FileInfo(path).FullName;
        }         
    }
}