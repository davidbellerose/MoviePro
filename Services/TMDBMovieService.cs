using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MoviePro.Enums;
using MoviePro.Models.Database;
using MoviePro.Models.Settings;
using MoviePro.Models.TMDB;
using MoviePro.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MoviePro.Services
{
    public class TMDBMovieService : IRemoteMovieService
    {
        private AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClient;

        public TMDBMovieService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClient)
        {
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<ActorDetail> ActorDetailAsync(int id)
        {
            //Step 0: Setup a default return object
            ActorDetail actorDetail = new();


            // Environment Variable Settings
            var baseUrl = _appSettings.TMDBSettings.BaseUrl ?? Environment.GetEnvironmentVariable("BaseUrl");
            var apiKey = _appSettings.MovieProSettings.TmDbApiKey ?? Environment.GetEnvironmentVariable("TmDbApiKey");
            var language = _appSettings.TMDBSettings.QueryOptions.Language ?? Environment.GetEnvironmentVariable("Language");

            //Step 1: Assemble the full request uri string
            var query = $"{baseUrl}/person/{id}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", apiKey },
                { "language", language}
            };
            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 2: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Step 3: Return the ActorDetail object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();

                var dcjs = new DataContractJsonSerializer(typeof(ActorDetail));
                actorDetail = (ActorDetail)dcjs.ReadObject(responseStream);
            }

            return actorDetail;
        }

        public async Task<MovieDetail> MovieDetailAsync(int id)
        {
            //Step 0: Setup default return object
            MovieDetail movieDetail = new();

            // Environment Variable Settings
            var baseUrl = _appSettings.TMDBSettings.BaseUrl ?? Environment.GetEnvironmentVariable("BaseUrl");
            var apiKey = _appSettings.MovieProSettings.TmDbApiKey ?? Environment.GetEnvironmentVariable("TmDbApiKey");
            var language = _appSettings.TMDBSettings.QueryOptions.Language ?? Environment.GetEnvironmentVariable("Language");
            var appendToResponse = _appSettings.TMDBSettings.QueryOptions.AppendToResponse ?? Environment.GetEnvironmentVariable("AppendToResponse");

            //Step 1:
            var query = $"{baseUrl}/movie/{id}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", apiKey },
                { "language", language},
                { "append_to_response", appendToResponse}
            };
            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 2: Create client and execute request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Step 3: Return the Movie Detail object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var dcjs = new DataContractJsonSerializer(typeof(MovieDetail));
                movieDetail = dcjs.ReadObject(responseStream) as MovieDetail;
            }
            return movieDetail;
        }

        public async Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count)
        {
            //Step 0: Setup a default return object
            MovieSearch movieSearch = new();

            // Environment Variable Settings
            var baseUrl = Environment.GetEnvironmentVariable("BaseUrl") ?? _appSettings.TMDBSettings.BaseUrl;
            var apiKey = Environment.GetEnvironmentVariable("TmDbApiKey") ?? _appSettings.MovieProSettings.TmDbApiKey;
            var language = Environment.GetEnvironmentVariable("Language") ?? _appSettings.TMDBSettings.QueryOptions.Language;
            var page = Environment.GetEnvironmentVariable("page") ?? _appSettings.TMDBSettings.QueryOptions.Page;
            var baseImagePath = Environment.GetEnvironmentVariable("BaseImagePath") ?? _appSettings.TMDBSettings.BaseImagePath;
            var defaultPosterSize = Environment.GetEnvironmentVariable("DefaultPosterSize") ?? _appSettings.MovieProSettings.DefaultPosterSize;

            //Step 1: Assemble the full request uri string
            //var query = $"{_appSettings.TMDBSettings.BaseUrl}/movie/{category}";
            var query = $"{baseUrl}/movie/{category}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", apiKey},
                { "language", language },
                { "page", page }
            };


            //{
            //    { "api_key", _appSettings.MovieProSettings.TmDbApiKey },
            //    { "language", _appSettings.TMDBSettings.QueryOptions.Language },
            //    { "page", _appSettings.TMDBSettings.QueryOptions.Page }
            //};

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 2: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Step 3: Return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));
                using var responseStream = await response.Content.ReadAsStreamAsync();
                movieSearch = (MovieSearch)dcjs.ReadObject(responseStream);

                //Move this into a Business Logic layer
                movieSearch.results = movieSearch.results.Take(count).ToArray();
                movieSearch.results.ToList().ForEach(r => r.poster_path = $"{baseImagePath}/{defaultPosterSize}/{r.poster_path}");
            }
            return movieSearch;
        }

        public async Task<MovieSearch> FindMoviesAsync(string searchTerm, string page)
        {
            //Step 0: Setup a default return object
            MovieSearch movieSearch = new();

            // Environment Variable Settings
            var baseUrl = _appSettings.TMDBSettings.BaseUrl ?? Environment.GetEnvironmentVariable("BaseUrl");
            var apiKey = _appSettings.MovieProSettings.TmDbApiKey ?? Environment.GetEnvironmentVariable("TmDbApiKey");
            var language = _appSettings.TMDBSettings.QueryOptions.Language ?? Environment.GetEnvironmentVariable("Language");
            var baseImagePath = _appSettings.TMDBSettings.BaseImagePath ?? Environment.GetEnvironmentVariable("BaseImagePath");
            var defaultPosterSize = _appSettings.MovieProSettings.DefaultPosterSize ?? Environment.GetEnvironmentVariable("DefaultPosterSize");

            //Step 1: Assemble the full request uri string
            var query = $"{baseUrl}/search/movie?query={searchTerm}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", apiKey },
                { "language", language },
                //{ "page", _appSettings.TMDBSettings.QueryOptions.Page }
                { "page", page }
            };
            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 2: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Step 3: Return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));
                using var responseStream = await response.Content.ReadAsStreamAsync();
                movieSearch = (MovieSearch)dcjs.ReadObject(responseStream);

                //Move this into a Business Logic layer
                movieSearch.results = movieSearch.results.ToArray();
                movieSearch.results.ToList().ForEach(r => r.poster_path = $"{baseImagePath}/{defaultPosterSize}/{r.poster_path}");
            }
            return movieSearch;
        }

        public async Task<MovieSearch> BrowseAllMoviesAsync(string genre, string page)
        {
            //Step 0: Setup a default return object
            MovieSearch movieSearch = new();

            // Environment Variable Settings
            var baseUrl = _appSettings.TMDBSettings.BaseUrl ?? Environment.GetEnvironmentVariable("BaseUrl");
            var apiKey = _appSettings.MovieProSettings.TmDbApiKey ?? Environment.GetEnvironmentVariable("TmDbApiKey");
            var language = _appSettings.TMDBSettings.QueryOptions.Language ?? Environment.GetEnvironmentVariable("Language");
            var baseImagePath = _appSettings.TMDBSettings.BaseImagePath ?? Environment.GetEnvironmentVariable("BaseImagePath");
            var defaultPosterSize = _appSettings.MovieProSettings.DefaultPosterSize ?? Environment.GetEnvironmentVariable("DefaultPosterSize");

            //Step 1: Assemble the full request uri string
            var query = $"{baseUrl}/discover/movie";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", apiKey },
                { "with_genres", genre },
                { "language", language },
                //{ "page", _appSettings.TMDBSettings.QueryOptions.Page }
                { "page", page }
            };
            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 2: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Step 3: Return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));
                using var responseStream = await response.Content.ReadAsStreamAsync();
                movieSearch = (MovieSearch)dcjs.ReadObject(responseStream);

                //Move this into a Business Logic layer
                movieSearch.results = movieSearch.results.ToArray();
                movieSearch.results.ToList().ForEach(r => r.poster_path = $"{baseImagePath}/{defaultPosterSize}/{r.poster_path}");
            }
            return movieSearch;
        }
    }
}