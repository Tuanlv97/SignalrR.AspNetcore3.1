using CovidChart.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CovidChart.API.Models
{
    public class CovidService
    {
        public readonly AppDbContext _context;
        public readonly IHubContext<CovidHub> _hubContext;

        public CovidService(AppDbContext context, IHubContext<CovidHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IQueryable<Covid> GetList()
        {
            return _context.Covids.AsQueryable();
        }

        public async Task SaveCovid(Covid covid)
        {
            await _context.Covids.AddAsync(covid);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveCovidList", GetCovidChartList());
        }
        public List<CovidChart> GetCovidChartList()
        {
            List<CovidChart> charts = new List<CovidChart>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT tarih,[1],[2],[3],[4],[5]\r\nFROM  \r\n    (SELECT [City], [Count], Cast([CovidDate] as date) as tarih from Covids) as covidT    \r\nPIVOT  \r\n(  \r\n    Sum(Count) for City in([1],[2],[3],[4],[5])\r\n) as PTable order by tarih asc";
                command.CommandType = CommandType.Text;
                _context.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChart covidChart = new CovidChart
                        {
                            CovidDate = reader.GetDateTime(0).ToShortDateString()
                        };
                        Enumerable.Range(1, 5).ToList().ForEach(i =>
                        {
                            if (System.DBNull.Value.Equals(reader[i]))
                            {
                                covidChart.Counts.Add(0);
                            }
                            else
                            {
                                covidChart.Counts.Add(reader.GetInt32(i));
                            }

                        });
                        charts.Add(covidChart);
                    }
                }
                _context.Database.CloseConnection();
            }

            return charts;
        }
    }
}
