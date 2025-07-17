
namespace PixelzOrderSystem.Shared.Services.Emails;

public class MockEmailService(ILogger<MockEmailService> logger): IEmailService
{
    public async Task SendAsync(string email, string subject, string content)
    {
        // Simulate sending an email
        logger.LogInformation("Sending email to {Email} with subject '{Subject}' and content '{Content}'",
            email, subject, content);

        // Simulate a delay to mimic real email sending
        await Task.Delay(1000);
    }
}