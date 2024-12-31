using AutoMapper;
using FreeCourse.Notification.API.Entities;
using FreeCourse.Notification.API.Entities.Email;
using FreeCourse.Notification.API.Entities.Notification;
using FreeCourse.Notification.API.Hubs;
using FreeCourse.Notification.API.Services.Interfaces;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using System.Text.Json;
using Twilio.Rest;
using Twilio.TwiML.Messaging;

namespace FreeCourse.Notification.API.Consumer
{
    public class CreateOrderNotificationConsumer : IConsumer<CreateOrderMessageCommand>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CreateOrderMessageCommand> _logger;
        private readonly IHubContext<NotificationHub> _notificationHub;

        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailSender;
        public CreateOrderNotificationConsumer(
            IMapper mapper,
            ILogger<CreateOrderMessageCommand> logger,
            INotificationService notificationService,
            IHubContext<NotificationHub> notificationHub,
            IEmailService emailSender)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationService = notificationService;
            _notificationHub = notificationHub;
            _emailSender = emailSender;
        }

        public async Task Consume(ConsumeContext<CreateOrderMessageCommand> context)
        {
            #region Comment

            //var newAddress = new EmailAddressDto(context.Message.Province, context.Message.District, context.Message.Street, context.Message.ZipCode, context.Message.Line);
            //EmailOrderDto order = new EmailOrderDto(context.Message.BuyerId, newAddress);
            //context.Message.OrderItems.ForEach(x =>
            //{
            //    order.AddOrderItem(x.ProductId, x.ProductName, x.Price, x.PictureUrl);
            //});

            //EmailPaymentDto emailPaymentDto = new EmailPaymentDto(context.Message.CardName,context.Message.CardNumber, context.Message.Expiration, context.Message.CVV, context.Message.TotalPrice, order);

            #endregion

            var createOrderNotificationDto = _mapper.Map<CreateOrderNotificationDto>(context.Message);
            string orderId = "";
            createOrderNotificationDto.OrderItems.ForEach(x => { orderId += x.ProductId.ToString(); });
            var notificationDto = new NotificationDto
            {
                UserId = createOrderNotificationDto.BuyerId,
                Title = $"New Order",
                Message = $"Course order create successfully as order number id {orderId}",
                TargetSource = TargetSource.Course,
                ExpiryAt = DateTime.Now.AddDays(10),
                DisplayType = DisplayType.INBOX,
                Severity = Severity.Information,
                Status = "NEW"
            };

            #region SendEmail

            await SendEmail(createOrderNotificationDto);

            #endregion

            #region SaveDatabase

            await _notificationService.Save(notificationDto);

            #endregion

            #region SentSignalR

            await _notificationHub.Clients.All.SendAsync("ReceiveNotificationAll", JsonSerializer.Serialize(notificationDto)).ConfigureAwait(false);

            #endregion

            _logger.LogInformation("NotificationConsumer consumed successfully.Notification Id  : {notificitaionDto.Id}", notificationDto.Id);

        }

        public async Task SendEmail(CreateOrderNotificationDto createOrderNotificationDto)
        {
            try
            {
                var html = GenerateHtml(createOrderNotificationDto);

                var message = new EmailMessage(new string[] { createOrderNotificationDto.BuyerEmail },
                    "New Order",
                    html);

                await _emailSender.SendEmailAsync(message);
            }
            catch (Exception ex)
            { throw ex; }
        }

        public string GenerateHtml(CreateOrderNotificationDto createOrderNotificationDto)
        {

            try
            {
                string html = string.Format("<h2 style='color:red;'>Order created successfully</h2>");
                string path = Directory.GetCurrentDirectory();
                var htmlHead = System.IO.File.ReadAllText(Path.Combine(path, "Template/EmailCreateOrderTemplate/indexHead.html"));
                //var htmlBody = System.IO.File.ReadAllText(@"../Template/EmailCreateOrderTemplate/indexBody.html");
                var htmlfooter = System.IO.File.ReadAllText(Path.Combine(path, "Template/EmailCreateOrderTemplate/indexFooter.html"));

                foreach (var item in createOrderNotificationDto.GetType().GetProperties())
                {
                    string propertyName = item.Name;
                    string key = "{{" + $"{propertyName}" + "}}";
                    string value = createOrderNotificationDto.GetType().GetProperty(propertyName).GetValue(createOrderNotificationDto, null).ToString();
                    if (htmlHead.Contains(key))
                        htmlHead = htmlHead.Replace(key, value);

                    //if (htmlBody.Contains("{{" + $"{propertyName}" + "}}"))
                    //    htmlBody = htmlBody.Replace("{{" + $"{propertyName}" + "}}", item?.GetValue(item).ToString());

                    if (htmlfooter.Contains(key))
                        htmlfooter = htmlfooter.Replace(key, value);
                }
                string htmlOrder = "<tr><td style=\"padding-top: 0;\">" +
                        "<table width=\"560\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"devicewidthinner\" style=\"border-bottom: 1px solid #eeeeee;\">" +
                        "<tbody>";
                foreach (var item in createOrderNotificationDto.OrderItems)
                {
                    htmlOrder += "<tr><td rowspan=\"4\" style=\"padding-right: 10px; padding-bottom: 10px;\">" +
                        "<img style=\"height: 80px;\" src=\"" + item.PictureUrl + "\" alt=\"Product Image\" /></td>" +
                        "<td colspan=\"2\" style=\"font-size: 14px; font-weight: bold; color: #666666; padding-bottom: 5px;\">" +
                        $"{item.ProductName}" +
                        "</td></tr><tr><td style=\"font-size: 14px; line-height: 18px; color: #757575; width: 440px;\">" +
                        "Product Id:" + $"{item.ProductId}" +
                        "</td><td style=\"width: 130px;\">" +
                        "</td></tr><tr><td style=\"font-size: 14px; line-height: 18px; color: #757575;\">" +
                        "Price:" + $"{item.Price}" +
                        "</td><td style=\"font-size: 14px; line-height: 18px; color: #757575; text-align: right;\">" +
                        $"${item.Price} Per Unit" +
                        "</td></tr><tr><td style=\"font-size: 14px; line-height: 18px; color: #757575; padding-bottom: 10px;\">" +
                        "</td><td style=\"font-size: 14px; line-height: 18px; color: #757575; text-align: right; padding-bottom: 10px;\">" +
                        "<b style=\"color: #666666;\">" +
                        $"${createOrderNotificationDto.TotalPrice}</b>" + " Total" +
                        "</td></tr>";


                }
                htmlOrder += "</tbody></table></td></tr>";

                html = htmlHead + htmlOrder + htmlfooter;

                return html;
            }
            catch (Exception ex)
            {

                return String.Empty;
            }

        }
    }
}
