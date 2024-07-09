using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoviePro.Data;
using MoviePro.Models.Database;
using MoviePro.Models.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoviePro.Services
{
    public class SeedService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedService(IOptions<AppSettings> appSettings,
                           ApplicationDbContext dbContext,
                           UserManager<IdentityUser> userManager,
                           RoleManager<IdentityRole> roleManager)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync();
            //await SeedRolesAsync();
            //await SeedUsersAsync();
            //await SeedCollections();
        }

        private async Task SeedRolesAsync()
        {
            if (_dbContext.Roles.Any()) return;


            var adminRole = _appSettings.MovieProSettings.DefaultCredentials.Role ?? Environment.GetEnvironmentVariable("Role");

            //var adminRole = _appSettings.MovieProSettings.DefaultCredentials.Role;
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        private async Task SeedUsersAsync()
        {
            if (_userManager.Users.Any()) return;
            var email = _appSettings.MovieProSettings.DefaultCredentials.Email ?? Environment.GetEnvironmentVariable("Email");
            var userName = _appSettings.MovieProSettings.DefaultCredentials.Email ?? Environment.GetEnvironmentVariable("Email");
            var password = _appSettings.MovieProSettings.DefaultCredentials.Password ?? Environment.GetEnvironmentVariable("Password");

            //var credentials = _appSettings.MovieProSettings.DefaultCredentials;
            var newUser = new IdentityUser()
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(newUser, password);
            //await _userManager.AddToRoleAsync(newUser, credentials.Role);

        }

        private async Task SeedCollections()
        {
            if (_dbContext.Collection.Any()) return;

            var name = _appSettings.MovieProSettings.DefaultCollection.Name ?? Environment.GetEnvironmentVariable("Name");
            var description = _appSettings.MovieProSettings.DefaultCollection.Description ?? Environment.GetEnvironmentVariable("Description");

            _dbContext.Add(new Collection()
            {
                Name = name,
                Description = description
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
