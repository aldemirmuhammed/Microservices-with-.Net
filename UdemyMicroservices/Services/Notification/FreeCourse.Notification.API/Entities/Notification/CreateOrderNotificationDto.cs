using FreeCourse.Shared.Messages;
using System.Text.Json.Serialization;

namespace FreeCourse.Notification.API.Entities.Notification
{
    public class CreateOrderNotificationDto
    {
        public string BuyerEmail { get; set; }
        public string BuyerId { get; set; }

        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Line { get; set; }

        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public decimal TotalPrice { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public CreateOrderNotificationDto()
        {
            OrderItems = new List<OrderItem>();
        }

    }

    //public class OrderItem
    //{
    //    public string ProductId { get; set; }
    //    public string ProductName { get; set; }
    //    public string PictureUrl { get; set; }
    //    public Decimal Price { get; set; }
    //}
}
