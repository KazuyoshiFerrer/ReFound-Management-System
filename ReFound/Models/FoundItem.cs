using System;
using System.ComponentModel.DataAnnotations;
using ReFound.Validation;

namespace ReFound.Models;

public class FoundItem
{
    public int Id { get; set; }

    [Display(Name = "Item Name")]
    [Required(ErrorMessage = "Please enter the item name.")]
    [StringLength(100, ErrorMessage = "The item name can't be longer than 100 characters.")]
    public string? ItemName { get; set; }

    [Display(Name = "Description")]
    [Required(ErrorMessage = "Please provide a description.")]
    [StringLength(500, ErrorMessage = "The description can't be longer than 500 characters.")]
    public string? Description { get; set; }

    [Display(Name = "Category")]
    [Required(ErrorMessage = "Please enter a category.")]
    [StringLength(50, ErrorMessage = "The category can't be longer than 50 characters.")]
    public string? Category { get; set; }

    [Display(Name = "Date Found")]
    [DataType(DataType.Date)]
    [NotFutureDate(ErrorMessage = "Date found can't be in the future.")]
    public DateTime? DateFound { get; set; } = DateTime.Today;

    [Display(Name = "Location Found")]
    [Required(ErrorMessage = "Please specify where the item was found.")]
    [StringLength(150, ErrorMessage = "The location can't be longer than 150 characters.")]
    public string? LocationFound { get; set; }

    [Required]
    public string Status { get; set; } = "Found";
}