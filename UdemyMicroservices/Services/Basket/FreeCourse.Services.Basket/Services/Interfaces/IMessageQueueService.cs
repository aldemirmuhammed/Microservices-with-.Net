using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Services.Interfaces
{
    public interface IMessageQueueService
    {
        Task SendToNotification(string userId, string title, string message);

    }
}
