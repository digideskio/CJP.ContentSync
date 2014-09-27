using System;
using CJP.ContentSync.Models;
using Orchard.Data.Migration;

namespace CJP.ContentSync.Migrations
{
    public class DataMigrations : DataMigrationImpl
    {
        public int Create() {
            return 1;
        }

        public int UpdateFrom1()
        {

            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.DropTable(typeof(MigrationExecutionRecord).Name);

            SchemaBuilder.CreateTable(typeof(MigrationExecutionRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("MigrationName")
                .Column<DateTime>("ExecutedAt"));

            return 3;
        }
    }
}