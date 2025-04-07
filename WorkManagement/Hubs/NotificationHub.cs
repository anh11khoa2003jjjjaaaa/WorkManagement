using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WorkManagement.Hubs;

public class NotificationHub : Hub
{
    private readonly IUserConnectionManager _userConnectionManager;

    public NotificationHub(IUserConnectionManager userConnectionManager)
    {
        _userConnectionManager = userConnectionManager;
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name; // Lấy UserId từ thông tin đăng nhập
        if (userId != null)
        {
            _userConnectionManager.AddConnection(userId, Context.ConnectionId);
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _userConnectionManager.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
