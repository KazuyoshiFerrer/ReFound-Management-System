using System.ComponentModel.DataAnnotations;

namespace ReFound.Models;

public class FoundItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = "";

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = "";

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = "";

    [DataType(DataType.Date)]
    [Display(Name = "Date Found")]
    public DateTime DateFound { get; set; } = DateTime.Today;

    [Required]
    [StringLength(150)]
    [Display(Name = "Location Found")]
    public string LocationFound { get; set; } = "";

    [Required]
    public string Status { get; set; } = "Unclaimed";

    [Display(Name = "Item Photo")]
    public string? ImagePath { get; set; }
}