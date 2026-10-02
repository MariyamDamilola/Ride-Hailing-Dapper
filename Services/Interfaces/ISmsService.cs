namespace RideHailingApi_Dapper.Services.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(string recipientNumber, string message);

}