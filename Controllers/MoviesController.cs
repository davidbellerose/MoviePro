using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoviePro.Data;
using MoviePro.Models;
using MoviePro.Models.Database;
using MoviePro.Models.Settings;
using MoviePro.Models.ViewModels;
using MoviePro.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MoviePro.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class MoviesController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IRemoteMovieService _movieService;
        private readonly IDataMappingService _mappingService;

        public MoviesController(IOptions<AppSettings> appSettings, ApplicationDbContext context, IImageService imageService, IRemoteMovieService tmdbMovieService, IDataMappingService tmdbMappingService)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _imageService = imageService;
            _movieService = tmdbMovieService;
            _mappingService = tmdbMappingService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Library(int pg = 1)
        {
            // original, no prameters in signature
            var movies = await _context.Movie.OrderBy(m => m.Title).ThenBy(m => m.ReleaseDate).ToListAsync();
            ViewData["Collections"] = _context.Collection.ToList();
            //return View(movies);
            // end original

            int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = movies.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = movies.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            return View(data);

        }

        //GET: The mapped(copy) API version
        public async Task<IActionResult> Import()
        {
            var movies = await _context.Movie.ToListAsync();
            return View(movies);
        }

        //POST: The mapped(copy) API version
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(int id, int collectionId, Movie movieImport)
        {
            if (_context.Movie.Any(m => m.MovieId == id))
            {
                var localMovie = await _context.Movie.FirstOrDefaultAsync(m => m.MovieId == id);
                return RedirectToAction("Details", "Movies", new { id = localMovie.MovieId, local = true });
            }

            //this gets the movie with id from the api
            var movieDetail = await _movieService.MovieDetailAsync(id);

            // this takes the movie just retreived from the api and puts it into an object
            var movie = await _mappingService.MapMovieDetailAsync(movieDetail);

            movie.MyRating = movieImport.MyRating;
            movie.Comments = movieImport.Comments;

            // the movie in the object is now added to the database
            _context.Add(movie);
            await _context.SaveChangesAsync();

            // adds movie to the default "All" collection
            //await AddToMovieCollection(movie.Id, _appSettings.MovieProSettings.DefaultCollection.Name);

            // adds movie to collection from select list
            // the select list default is "All"
            await AddToMovieCollection(movie.Id, collectionId);

            //return RedirectToAction("Import");
            return RedirectToAction("Details", "Movies", new { id = movie.MovieId, local = true });
        }

        public IActionResult Create()
        {
            // the select list is a list of the collections/catagories
            ViewData["CollectionId"] = new SelectList(_context.Collection, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId,Title,TagLine,Overview,RunTime,ReleaseDate,Rating,VoteAverage,Poster,PosterType,Backdrop,BackdropType,TailerUrl")] Movie movie, int collectionId)
        {
            if (ModelState.IsValid)
            {
                movie.PosterType = movie.PosterFile?.ContentType;
                movie.Poster = await _imageService.EncodeImageAsync(movie.PosterFile);

                movie.BackdropType = movie.BackdropFile?.ContentType;
                movie.Backdrop = await _imageService.EncodeImageAsync(movie.BackdropFile);

                _context.Add(movie);
                await _context.SaveChangesAsync();

                await AddToMovieCollection(movie.Id, collectionId);

                return RedirectToAction("Index", "MovieCollections");
            }

            return View(movie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,Title,TagLine,Overview,RunTime,ReleaseDate,Rating,VoteAverage,Poster,PosterType,Backdrop,BackdropType,TailerUrl, Comments, MyRating")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (movie.PosterFile != null)
                    {
                        movie.PosterType = movie.PosterFile.ContentType;
                        movie.Poster = await _imageService.EncodeImageAsync(movie.PosterFile);
                    }

                    if (movie.BackdropFile != null)
                    {
                        movie.BackdropType = movie.BackdropFile.ContentType;
                        movie.Backdrop = await _imageService.EncodeImageAsync(movie.BackdropFile);
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Movies", new { id = movie.Id, local = true });
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMovie(int id, int collectionId, string Comments, string MyRating)
        {
            var movie = _context.Movie.Find(id);
            //if (id != movie.MovieId)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    // The poster is handled in the view, showing a default image if null
                    //if (movie.PosterFile != null)
                    //{
                    //    movie.PosterType = movie.PosterFile.ContentType;
                    //    movie.Poster = await _imageService.EncodeImageAsync(movie.PosterFile);
                    //}

                    //if (movie.BackdropFile != null)
                    //{
                    //    movie.BackdropType = movie.BackdropFile.ContentType;
                    //    movie.Backdrop = await _imageService.EncodeImageAsync(movie.BackdropFile);
                    //}
                    if (movie != null)
                    {
                        movie.MyRating = MyRating;
                        movie.Comments = Comments;
                        _context.Update(movie);
                        await _context.SaveChangesAsync();
                        await UpdateMovieCollection(id, collectionId);
                    }
                    return RedirectToAction("Details", "Movies", new { id = movie.MovieId, local = true });

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Details", "Movies", new { id = movie.Id, local = true });
        }

        public async Task<IActionResult> Delete(int? id, bool local = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie movie = new();
            if (local)
            {
                //Get the Movie data straight from the DB
                movie = await _context.Movie.Include(m => m.Cast)
                                            .Include(m => m.Crew)
                                            .FirstOrDefaultAsync(m => m.Id == id);
            }
            else
            {
                //Get the movie data from the TMDB API
                var movieDetail = await _movieService.MovieDetailAsync((int)id);
                movie = await _mappingService.MapMovieDetailAsync(movieDetail);
            }

            if (movie == null)
            {
                return NotFound();
            }

            ViewData["Local"] = local;
            return View(movie);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction("Library", "Movies");
        }

        private bool MovieExists(int id)
        {
            var movie = _context.Movie.Where(e => e.MovieId == id).ToList();

            if (movie.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
            //return _context.Movie.Any(e => e.MovieId == id);
        }

        public async Task<IActionResult> Details(int id, bool local = false)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                return NotFound();
            }

            if (MovieExists(id))
            {
                local = true;
            }

            Movie movie = new();
            if (local)
            {
                movie = await _context.Movie
                    .Include(m => m.Cast)
                    .Include(m => m.Crew)
                    .Include(m => m.MovieCollections)
                        .ThenInclude(m => m.Collection)
                    .FirstOrDefaultAsync(m => m.MovieId == id);
            }
            else
            {
                var movieDetail = await _movieService.MovieDetailAsync(id);
                movie = await _mappingService.MapMovieDetailAsync(movieDetail);
            }

            if (movie == null)
            {
                return NotFound();
            }


            if (local)
            {
                // need to get collection id from moviecollections for movieid
                var collectionId = _context.MovieCollection.Where(m => m.Movie.Id == movie.Id).Select(m => m.CollectionId).ToList();

                // there is only one in the list so take the first index
                var cId = collectionId[0];

                // now I need the name of the collection
                var collectionName = _context.Collection.Where(m => m.Id == cId).Select(m => m.Name).ToList();

                // only one in the list so take the first index
                var cName = collectionName[0];

                ViewData["cName"] = cName;
            }
            
            ViewData["CollectionId"] = new SelectList(_context.Collection, "Id", "Name");
            ViewData["Local"] = local;
            return View(movie);
        }

        private async Task AddToMovieCollection(int movieId, string collectionName)
        {
            var collection = await _context.Collection.FirstOrDefaultAsync(c => c.Name == collectionName);
            _context.Add(
                new MovieCollection()
                {
                    CollectionId = collection.Id,
                    MovieId = movieId
                });

            await _context.SaveChangesAsync();
        }

        private async Task AddToMovieCollection(int movieId, int collcetionId)
        {
            _context.Add(
                new MovieCollection()

                {
                    CollectionId = collcetionId,
                    MovieId = movieId
                });

            await _context.SaveChangesAsync();
        }

        private async Task UpdateMovieCollection(int id, int collcetionId)
        {
            var collection = await _context.MovieCollection.Where(c => c.MovieId == id).FirstOrDefaultAsync();


            collection.CollectionId = collcetionId;
            collection.MovieId = id;
            //await _context.Update(collection);
            await _context.SaveChangesAsync();
        }

    }
}
