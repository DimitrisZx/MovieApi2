using Microsoft.EntityFrameworkCore;

namespace MovieApi2;

public class MovieDb : DbContext
{
    public MovieDb(DbContextOptions<MovieDb> options) : base(options) { }
    public DbSet<Movie> Movies => Set<Movie>();
}

