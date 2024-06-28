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

app.Run();
