using System.ComponentModel.DataAnnotations;

namespace ReFound.Models;

public class ClaimRequest
{
    public int Id { get; set; }

    public int FoundItemId { get; set; }

    public FoundItem? FoundItem { get; set; }

    [Required]
    [StringLength(100)]
    public string StudentName { get; set; } = "";

    [Required]
    [StringLength(20)]
    public string StudentID { get; set; } = "";

    [Required]
    [Phone]
    [StringLength(20)]
    public string ContactNumber { get; set; } = "";

    [Required]
    [StringLength(500)]
    public string ClaimDescription { get; set; } = "";

    [DataType(DataType.Date)]
    public DateTime RequestDate { get; set; } = DateTime.Today;

    [Required]
    public string Status { get; set; } = "Pending";
}