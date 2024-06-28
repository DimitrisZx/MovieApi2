using MovieApi2;
using Microsoft.EntityFrameworkCore;
using NSwag.AspNetCore;

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

movies.MapGet("/", async (MovieDb db) =>
    await db.Movies.ToListAsync());

movies.MapGet("/available", async (MovieDb db) =>
    await db.Movies.Where(m => !m.Owned).ToListAsync());

movies.MapGet("/{id}", async (int id, MovieDb db) =>
    await db.Movies.Where(m => m.Id == id).ToListAsync());

movies.MapPost("/", async (Movie movie, MovieDb db) =>
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();

    return Results.Created($"/movies/{movie.Id}", movie);
});

movies.MapPut("/{id}", async (int id, Movie inputMovie, MovieDb db) =>
{
    var movie = await db.Movies.FindAsync(id);

    if (movie is null) return Results.NotFound();

    movie.Title = inputMovie.Title;
    movie.Owned = inputMovie.Owned;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

movies.MapDelete("/{id}", async (int id, MovieDb db) =>
{
    if (await db.Movies.FindAsync(id) is not { } movie) 
        return Results.NotFound();
    
    db.Movies.Remove(movie);
    await db.SaveChangesAsync();
    return Results.NoContent();

});
    

app.Run();
