using MovieApi2;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MovieDb>(options => options.UseInMemoryDatabase("Movies"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "MovieApi";
    config.Title = "MovieApi v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "MovieApi";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

var movies = app.MapGroup("/movies");

movies.MapGet("/", GetAllMovies);
movies.MapGet("/available", GetAvailableMovies);
movies.MapGet("/{id}", GetMovie);
movies.MapPost("/", CreateMovie);
movies.MapPut("/{id}", UpdateMovie);
movies.MapDelete("/{id}", DeleteMovie);

static async Task<IResult> GetAllMovies(MovieDb db)
{
    return TypedResults.Ok(await db.Movies.ToArrayAsync());
}

static async Task<IResult> GetMovie(int id, MovieDb db)
{
    return await db.Movies.FindAsync(id)
        is { } movie
        ? TypedResults.Ok(movie)
        : TypedResults.NotFound();
}

static async Task<IResult> GetAvailableMovies(MovieDb db)
{
    var movies= await db.Movies.Where(m => !m.Owned).ToListAsync();
    return TypedResults.Ok(movies);
};

static async Task<IResult> CreateMovie(Movie movie, MovieDb db)
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/movies/{movie.Id}", movie);
}

static async Task<IResult> UpdateMovie(int id, Movie inputMovie, MovieDb db)
{
    var movie = await db.Movies.FindAsync(id);
    if (movie is null)
        return TypedResults.NotFound();
    movie.Title = inputMovie.Title;
    movie.Owned = inputMovie.Owned;

    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteMovie(int id, MovieDb db)
{
    if (await db.Movies.FindAsync(id) is not { } movie)
        return TypedResults.NotFound();
    db.Movies.Remove(movie);
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
};

    

app.Run();
