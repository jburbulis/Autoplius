using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Autoplius.Repository;

public class AutopliusDatabase : DbContext
{
    // Your context has been configured to use a 'AutopliusDatabase' connection string from your application's 
    // configuration file (App.config or Web.config). By default, this connection string targets the 
    // 'Autoplius.Repository.AutopliusDatabase' database on your LocalDb instance. 
    // 
    // If you wish to target a different database and/or database provider, modify the 'AutopliusDatabase' 
    // connection string in the application configuration file.
    //public AutopliusDatabase()

    //{
    //}
    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //    => options.UseSqlite($@"Data Source=C:\Projects\individual\Test\autoplius.db");

    public virtual DbSet<Searches> Searches { get; set; }

    public virtual DbSet<SearchesItem> SearchesItem { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        //
#if DEBUG
        options.UseSqlite(@"Data Source=C:\Project\private\autoplius\autoplius.db");
#else
        options.UseSqlite(@"Data Source=/home/jburbulis/Databases/autoplius.db");
#endif
    }
}

public class Searches
{
    public Searches()
    {
        Cars = new List<SearchesItem>();
    }

    public bool Active { get; set; }

    public List<SearchesItem> Cars { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Url { get; set; }
}

public class SearchesItem
{
    public DateTime AddDateTime { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public double Price { get; set; }

    public int SearchesId { get; set; }

    public string Url { get; set; }
}