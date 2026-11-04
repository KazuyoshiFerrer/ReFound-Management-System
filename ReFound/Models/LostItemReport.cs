using System;
using System.ComponentModel.DataAnnotations;
using ReFound.Validation;

namespace ReFound.Models
{
    public class LostItemReport
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the title or item name.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        [Display(Name = "Item Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please provide a detailed description.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please specify where the item was last seen.")]
        [StringLength(150, ErrorMessage = "Location cannot exceed 150 characters.")]
        [Display(Name = "Last Seen Location")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the date the item was lost.")]
        [DataType(DataType.Date)]
        [NotFutureDate(ErrorMessage = "Date lost cannot be in the future.")]
        [Display(Name = "Date Lost")]
        public DateTime DateLost { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Please provide your contact information.")]
        [StringLength(100, ErrorMessage = "Contact info cannot exceed 100 characters.")]
        [Display(Name = "Contact Information")]
        public string ContactInfo { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
    }
}