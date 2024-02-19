using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProstoA.Core.Data.Providers.EF;

namespace ProstoA.Core.Data.Tests;

public record struct PostId(Guid Value);

public record struct UserId(string Value);

public record Post(PostId Id, string Name, string Content);

public record User(UserId Id, string Name);

public interface ISecretManager<in TSecretKey>
{
    Task<NameValueCollection> Get(TSecretKey key);
}

public static class DotEnv
{
    // https://dusted.codes/dotenv-in-dotnet

    public static void Load()
    {
        var path = Path.Combine(Environment.CurrentDirectory, ".env");
        Load(path);
    }
    
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;

            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
} 

public record struct VaultSecretKey(string SecretId);

public class VaultServer : ISecretManager<VaultSecretKey>
{
    private const string GetSecretUri = "lockbox/v1/secrets/{secretId}/payload";
    
    private readonly Uri _baseUri;
    private readonly string _token;
    
    public VaultServer(string baseUri, string token)
    {
        _baseUri = new Uri(baseUri);
        _token = token;
    }

    private class Response
    {
        public ResponseEntry[] Entries { get; set; }
        
        public string VersionId { get; set; }
    }

    private class ResponseEntry
    {
        public string Key { get; set; }
        
        public string TextValue { get; set; }
    }
    
    public async Task<NameValueCollection> Get(VaultSecretKey key)
    {
        var uri = new Uri(_baseUri, GetSecretUri.Replace("{secretId}", key.SecretId));

        var httpHandler = new HttpClientHandler();
        var httpClient = new HttpClient(httpHandler);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var data = await httpClient.GetAsync(uri);
        var json = (Response?) await data.Content.ReadFromJsonAsync(typeof(Response));

        var result = new NameValueCollection();
        foreach (var item in json?.Entries ?? Array.Empty<ResponseEntry>())
        {
            result.Add(item.Key, item.TextValue);
        }
        
        return new NameValueCollection();
    }
}

public record MyDbSettings(string Host, string User, string Password, string DbName)
    : IOptions<MyDbSettings>
{
    MyDbSettings IOptions<MyDbSettings>.Value => this;
}

public class MyContext : DbContext
{
    private readonly IOptions<MyDbSettings> _dbSettings;

    public MyContext(IOptions<MyDbSettings> dbSettings)
    {
        _dbSettings = dbSettings;
    }
    
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
            DataSource = _dbSettings.Value.Host,
            UserID = _dbSettings.Value.User,
            Password = _dbSettings.Value.Password,
            InitialCatalog = _dbSettings.Value.DbName,
            TrustServerCertificate = true
        };

        optionsBuilder.UseSqlServer(builder.ToString());
                    //.UseNpgsql(@"");
    }
    
    public DbSet<Post> Posts { get; set; }
    
    public DbSet<User> Users { get; set; }
}

public class UnitTest1
{
    [Fact(Skip = "under construction")]
    public async Task Test1()
    {
        DotEnv.Load();

        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        
        var vault = new VaultServer(config["VAULT_SERVER"], config["VAULT_TOKEN"]);
        var data = await vault.Get(new VaultSecretKey(config["VAULT_SECRET"]));

        var dbSettings = new MyDbSettings(
            data["host"],
            data["user"],
            data["pwd"],
            data["db_name"]
        );
        
        await using var context = new MyContext(dbSettings);
        var dataStore = new EfDataStore<MyContext>(context);
        var dataset = new EfDataSet<User>(context.Users);

        await dataStore.DbContext.Database.MigrateAsync();
        
        await dataset.DbSet.Select(x => x).ToArrayAsync();

        await context.SaveChangesAsync();
    }
}