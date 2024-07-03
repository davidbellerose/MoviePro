using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviePro.Data;
using MoviePro.Models;
using MoviePro.Models.ViewModels;
using MoviePro.Services;
using MoviePro.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MoviePro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRemoteMovieService _movieService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IRemoteMovieService movieService)
        {
            _logger = logger;
            _context = context;
            _movieService = movieService;
        }

        public async Task<IActionResult> Index()
        {
            const int count = 18;

            var data = new LandingPageVM()
            {
                CustomCollections = await _context.Collection
                                .Include(c => c.MovieCollections)
                                .ThenInclude(mc => mc.Movie)
                                .ToListAsync(),
                Upcoming = await _movieService.SearchMoviesAsync(Enums.MovieCategory.upcoming, count)
            };
            return View(data);
        }

        public async Task<IActionResult> IndexPopular()
        {
            const int count = 18;

            var data = new LandingPageVM()
            {
                CustomCollections = await _context.Collection
                                .Include(c => c.MovieCollections)
                                .ThenInclude(mc => mc.Movie)
                                .ToListAsync(),
                Popular = await _movieService.SearchMoviesAsync(Enums.MovieCategory.popular, count),
            };
            return View(data);
        }

        public async Task<IActionResult> IndexTopRated()
        {
            const int count = 18;

            var data = new LandingPageVM()
            {
                CustomCollections = await _context.Collection
                                .Include(c => c.MovieCollections)
                                .ThenInclude(mc => mc.Movie)
                                .ToListAsync(),
                TopRated = await _movieService.SearchMoviesAsync(Enums.MovieCategory.top_rated, count)
            };
            return View(data);
        }

        public async Task<IActionResult> IndexNowPlaying()
        {
            const int count = 18;

            var data = new LandingPageVM()
            {
                CustomCollections = await _context.Collection
                                .Include(c => c.MovieCollections)
                                .ThenInclude(mc => mc.Movie)
                                .ToListAsync(),
                NowPlaying = await _movieService.SearchMoviesAsync(Enums.MovieCategory.now_playing, count)
            };
            return View(data);
        }


        public async Task<IActionResult> BrowseMovies(string genre, string page)
        {
            //const int count = 30;

            if (page is null)
            {
                page = "1";
                //Convert.ToInt32(page);
            }

            if (genre == null) {
                genre = "35";
            }
            //else
            //{
            //    var newPage = Convert.ToInt32(page);
            //    newPage = newPage + 1;
            //    page = newPage.ToString();

            //}

            var data = new LandingPageVM()
            {
                CustomCollections = await _context.Collection
                                .Include(c => c.MovieCollections)
                                .ThenInclude(mc => mc.Movie)
                                .ToListAsync(),
                BrowseMovies = await _movieService.BrowseAllMoviesAsync(genre, page)
            };
            ViewData["Genre"] = genre;

            //var newPage = Convert.ToInt32(page);
            //newPage = newPage + 1;
            //page = newPage.ToString();

            ViewData["Page"] = page;
            return View(data);
        }

        
        public async Task<IActionResult> FindMovies(string searchTerm, string page)
        {

            if (page is null)
            {
                page = "1";
                //Convert.ToInt32(page);
            }
            //change input string; switch spaces with plus sign
            if(searchTerm is null)
            {
                searchTerm = "matrix";
            }

            searchTerm = searchTerm.Replace(' ', '+');

            var data = new LandingPageVM()
            {
                CustomCollections = await _context.Collection
                                .Include(c => c.MovieCollections)
                                .ThenInclude(mc => mc.Movie)
                                .ToListAsync(),
                FindMovies = await _movieService.FindMoviesAsync(searchTerm, page)
            };

            ViewData["Page"] = page;

            ViewData["SearchTerm"] = searchTerm;
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
