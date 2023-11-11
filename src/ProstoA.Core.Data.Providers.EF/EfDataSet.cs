using Microsoft.EntityFrameworkCore;

namespace ProstoA.Core.Data.Providers.EF;

public class EfDataSet<TEntity> : DataSet
    where TEntity : class
{
    public DbSet<TEntity> DbSet { get; }

    public EfDataSet(DbSet<TEntity> dbSet)
    {
        DbSet = dbSet;
    }
}