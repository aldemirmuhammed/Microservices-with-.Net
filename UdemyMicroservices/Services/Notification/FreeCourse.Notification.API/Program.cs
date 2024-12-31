using FreeCourse.EventBus.Messages.Common;
using FreeCourse.Notification.API;
using FreeCourse.Notification.API.Consumer;
using FreeCourse.Notification.API.Entities.Email;
using FreeCourse.Notification.API.Helper.Sms;
using FreeCourse.Notification.API.Hubs;
using FreeCourse.Notification.API.Repositories;
using FreeCourse.Notification.API.Repositories.Interfaces;
using FreeCourse.Notification.API.Services;
using FreeCourse.Notification.API.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Polly;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Controller auth
var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
});

//builder.Services.AddControllers(); 

// Add services to the container.
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Identitiy server auth
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];
    options.Audience = "resource_notification";
    options.RequireHttpsMetadata = false;
});



// General Configuration
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository,NotificationRepository>();
builder.Services.AddScoped<NotificationHub>();

builder.Services.AddAutoMapper(typeof(Program));


//builder.Services.Configure<EmailConfiguration>(
//    builder.Configuration.GetSection("EmailConfiguration")
//);
//builder.Services.AddScoped<IEmailSender, EmailSender>();
//builder.Services.Configure<TwilioConfiguration>(builder.Configuration.GetSection("Twilio"));
//builder.Services.AddHttpClient<ITwilioRestClient, TwilioClient>();

#region Email

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<FormOptions>(o => {
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

#endregion

#region SMS

var smsConfig = builder.Configuration.GetSection("SmsCredentials").Get<SmsCredentials>();
builder.Services.AddSingleton(smsConfig);
builder.Services.AddScoped<ISmsService,SmsService>();


#endregion

builder.Services.AddDbContext<NotificationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), configure =>
    {
        configure.MigrationsAssembly("FreeCourse.Notification.API");
    });
});

#region RabbitMq
// MassTransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<CreateOrderNotificationConsumer>();
    config.AddConsumer<NotificationConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        cfg.ReceiveEndpoint(EventBusConstants.CreateOrderQueue, e =>
        {
            e.ConfigureConsumer<CreateOrderNotificationConsumer>(ctx);
        });
        cfg.ReceiveEndpoint(EventBusConstants.NotificationQueue, c =>
        {
            c.ConfigureConsumer<NotificationConsumer>(ctx);
        });
    });
});
builder.Services.AddScoped<CreateOrderNotificationConsumer>();
builder.Services.AddScoped<NotificationConsumer>();
builder.Services.AddMassTransitHostedService();

#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder2 =>
    {
        builder2.WithOrigins("https://localhost:5010", builder.Configuration["WebClient"]).
         AllowAnyHeader().
         AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var notifyDbContext = serviceProvider.GetRequiredService<NotificationDbContext>();
    if (!notifyDbContext.Database.GetAppliedMigrations().SequenceEqual(notifyDbContext.Database.GetMigrations()))
    {
        await notifyDbContext.Database.MigrateAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();

}
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/NotificationHub");
    endpoints.MapControllers();
});


app.MapControllers();


app.Run();
