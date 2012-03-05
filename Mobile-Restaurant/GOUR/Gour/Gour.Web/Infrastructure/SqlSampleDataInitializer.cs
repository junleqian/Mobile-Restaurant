namespace Gour.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using Gour.Web.Models;

    // Summary:
    //     Sample Entity Framework 4.1 data initializer for SQL Azure.
    //     The seed method is executed after the database is created using EF 4.1 Code-First, 
    //     giving the developer the opportunity to insert "seed" (i.e. initial) data.
    //     For more information, visit the ADO.NET Entity Framework website at http://msdn.microsoft.com/data/aa937723

    public class SqlSampleDataInitializer : DropCreateDatabaseIfModelChanges<SqlDataContext>
    {

        protected override void Seed(SqlDataContext context)
        {
            var data = new List<SqlSampleData>
            {
                new SqlSampleData
                {
                    Id = 1,
                    Title = "I am the first title",
                    IsPublic = true,
                    Date = DateTime.UtcNow
                },

                new SqlSampleData
                {
                    Id = 2,
                    Title = "I am the second title",
                    IsPublic = true,
                    Date = DateTime.UtcNow
                }
            };

            data.ForEach(d => context.SqlSampleData.Add(d));
        }
    }
}