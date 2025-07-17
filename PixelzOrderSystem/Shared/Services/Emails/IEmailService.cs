using PixelzOrderSystem.Shared.Domain.Entities;

namespace PixelzOrderSystem.Shared.Services.Emails;

public interface IEmailService
{
    Task SendAsync(string email, string subject, string content);
}