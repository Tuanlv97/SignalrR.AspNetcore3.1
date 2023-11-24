using Microsoft.AspNetCore.SignalR;
using SingalR.Server.Models;
using System.Threading.Tasks;

namespace SingalR.Server.Hubs
{
    public class ProductHub : Hub<IProductHub>
    {
        public async Task SendProduct(Product product)
        {
            await Clients.All.ReceiveProduct(product);
        }
    }
}
