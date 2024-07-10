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


            var adminRole = Environment.GetEnvironmentVariable("Role") ?? _appSettings.MovieProSettings.DefaultCredentials.Role;

            //var adminRole = _appSettings.MovieProSettings.DefaultCredentials.Role;
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        private async Task SeedUsersAsync()
        {
            if (_userManager.Users.Any()) return;
            var email = Environment.GetEnvironmentVariable("Email") ?? _appSettings.MovieProSettings.DefaultCredentials.Email;
            var userName = Environment.GetEnvironmentVariable("Email") ?? _appSettings.MovieProSettings.DefaultCredentials.Email;
            var password = Environment.GetEnvironmentVariable("Password") ?? _appSettings.MovieProSettings.DefaultCredentials.Password;

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

            var name = Environment.GetEnvironmentVariable("Name") ?? _appSettings.MovieProSettings.DefaultCollection.Name;
            var description = Environment.GetEnvironmentVariable("Description") ?? _appSettings.MovieProSettings.DefaultCollection.Description;

            _dbContext.Add(new Collection()
            {
                Name = name,
                Description = description
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
