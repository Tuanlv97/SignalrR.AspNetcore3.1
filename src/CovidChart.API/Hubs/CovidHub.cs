using CovidChart.API.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CovidChart.API.Hubs
{
        public class CovidHub : Hub
        {
            private CovidService _covidService;

            public CovidHub(CovidService covidService)
            {
                _covidService = covidService;
            }

            public async Task GetCovidList()
            {
                await Clients.All.SendAsync("ReceiveCovidList", _covidService.GetCovidChartList());
            }
        }
}
