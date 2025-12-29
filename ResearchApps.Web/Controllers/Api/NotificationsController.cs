using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Service.Interface;

namespace ResearchApps.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Get notifications for the current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int take = 20, CancellationToken cancellationToken = default)
    {
        var response = await _notificationService.GetNotifications(take, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Message);
    }

    /// <summary>
    /// Get unread notifications for the current user
    /// </summary>
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications(CancellationToken cancellationToken = default)
    {
        var response = await _notificationService.GetUnreadNotifications(cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Message);
    }

    /// <summary>
    /// Get notification count for the current user
    /// </summary>
    [HttpGet("count")]
    public async Task<IActionResult> GetNotificationCount(CancellationToken cancellationToken = default)
    {
        var response = await _notificationService.GetNotificationCount(cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }
        return BadRequest(response.Message);
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken = default)
    {
        var response = await _notificationService.MarkAsRead(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(new { message = response.Message });
        }
        return BadRequest(response.Message);
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        var response = await _notificationService.MarkAllAsRead(cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(new { message = response.Message });
        }
        return BadRequest(response.Message);
    }

    /// <summary>
    /// Delete a notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(int id, CancellationToken cancellationToken = default)
    {
        var response = await _notificationService.DeleteNotification(id, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(new { message = response.Message });
        }
        return BadRequest(response.Message);
    }
}

