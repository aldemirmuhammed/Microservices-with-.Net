using FreeCourse.EventBus.Messages.Common;
using FreeCourse.Services.Order.Application.Consumers;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Services;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using static MassTransit.MessageHeaders;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];
    options.Audience = "resource_order";
    options.RequireHttpsMetadata = false;
});

#region Database

// Sql Server Database
builder.Services.AddDbContext<OrderDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), configure =>
    {
        configure.MigrationsAssembly("FreeCourse.Services.Order.Infrastructure");
    });
});

#endregion

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();

builder.Services.AddMediatR(typeof(FreeCourse.Services.Order.Application.Handlers.CreateOrderCommandHandler).Assembly);

#region RabbitMq

// Message queue
builder.Services.AddMassTransit(x =>
{

    x.AddConsumer<CreateOrderMessageCommandConsumer>();
    x.AddConsumer<CourseNameChangedEventConsumer>();
    // Default Port : 5672
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        cfg.ReceiveEndpoint(EventBusConstants.CreateOrderQueue, e =>
        {
            e.ConfigureConsumer<CreateOrderMessageCommandConsumer>(context);
        });
        cfg.ReceiveEndpoint(EventBusConstants.CourseNameChangedEventOrder, e =>
        {
            e.ConfigureConsumer<CourseNameChangedEventConsumer>(context);
        });
    });
});

#endregion



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var orderDbContext = serviceProvider.GetRequiredService<OrderDbContext>();
    if (!orderDbContext.Database.GetAppliedMigrations().SequenceEqual(orderDbContext.Database.GetMigrations()))
    {
        await orderDbContext.Database.MigrateAsync();
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();












