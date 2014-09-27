using System.Collections.Generic;
using CJP.ContentSync.Services;

namespace CJP.ContentSync.Models {
    public class MigrationExectutionSummary
    {
        public MigrationExectutionSummary() {
            SuccessfulMigrations = new List<string>();
            FailedMigrations = new List<FailedMigrationSummary>();
        }

        public List<string> SuccessfulMigrations { get; set; }
        public List<FailedMigrationSummary> FailedMigrations { get; set; } 
    }
}