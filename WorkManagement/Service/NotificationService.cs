using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WorkManagement.Data;
using WorkManagement.Hubs;
using WorkManagement.Models;

public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IUserConnectionManager _connectionManager;
    private readonly WorkManagementContext _context;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        IUserConnectionManager connectionManager,
        WorkManagementContext context)
    {
        _hubContext = hubContext;
        _connectionManager = connectionManager;
        _context = context;
    }

    /// <summary>
    /// Gửi thông báo đến cá nhân hoặc toàn bộ nhân viên.
    /// </summary>
    /// <param name="userId">ID của người nhận, để null nếu gửi toàn bộ</param>
    /// <param name="title">Tiêu đề thông báo</param>
    /// <param name="content">Nội dung thông báo</param>
    /// <param name="isGlobal">Xác định đây là thông báo toàn bộ</param>
    public async Task SendNotificationAsync(int? userId, string title, string content, bool isGlobal = false)
    {
        if (isGlobal)
        {
            await SendGlobalNotificationAsync(title, content);
        }
        else if (userId.HasValue)
        {
            await SendPersonalNotificationAsync(userId.Value, title, content);
        }
    }

    /// <summary>
    /// Gửi thông báo đến toàn bộ nhân viên.
    /// </summary>
    private async Task SendGlobalNotificationAsync(string title, string content)
    {
        // Lấy toàn bộ danh sách nhân viên
        var users = await _context.Users.ToListAsync();

        // Tạo danh sách thông báo
        var notifications = users.Select(user => new Notification
        {
            UserId = user.Id,
            Title = title,
            Content = content,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            IsGlobal = true
        }).ToList();

        // Lưu thông báo vào cơ sở dữ liệu
        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();

        // Gửi thông báo qua SignalR
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", title, content);
    }

    /// <summary>
    /// Gửi thông báo đến một nhân viên cụ thể.
    /// </summary>
    private async Task SendPersonalNotificationAsync(int userId, string title, string content)
    {
        // Tạo thông báo
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Content = content,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            IsGlobal = false
        };

        // Lưu thông báo vào cơ sở dữ liệu
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        // Gửi thông báo qua SignalR cho nhân viên cụ thể
        var connections = _connectionManager.GetConnections(userId.ToString());
        foreach (var connectionId in connections)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", title, content);
        }
    }

    /// <summary>
    /// Lấy danh sách thông báo của người dùng.
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <returns>Danh sách thông báo</returns>
    public async Task<IActionResult> GetNotificationsAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return new OkObjectResult(notifications);
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null)
        {
            return false; // Không tìm thấy thông báo
        }

        notification.IsRead = true; // Đánh dấu là đã đọc
        _context.Notifications.Update(notification);

        await _context.SaveChangesAsync();
        return true;
    }

}
