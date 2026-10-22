using System.ComponentModel.DataAnnotations;

namespace ReFound.Models;

public class FoundItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string ItemName { get; set; } = "";

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = "";

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = "";

    [DataType(DataType.Date)]
    public DateTime DateFound { get; set; } = DateTime.Today;

    [Required]
    [StringLength(150)]
    public string LocationFound { get; set; } = "";

    [Required]
    public string Status { get; set; } = "Found";
}