using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Autoplius.Repository
{
    public class AutopliusDatabase : DbContext
    {
        // Your context has been configured to use a 'AutopliusDatabase' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Autoplius.Repository.AutopliusDatabase' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'AutopliusDatabase' 
        // connection string in the application configuration file.
        public AutopliusDatabase()
            : base("name=AutopliusDatabase")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.
        public virtual DbSet<Searches> Searches { get; set; }
        public virtual DbSet<SearchesItem> SearchesItem { get; set; }
    }

    public class Searches
    {
        public Searches()
        {
            Cars = new List<SearchesItem>();
        }

        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }
        public bool Active { get; set; }
        public List<SearchesItem> Cars { get; set; }
    }

    public class SearchesItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime AddDateTime { get; set; }
        public double Price { get; set; }
        public int SearchesId { get; set; }
    }
}