using System;
using System.ComponentModel.DataAnnotations;

namespace ReFound.Models
{
    public class ClaimRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select the found item.")]
        [Display(Name = "Found Item ID")]
        public int? FoundItemId { get; set; }

        public int? ClaimantUserId { get; set; }

        [Required(ErrorMessage = "Please add time or date.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Claim Date")]
        public DateTime? ClaimDate { get; set; }

        [Required(ErrorMessage = "Please provide details as proof of ownership.")]
        [StringLength(500, ErrorMessage = "Proof of ownership cannot exceed 500 characters.")]
        [Display(Name = "Proof of Ownership")]
        public string ProofOfOwnership { get; set; } = "";

        public string Status { get; set; } = "Pending";
    }
}