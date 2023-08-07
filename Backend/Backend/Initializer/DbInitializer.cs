using Backend.DbContext;
using Backend.Dtos.UserDtos;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Backend.Initializer;

public static class DbInitializer
{
    public static void Initialize(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            var userService =
                serviceScope.ServiceProvider.GetService<IUserService>();
            try
            {
                if (context.Database.GetPendingMigrations().Count() > 0)
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }
            
            if (userService.AdminCheck())
                return;

            // user admin
            var user = new CreateRequest()
            {
                Title = "Mr",
                FirstName = "Admin",
                LastName = "Nguyen",
                Role = Role.Admin,
                Email = "Admin@gmail.com",
                Password = "Admin123@",
                ConfirmPassword = "Admin123@"
            };
            
            userService.Create(user);
        }
    }
}
