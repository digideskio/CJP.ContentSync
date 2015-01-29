using System.Linq;
using CJP.ContentSync.Models;
using Orchard.Data;
using Orchard.Services;

namespace CJP.ContentSync.RetentionPolicies 
{
    public class PreviousYearSnapshotRetentionPolicy : ISnapshotRetentionPolicy
    {
        private readonly IRepository<SnapshotRecord> _repository;
        private readonly IClock _clock;

        public PreviousYearSnapshotRetentionPolicy(IRepository<SnapshotRecord> repository, IClock clock)
        {
            _repository = repository;
            _clock = clock;
        }

        public int[] GetSnapshotIdsToRetain()
        {//keep 1 snapshot a month for the past year
            var records = _repository.Table.Where(r => r.TimeTaken > _clock.UtcNow.AddYears(-1)).ToList();

            var groups = records.GroupBy(r => r.TimeTaken.ToString("yyyy MMMM"), r => r.Id);

            return groups.Select(g => g.FirstOrDefault()).ToArray();
        }
    }
}