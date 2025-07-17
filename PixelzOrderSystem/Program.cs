using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PixelzOrderSystem.Shared.Background;
using PixelzOrderSystem.Shared.Common;
using PixelzOrderSystem.Shared.Database;
using PixelzOrderSystem.Shared.Repositories.Customers;
using PixelzOrderSystem.Shared.Repositories.OrderProcessing;
using PixelzOrderSystem.Shared.Repositories.Orders;
using PixelzOrderSystem.Shared.Services.Emails;
using PixelzOrderSystem.Shared.Services.Invoices;
using PixelzOrderSystem.Shared.Services.Payments;
using PixelzOrderSystem.Shared.Services.Production;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
var configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddFastEndpoints(options =>
{
    options.IncludeAbstractValidators = true;
})
.SwaggerDocument(o => o.DocumentSettings = s =>
{
    s.Title = "Pixelz Order System API";
    s.Version = "v1.0";
});

builder.Services.AddHttpContextAccessor();

// Đăng ký các repository
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderProcessingRepository, OrderProcessingRepository>();

// Đăng ký các service mock để sử dụng trong quá trình phát triển
builder.Services.AddScoped<IPaymentService, MockPaymentService>();
builder.Services.AddScoped<IProductionService, MockProductionService>();
builder.Services.AddScoped<IEmailService, MockEmailService>();
builder.Services.AddScoped<IInvoiceService, MockInvoiceService>();

// Đăng ký job để xử lý các domain event, retry các event không thành công
builder.Services.AddHostedService<DomainEventProcessor>();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseFastEndpoints(o =>
{
    o.Endpoints.RoutePrefix = "api";
}).UseSwaggerGen();

app.UseHttpsRedirection();

// Seeding sample data if the database is empty
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedSampleDataAsync(dbContext);
}

app.Run();