using System;
using Bogus;
using MyBlog.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBlog.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });
            //Insert Data
            //migrationBuilder.InsertData(
            //    table: "Articles",
            //    columns: new[] { "Title", "Created", "Content" },
            //    values: new object[]
            //    {
            //        "Content 1",
            //        new DateTime(2021,8,20),
            //        "Today is Monday"
            //    }
            //    );
            //Fake Data :Bogus
            Randomizer.Seed = new Random(8675309);

            var fakeArticle = new Faker<Article>();
            fakeArticle.RuleFor(a => a.Title, f => f.Lorem.Sentence(5, 5));
            fakeArticle.RuleFor(a => a.Created, f => f.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 12, 30)));
            fakeArticle.RuleFor(a => a.Content, f => f.Lorem.Paragraphs(1, 4));

            for (int i = 0; i < 150; i++)
            {
                Article article = fakeArticle.Generate();
                migrationBuilder.InsertData(
               table: "Articles",
               columns: new[] { "Title", "Created", "Content" },
               values: new object[]
               {
                    article.Title,
                    article.Created,
                    article.Content
               }
               );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
