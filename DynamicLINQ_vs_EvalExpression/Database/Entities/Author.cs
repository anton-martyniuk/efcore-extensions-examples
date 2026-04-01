using System.Text.Json.Serialization;

namespace Database.Entities;

public class Author
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    [JsonIgnore]
    public List<Book> Books { get; set; } = [];
}
