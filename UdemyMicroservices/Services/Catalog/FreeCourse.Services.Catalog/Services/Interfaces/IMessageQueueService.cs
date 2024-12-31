using FreeCourse.EventBus.Messages.Events;
using System.Threading.Tasks;
namespace FreeCourse.Services.Catalog.Services.Interfaces
{
    public interface IMessageQueueService
    {

        Task SendToNotification(string userId, string title, string message); 
        Task SendToBasket(string userId, string courseId, string updatedName); 
    }
}
