﻿using System;
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

        public int UpdateFrom4()
        {
            SchemaBuilder.CreateTable(typeof(ContentSyncSettingsRecord).Name, table => table
                .ContentPartRecord()
                .Column<string>("ExcludedExportSteps", column => column.Unlimited())
                .Column<string>("ExcludedSiteSettings", column => column.Unlimited())
                .Column<string>("ExcludedContentTypes", column => column.Unlimited()));

            return 5;
        }

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

        public int UpdateFrom7() 
        {
            SchemaBuilder.AlterTable(typeof(ContentSyncSettingsRecord).Name, table => table.AddColumn<int>("SnapshotFrequencyMinutes"));

            return 8;
        }
    }
}