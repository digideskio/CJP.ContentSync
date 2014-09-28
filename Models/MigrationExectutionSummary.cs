using System.Collections.Generic;

namespace CJP.ContentSync.Models {
    public class MigrationExectutionSummary
    {
        public MigrationExectutionSummary() {
            SuccessfulMigrations = new List<string>();
            FailedMigrations = new List<string>();
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public List<string> SuccessfulMigrations { get; set; }
        public List<string> FailedMigrations { get; set; } 
    }
}