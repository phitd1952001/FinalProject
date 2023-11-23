using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Backend.DbContext;
using Backend.Services;
using Backend.Services.IServices;
using CloudinaryDotNet;
using Backend.BackgroundServices;
using Backend.Dtos.ChatDtos;
using Backend.Workers;

namespace Backend.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection service)
        {
            //Configure DbContext with Scoped Lifetime
            service.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(AppSettings.ConnectionStrings,
                    sqlOptions => sqlOptions.CommandTimeout(12000));
            });
            return service;
        }
        
        public static IServiceCollection AddService(this IServiceCollection service)
        {
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IJwtUtils, JwtUtils>();
            service.AddScoped<IEmailService, EmailService>();
            service.AddScoped<IImageServices, ImageServices>();
            service.AddScoped<ISubjectService, SubjectService>(); 
            service.AddScoped<IImageServices, ImageServices>();   
            service.AddScoped<IRoomService, RoomService>(); 
            service.AddScoped<IClassService, ClassService>(); 
            service.AddScoped<ISlotService, SlotService>(); 
            service.AddScoped<ICheckInService, CheckInService>();
            service.AddScoped<IDashBoardService, DashboardService>();
            service.AddScoped<ISettingService, SettingService>();
            service.AddScoped<ICalendarService, CalendarService>();
            service.AddScoped<IAutomationGenerateSchedule, AutomationGenerateSchedule>();
            service.AddScoped<IChatService, ChatService>();
            
            service.AddTransient<IReminderService, ReminderService>();

            return service;
        }

        public static IServiceCollection AddBackgroundService(this IServiceCollection service)
        {
            service.AddHostedService<LongRunningService>();
            service.AddSingleton<BackgroundWorkerQueue>();
            return service;
        }
        
        public static IServiceCollection AddChatService(this IServiceCollection service)
        {
            service.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());
            service.AddSingleton<IDictionary<string, string>>(opts => new Dictionary<string, string>());
            return service;
        }

        public static IServiceCollection AddChroneJobService(this IServiceCollection service)
        {
            service.AddHostedService<AutomationMailSender>();
            return service;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection service)
        {
            //auto mapper config
            var mapper = MappingConfig.RegisterMaps().CreateMapper();
            service.AddSingleton(mapper);
            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return service;
        }
        
        public static IServiceCollection AddCloudinary(this IServiceCollection service)
        {
            service.AddSingleton(new Cloudinary(new Account(
                AppSettings.Cloud,
                AppSettings.ApiKey,
                AppSettings.ApiSecretKey)));
            return service;
        }
        
        public static IServiceCollection AddCORS(this IServiceCollection service)
        {
            service.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .WithOrigins(AppSettings.CORS)
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .AllowAnyHeader();
                    });
            });

            return service;
        }
        
        public static IServiceCollection AddSwagger(this IServiceCollection service)
        {
            service.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                        "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

            });
            
            return service;
        }
    }
}