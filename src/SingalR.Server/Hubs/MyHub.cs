using Microsoft.AspNetCore.SignalR;
using SingalR.Server.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SingalR.Server.Hubs
{
    public class MyHub : Hub
    {
        private AppDbContext _dbContext;

        public MyHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Client her istek yaptığında bu sınıf tekrar oluşacağından static tanımlanmayan tüm veriler sıfırlanacaktır.
        private static List<string> Names { get; set; } = new List<string>();
        private static int ClientCount { get; set; } = 0;
        public static int TeamCount { get; set; } = 7;

        //Tüm metotlar public ve async olmalıdır
        public async Task SendName(string name)
        {
            if (Names.Count >= TeamCount)
            {
                //Caller propu sadece mesajı gönderen client üzerinde işlem yapar. Örneğin sadece o clienta bir mesaj gönderebilir. Hepsine değil.
                await Clients.Caller.SendAsync("Error", $"Takım en fazla {TeamCount} kişi olabilir.");
            }
            else
            {
                //Clients propu tüm clientları temsil ediyor
                //All.SendAsync metodu ise bu huba bağlı tüm clientlarda çalışacak metodu ve metodun parametresini alır ve clientta çalıştırır.
                Names.Add(name);
                await Clients.All.SendAsync("ReceiveName", name);
            }
        }
        public async Task GetNames()
        {
            await Clients.Caller.SendAsync("ReceiveNames", Names);
        }
        //Client bağlandığında çalışan method
        public async override Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnConnectedAsync();
        }
        //Client bağlantıdan çıktığında çalışan method
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnConnectedAsync();
        }

        #region Groups
        public async Task AddToGroup(string teamName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }

        public async Task GetNamesByGroup()
        {
            await Clients.Caller.SendAsync("ReceiveNamesByGroup",
                _dbContext.Teams.Include(team => team.Users)
                    .Select(x => new { teamName = x.Name, users = x.Users.ToList() }));
        }

        public async Task SendNameByGroup(string name, string teamName)
        {
            var team = _dbContext.Teams.Where(x => x.Name == teamName).FirstOrDefault();

            if (team != null)
            {
                team.Users.Add(new User { Name = name });
            }
            else
            {
                var newTeam = new Team { Name = teamName };
                newTeam.Users.Add(new User { Name = name });
                _dbContext.Teams.Add(newTeam);
            }
            await _dbContext.SaveChangesAsync();

            await Clients.Group(teamName).SendAsync("ReceiveMessageByGroup", name, team.Id);
        }

        public async Task RemoveToGroup(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }

        #endregion


   



    }
}
