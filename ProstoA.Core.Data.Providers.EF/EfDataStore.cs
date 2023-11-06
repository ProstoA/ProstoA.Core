using Microsoft.EntityFrameworkCore;

namespace ProstoA.Core.Data.Providers.EF;

public class EfDataStore<TDbContext> : DataStore
    where TDbContext : DbContext
{
    public TDbContext DbContext { get; }
    
    public EfDataStore(TDbContext dbContext)
    {
        DbContext = dbContext;
    }
}