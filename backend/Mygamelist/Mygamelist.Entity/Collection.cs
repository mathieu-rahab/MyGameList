namespace Mygamelist.Entity;

public class Collection
{ 
    public int Id { get; set; } 
    public int UserId { get; set; }
    public string Label { get; set; }
    public ICollection<int> GamesId { get; set; }
}