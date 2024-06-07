namespace WallWPF;
using Microsoft.AspNetCore.SignalR.Client;

public static class MyHubConnection
{
    public static readonly HubConnection Connection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7005/chat")
        .WithAutomaticReconnect()
        .Build();
    
}