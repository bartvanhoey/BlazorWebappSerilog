using Microsoft.EntityFrameworkCore;
using Serilog;
using SerLogBlazorWebApp.Components;
using SerLogBlazorWebApp.Data;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connString));

var logConfig = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .WriteTo.Console().WriteTo.Seq("http://localhost:5341")
    .WriteTo.MSSqlServer(connectionString: connString, sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs" }).CreateLogger();


Serilog.Debugging.SelfLog.Enable(Console.Error);

// var logConfig = new LoggerConfiguration().WriteTo.Console().CreateLogger();
Log.Logger = logConfig;
// builder.Host.UseSerilog();
builder.Logging.AddSerilog(logConfig);

Log.Information("Starting the program");


// Important to call at exit so that batched events are flushed.
// Log.CloseAndFlush();

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
