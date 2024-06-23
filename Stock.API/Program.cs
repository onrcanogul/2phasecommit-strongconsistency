var builder = WebApplication.CreateBuilder(args);



var app = builder.Build();


app.MapGet("/ready", () =>
{
    Console.WriteLine("Stock API is Ready");
    return true;
});
app.MapGet("/commit", () =>
{
    Console.WriteLine("Stock API is comitted");
    return true;
});
app.MapGet("/rollback", () =>
{
    Console.WriteLine("Stock API is rollbacked");

});

app.Run();
