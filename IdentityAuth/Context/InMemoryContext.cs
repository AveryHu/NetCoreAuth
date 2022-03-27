using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityAuth.Context
{
    public class InMemoryContext : IdentityDbContext
    {
        public InMemoryContext(DbContextOptions<InMemoryContext> options)
            : base(options) { }
    }
}