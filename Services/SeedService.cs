﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoviePro.Data;
using MoviePro.Models.Database;
using MoviePro.Models.Settings;
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

            var adminRole = _appSettings.MovieProSettings.DefaultCredentials.Role;
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        private async Task SeedUsersAsync()
        {
            if (_userManager.Users.Any()) return;

            var credentials = _appSettings.MovieProSettings.DefaultCredentials;
            var newUser = new IdentityUser()
            {
                Email = credentials.Email,
                UserName = credentials.Email,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(newUser, credentials.Password);
            //await _userManager.AddToRoleAsync(newUser, credentials.Role);

        }

        private async Task SeedCollections()
        {
            if (_dbContext.Collection.Any()) return;

            _dbContext.Add(new Collection()
            {
                Name = _appSettings.MovieProSettings.DefaultCollection.Name,
                Description = _appSettings.MovieProSettings.DefaultCollection.Description
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
