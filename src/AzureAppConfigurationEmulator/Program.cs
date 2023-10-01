var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", (HttpContext _) => Results.Text("Hello World"));

app.Run();
