using Coordinator.Contexts;
using Coordinator.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("OrderAPI", client => client.BaseAddress = new("https://localhost:7237/"));
builder.Services.AddHttpClient("StockAPI", client => client.BaseAddress = new("https://localhost:7200/"));
builder.Services.AddHttpClient("PaymentAPI", client => client.BaseAddress = new("https://localhost:7096/"));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    //Phase 1
    Guid transactionId = await transactionService.CreateTransactionAsync();
    await transactionService.PrepareServicesAsync(transactionId);
    bool result = await transactionService.CheckReadyServices(transactionId);
    if (result)
    { 
        //Phase 2
        await transactionService.Commit(transactionId);
        result = await transactionService.CheckTransactionStateServices(transactionId);
    }
    if (!result)
        await transactionService.Rollback(transactionId);
    
});



app.Run();
