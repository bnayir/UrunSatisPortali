using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace UrunSatisPortali.Hubs
{
    public class GeneralHub : Hub
    {
        public async Task SendUpdate(int count, string sales)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", count, sales);
        }
    }
}