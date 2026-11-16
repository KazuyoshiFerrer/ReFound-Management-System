using System.ComponentModel.DataAnnotations;

namespace ReFound.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // "Student" or "Admin"
    [Required]
    public string Role { get; set; } = "Student";
}