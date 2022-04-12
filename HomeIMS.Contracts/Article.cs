using System.ComponentModel.DataAnnotations;

namespace HomeIMS.Contracts;

public class Article
{
    [Key]
    public int Id { get; set; }

    public string EAN { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}