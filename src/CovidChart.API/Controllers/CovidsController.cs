using CovidChart.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CovidChart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidsController : ControllerBase
    {
        private CovidService _covidService;

        public CovidsController(CovidService covidService)
        {
            _covidService = covidService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCovid(Covid covid)
        {
            await _covidService.SaveCovid(covid);
            return Ok(_covidService.GetCovidChartList());
        }

        [HttpGet]
        public IActionResult InitializeCovid()
        {
            Random random = new Random();

            Enumerable.Range(1, 10).ToList().ForEach(i =>
            {
                foreach (ECity value in Enum.GetValues(typeof(ECity)))
                {
                    var newCovid = new Covid
                    { City = value, Count = random.Next(100, 1000), CovidDate = DateTime.Now.AddDays(i) };

                    _covidService.SaveCovid(newCovid).Wait();
                    System.Threading.Thread.Sleep(1000);
                }
            });
            return Ok("Covid19 dataları veritabanına kaydedildi");
        }
    }
}
