using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace UrunSatisPortali.Hubs
{
    public class GeneralHub : Hub
    {
        // Bu metodun içi boş olabilir, tetiklemeyi Controller yapar.
        // Ancak tünelin açık kalması için sınıfın varlığı şarttır.
        public async Task SendUpdate(int count, string sales)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", count, sales);
        }
    }
}