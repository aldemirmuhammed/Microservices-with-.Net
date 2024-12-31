using FreeCourse.EventBus.Messages.Events;
using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Services.Basket.Services.Interfaces;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Services
{
    public class BasketService : IBasketService
    {
        private readonly RedisService _redisService;
        private readonly IMessageQueueService _messageQueueService;

        public BasketService(RedisService redisService, IMessageQueueService messageQueueService)
        {
            _redisService = redisService;
            _messageQueueService = messageQueueService;
        }

        public async Task<Response<bool>> Delete(string userId)
        {
            var status = await _redisService.GetDb().KeyDeleteAsync(userId);
            return status ? Response<bool>.Success(204) : Response<bool>.Fail("Basket could not found", 404);

        }

        public async Task<Response<BasketDto>> GetBasket(string userId)
        {
            var existBasket = await _redisService.GetDb().StringGetAsync(userId);

            if (string.IsNullOrEmpty(existBasket))
            {
                return Response<BasketDto>.Fail("Basket not found", 404);
            }
            return Response<BasketDto>.Success(JsonSerializer.Deserialize<BasketDto>(existBasket), 200);
        }

        public async Task<Response<bool>> SaveOrUpdate(BasketDto basketDto)
        {

            var status = await _redisService.GetDb().StringSetAsync(basketDto.UserId, JsonSerializer.Serialize(basketDto));

            //await _messageQueueService.SendToNotification(basketDto.UserId, "Basket updated or created successfully", "Basket updated or created operation successfully completed");

            return status ? Response<bool>.Success(204) : Response<bool>.Fail("Basket could not update or save", 500);
        }
    }
}
