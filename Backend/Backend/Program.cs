using Backend;
using Backend.Extensions;
using Backend.Initializer;
using Backend.Middleware;
using Backend.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Configure the application's configuration settings
builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
builder.Configuration.AddEnvironmentVariables();
// Map AppSettings section in appsettings.json file value to AppSetting model
builder.Configuration.GetSection("AppSettings").Get<AppSettings>(options => options.BindNonPublicProperties = true);
// Add services to the container.

builder.Services.AddControllers();
builder.Services
    .AddDatabase()
    .AddAutoMapper()
    .AddService()
    .AddBackgroundService()
    .AddChroneJobService()
    .AddCloudinary()
    .AddChatService()
    .AddEndpointsApiExplorer()
    .AddSwagger()
    .AddCORS()
    .AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend v1"));
}

app.UseRouting();

app.UseCors("AllowAll");

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();;

app.MapHub<ChatHub>("/chat");

app.MapControllers();

DbInitializer.Initialize(app);

app.Run();