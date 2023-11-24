using SingalR.Server.Models;
using System.Threading.Tasks;

namespace SingalR.Server.Hubs
{
    public interface IProductHub
    {
        Task ReceiveProduct(Product product);
        Task ReceiveProduct2(Product product);
        Task ReceiveProduct3(Product product);
    }
}
