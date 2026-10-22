namespace ReFound.Models;

public class ClaimRequest
{
    public int Id { get; set; }
    public int FoundItemId { get; set; }
    public int ClaimantUserId { get; set; }
    public DateTime ClaimDate { get; set; } = DateTime.Now;
    public string ProofOfOwnership { get; set; } = "";
    public string Status { get; set; } = "Pending";
}