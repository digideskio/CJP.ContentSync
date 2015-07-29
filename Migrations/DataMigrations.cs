using System;
using CJP.ContentSync.Models;
using CJP.ContentSync.Services;
using Orchard.Data.Migration;

namespace CJP.ContentSync.Migrations
{
    public class DataMigrations : DataMigrationImpl
    {
        private readonly IInfoSetMigrationService _migrationService;

        public DataMigrations(IInfoSetMigrationService migrationService)
        {
            _migrationService = migrationService;
        }

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(MigrationExecutionRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("MigrationName")
                .Column<DateTime>("ExecutedAt"));

            return 9;
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

        public int UpdateFrom2()
        {
            SchemaBuilder.CreateTable(typeof(SnapshotRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<DateTime>("TimeTaken")
                .Column<string>("Data", column => column.Unlimited()));

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.CreateTable(typeof(RemoteSiteConfigRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<DateTime>("LastSynced", column => column.Nullable())
                .Column<string>("Url")
                .Column<string>("Username")
                .Column<string>("Password"));

            return 4;
        }

        public int UpdateFrom4() { return 5; }

        public int UpdateFrom5()
        {
            SchemaBuilder.CreateTable(typeof(SettingRedactionRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("SettingName", column => column.Unlimited())
                .Column<string>("Value", column => column.Unlimited()));

            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.CreateTable(typeof(FeatureRedactionRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("FeatureId", column => column.Unlimited())
                .Column<bool>("Enabled"));

            return 7;
        }

        public int UpdateFrom7() { return 8; }

        public int UpdateFrom8()
        {
            _migrationService.Migrate<ContentSyncSettingsPart>(this, "ContentSyncSettingsRecord", (part, row) =>
            {
                part.Store("ExcludedExportSteps", (string)row[1]);
                part.Store("ExcludedSiteSettings", (string)row[2]);
                part.Store("ExcludedContentTypes", (string)row[3]);
                part.SnapshotFrequencyMinutes = (int) row[4];
            });

            return 9;
        }
    }
}