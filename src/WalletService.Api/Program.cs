using WalletService.Api.Application.Mapping;

using Microsoft.EntityFrameworkCore;
using WalletService.Infrastructure.DataContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var writeConnectionString = builder.Configuration.GetConnectionString("WalletServiceDbWriteConnectionString");
ArgumentException.ThrowIfNullOrWhiteSpace(writeConnectionString, nameof(writeConnectionString));
builder.Services.AddDbContext<WalletServiceContext>(options =>
{
    options.UseSqlServer(writeConnectionString);
});


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
