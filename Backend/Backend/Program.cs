using Backend;
using Backend.Extensions;
using Backend.Middleware;

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
    .AddSwagger();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapControllers();

app.Run();