using System.Linq;
using CJP.ContentSync.Models;
using Orchard.Data;
using Orchard.Services;

namespace CJP.ContentSync.RetentionPolicies 
{
    public class PreviousWeekSnapshotRetentionPolicy : ISnapshotRetentionPolicy
    {
        private readonly IRepository<SnapshotRecord> _repository;
        private readonly IClock _clock;

        public PreviousWeekSnapshotRetentionPolicy(IRepository<SnapshotRecord> repository, IClock clock)
        {
            _repository = repository;
            _clock = clock;
        }

        public int[] GetSnapshotIdsToRetain()
        {//keep 1 snapshot a day for the past 7 days
            var records = _repository.Table.Where(r => r.TimeTaken > _clock.UtcNow.AddDays(-7)).ToList();

            var groups = records.GroupBy(r => r.TimeTaken.ToString("yyyy MMMM dd"), r => r.Id);

            return groups.Select(g => g.FirstOrDefault()).ToArray();
        }
    }
}