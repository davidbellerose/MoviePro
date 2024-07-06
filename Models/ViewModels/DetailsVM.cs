using MoviePro.Models.Database;
using MoviePro.Models.TMDB;
using System.Collections.Generic;

namespace MoviePro.Models.ViewModels
{
    public class DetailsVM
    {
        public Movie Movie { get; set; }
        public Collection Collection { get; set; }
        public MovieCollection MovieCollection { get; set; }
    }
}
