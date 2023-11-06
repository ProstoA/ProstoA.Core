using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProstoA.Core.Data.Providers.EF;

namespace ProstoA.Core.Data.Tests;

public record struct PostId(Guid Value);

public record struct UserId(string Value);

public record Post(PostId Id, string Name, string Content);

public record User(UserId Id, string Name);

public class MyContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(x => x.Id).HasConversion(
                x => x.Value,
                x => new UserId(x));
        
        modelBuilder.Entity<Post>()
            .Property(x => x.Id).HasConversion(
                x => x.Value,
                x => new PostId(x));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new SqlConnectionStringBuilder {
            DataSource = "192.168.21.100",
            UserID = "sa",
            Password = "nj4rfnj4rfnj4rfnj4rf",
            InitialCatalog = "my_test"
        };

        optionsBuilder.UseSqlServer(builder.ToString());
                    //.UseNpgsql(@"");
    }
    
    public DbSet<Post> Posts { get; set; }
    
    public DbSet<User> Users { get; set; }
}

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        await using var context = new MyContext();
        var dataStore = new EfDataStore<MyContext>(context);
        var dataset = new EfDataSet<User>(context.Users);

        await context.SaveChangesAsync();
    }
}