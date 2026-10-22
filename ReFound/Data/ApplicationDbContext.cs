using Microsoft.EntityFrameworkCore;
using ReFound.Models;

namespace ReFound.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ClaimRequest> ClaimRequests { get; set; } = null!;
}