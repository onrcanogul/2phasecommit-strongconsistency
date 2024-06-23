var builder = WebApplication.CreateBuilder(args);



var app = builder.Build();


app.MapGet("/ready", () =>
{
    Console.WriteLine("Payment API is Ready");
    return true;
});
app.MapGet("/commit", () =>
{
    Console.WriteLine("Payment API is committed");
    return true;
});
app.MapGet("/rollback", () =>
{
    Console.WriteLine("Payment API is rollbacked");

});

app.Run();
