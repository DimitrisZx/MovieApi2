using MovieApi2;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MovieDb>(options => options.UseInMemoryDatabase("Movies"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/movies", async (MovieDb db) =>
    await db.Movies.ToListAsync());

app.MapGet("/movies/available", async (MovieDb db) =>
    await db.Movies.Where(m => !m.Owned).ToListAsync());

app.MapGet("/movies/{id}", async (int id, MovieDb db) =>
    await db.Movies.Where(m => m.Id == id).ToListAsync());

app.MapPost("/movies", async (Movie movie, MovieDb db) =>
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();

    return Results.Created($"/movies/{movie.Id}", movie);
});

app.MapPut("/movies/{id}", async (int id, Movie inputMovie, MovieDb db) =>
{
    var movie = await db.Movies.FindAsync(id);

    if (movie is null) return Results.NotFound();

    movie.Title = inputMovie.Title;
    movie.Owned = inputMovie.Owned;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/movies/{id}", async (int id, MovieDb db) =>
{
    if (await db.Movies.FindAsync(id) is Movie movie)
    {
        db.Movies.Remove(movie);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});
    

app.Run();
