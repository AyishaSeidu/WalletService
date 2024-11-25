using WalletService.Api.Application.Mapping;

using Microsoft.EntityFrameworkCore;
using WalletService.Infrastructure.DataContext;
using WalletService.Api.Application.Validation;
using WalletService.Infrastructure.Repository.Interfaces;
using WalletService.Infrastructure.Repository;
using WalletService.Api.Controllers.Filters;
using WalletService.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var writeConnectionString = builder.Configuration.GetConnectionString("WalletServiceDbWriteConnectionString");
ArgumentException.ThrowIfNullOrWhiteSpace(writeConnectionString, nameof(writeConnectionString));
builder.Services.AddDbContext<IWalletServiceContext, WalletServiceContext>(options =>
{
    options.UseSqlServer(writeConnectionString);
});

builder.Services.AddScoped<IWalletValidator, WalletValidator>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddSingleton<ExceptionFilter<WalletController>>();


builder.Services.AddAutoMapper(typeof(WalletReadDtoProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


// Run migrations before running app
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<WalletServiceContext>();
    context.Database.Migrate();
}

app.Run();
