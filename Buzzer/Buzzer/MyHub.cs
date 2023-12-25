using Microsoft.AspNetCore.SignalR;

namespace Buzzer.Hubs;

public class MyHub : Hub
{
    public async Task UpdateGame()
    {
        await Clients.All.SendAsync("UpdateData", BuzzData.LoadOrCreate().Serialize());
    }

    public async Task Buzz(string name, string dateTimeEncoded)
    {
        BuzzData data = BuzzData.LoadOrCreate();
        data.Add(name, long.Parse(dateTimeEncoded));
        data.Save();
        await Clients.All.SendAsync("UpdateData", data.Serialize());
    }

    public async Task Clear()
    {
        BuzzData data = BuzzData.LoadOrCreate();
        data.Buzzes = new();
        data.Save();
        await Clients.All.SendAsync("UpdateData", data.Serialize());
    }

    public async Task JoinAsHost(string name)
    {
        BuzzData data = BuzzData.LoadOrCreate();
        if (data.HostConnectionID == "")
        {
            data.HostConnectionID = Context.ConnectionId;
            data.Save();
            await Clients.All.SendAsync("UpdateData", data.Serialize());
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        BuzzData data = BuzzData.LoadOrCreate();
        if (Context.ConnectionId == data.HostConnectionID)
        {
            data.HostConnectionID = "";
            data.Save();
            await Clients.All.SendAsync("UpdateData", data.Serialize());
        }
        await base.OnDisconnectedAsync(exception);
    }
}