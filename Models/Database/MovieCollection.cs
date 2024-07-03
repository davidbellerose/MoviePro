namespace MoviePro.Models.Database
{
    // this is the join table for many-to-many movie-collection
    public class MovieCollection
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public int MovieId { get; set; }

        public int Order { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
