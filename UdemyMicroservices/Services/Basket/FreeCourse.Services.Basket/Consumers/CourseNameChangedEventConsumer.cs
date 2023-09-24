using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Shared.Messages;
using FreeCourse.Shared.Services;
using MassTransit;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Consumers
{
    public class CourseNameChangedEventConsumer : IConsumer<CourseNameChangedEvent>
    {
        private readonly IBasketService _basketService;

        public CourseNameChangedEventConsumer(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task Consume(ConsumeContext<CourseNameChangedEvent> context)
        {
          
            var baskets = await _basketService.GetBasket(context.Message.UserId);
            if (baskets == null)
            {
                return;
            }

            baskets.Data.basketItems.Where(x => x.CourseId == context.Message.CourseId).ToList();
            baskets.Data.basketItems.ForEach(x =>
            {
                x.CourseName = context.Message.UpdatedName;
            });

            await _basketService.SaveOrUpdate(baskets.Data);
        }
    }
}
