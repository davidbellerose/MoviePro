using MoviePro.Enums;
using MoviePro.Models.TMDB;
using System.Threading.Tasks;

namespace MoviePro.Services.Interfaces
{
    public interface IRemoteMovieService
    {
        Task<MovieDetail> MovieDetailAsync(int id);
        Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count);
        Task<MovieSearch> FindMoviesAsync(string searchTerm, string page);
        Task<MovieSearch> BrowseAllMoviesAsync(string genre, string page);
        Task<ActorDetail> ActorDetailAsync(int id);
    }
}
