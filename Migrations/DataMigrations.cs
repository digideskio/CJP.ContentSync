using System;
using CJP.ContentSync.Models;
using Orchard.Data.Migration;

namespace CJP.ContentSync.Migrations
{
    public class DataMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(MigrationExecutionRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("MigrationName")
                .Column<DateTime>("ExecutedAt"));

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.CreateTable(typeof(RedactionRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("Regex")
                .Column<string>("Placeholder")
                .Column<string>("ReplaceWith"));

            return 2;
        }
    }
}