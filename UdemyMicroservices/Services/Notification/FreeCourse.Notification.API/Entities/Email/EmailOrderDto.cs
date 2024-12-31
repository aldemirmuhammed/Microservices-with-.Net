
namespace FreeCourse.Notification.API.Entities.Email
{
    public class EmailOrderDto
    {

        public DateTime CreatedDate { get; private set; }

        public EmailAddressDto Address { get; private set; }

        public string BuyerId { get; private set; }

        private readonly List<EmailOrderItemDto> _orderItems;

        public IReadOnlyCollection<EmailOrderItemDto> OrderItems => _orderItems;

        public EmailOrderDto() { }

        public EmailOrderDto(string buyerId, EmailAddressDto address)
        {
            _orderItems = new List<EmailOrderItemDto>();
            CreatedDate = DateTime.Now;
            Address = address;
            BuyerId = buyerId;
        }


        public void AddOrderItem(string productId, string productName, decimal price, string pictureUrl)
        {
            var existProduct = _orderItems.Any(x => x.ProductId == productId);


            if (!existProduct)
            {
                var newOrderItem = new EmailOrderItemDto(productId, productName, pictureUrl, price);
                _orderItems.Add(newOrderItem);
            }
        }

        public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);

    }
}
