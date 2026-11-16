using Microsoft.EntityFrameworkCore;
using ReFound.Models;

namespace ReFound.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<FoundItem> FoundItems { get; set; } = null!;
    public DbSet<ClaimRequest> ClaimRequests { get; set; } = null!;
    public DbSet<LostItemReport> LostItemReports { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
}