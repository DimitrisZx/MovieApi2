namespace MovieApi2;

public class Movie
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public Genre Genre { get; set; }
    public int BoxOffice { get; set; }
    public bool Owned { get; set; }
    public required Actor[] Actors { get; set; }
}

public class Actor
{
    public required string Name { get; set; }
    public required string Id { get; set; }
}
public enum Genre
{
    Horror
}
