var builder = WebApplication.CreateBuilder(args);



var app = builder.Build();


app.MapGet("/ready", () =>
{
    Console.WriteLine("Order API is Ready");
    return false;
});
app.MapGet("/commit", () =>
{
    Console.WriteLine("Order API is comitted");
    return true;
});
app.MapGet("/rollback", () =>
{
    Console.WriteLine("Order API is rollbacked");
    
});

app.Run();
