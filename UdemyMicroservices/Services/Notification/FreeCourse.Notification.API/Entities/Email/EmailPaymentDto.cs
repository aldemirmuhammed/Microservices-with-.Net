namespace FreeCourse.Notification.API.Entities.Email
{
    public class EmailPaymentDto
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public decimal TotalPrice { get; set; }

        public EmailOrderDto Order { get; set; }

        public EmailPaymentDto(string cardName, string cardNumber, string expiration, string cVV, decimal totalPrice, EmailOrderDto order)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            Expiration = expiration;
            CVV = cVV;
            TotalPrice = totalPrice;
            Order = order;
        }
    }
}
