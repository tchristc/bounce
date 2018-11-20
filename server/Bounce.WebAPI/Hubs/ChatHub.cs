using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Bounce.WebAPI.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send(string user, string message)
        {
            //"ReceiveMessage", 
            await Clients.All.addNewMessageToPage(user, message);
        }
    }
}