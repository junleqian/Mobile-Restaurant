namespace Gour.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    // Summary:
    //     Sample Entity Framework 4.1 data class for SQL Azure.
    //     Using EF 4.1 Code-First, the database structure will be created to mirror this class properties.
    //     For more information, visit the ADO.NET Entity Framework website at http://msdn.microsoft.com/data/aa937723
    public class SqlSampleData
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public bool IsPublic { get; set; }
    }
}