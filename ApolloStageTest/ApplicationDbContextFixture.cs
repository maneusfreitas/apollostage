using System;
using ApolloStage.Data;
using ApolloStage.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApolloStageTest
{
	public class ApplicationDbContextFixture : IDisposable
    {
        public ApplicationDbContext DbContext { get; private set; }

        public ApplicationDbContextFixture()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;
            DbContext = new ApplicationDbContext(options);

            DbContext.Database.EnsureCreated();

            DbContext.Add(new ApplicationUser
            {
                Name = "John Doe",
                UserMail = "john@example.com",
                Password = "Password123@",
                ConfirmPassword = "Password123@",
                Code = "0",
                ConfirmedEmail = false,
            });

            DbContext.SaveChanges();
        }

        public void Dispose() => DbContext.Dispose();
    }
}