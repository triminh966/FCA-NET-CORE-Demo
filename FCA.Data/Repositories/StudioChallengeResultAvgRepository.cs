using System.Collections.Generic;
using System.Linq;
using FCA.Data.Entities;

namespace FCA.Data.Repositories
{
    public interface IStudioChallengeResultAvgRepository : IGenericRepository<StudioChalllengeResultAvg>
    {
        List<StudioChalllengeResultAvg> GetStudioChalllengeResultAvgs(List<int> studioIds, int challengeId);
    }

    public class StudioChallengeResultAvgRepository : GenericRepository<FCAContext, StudioChalllengeResultAvg>,
        IStudioChallengeResultAvgRepository
    {
        private readonly FCAContext _fcaContext;
        public StudioChallengeResultAvgRepository(FCAContext fcaContext) : base(fcaContext)
        {
            _fcaContext = fcaContext;;
        }

        public List<StudioChalllengeResultAvg> GetStudioChalllengeResultAvgs(List<int> studioIds, int challengeId)
        {
            var studioChallengeResultAvgs = (from c in _fcaContext.Challenge
                join s in _fcaContext.StudioChalllengeResultAvg on c.ChallengeId equals s.ChallengeId
                where studioIds.Contains(c.StudioId) && s.ChallengeId == challengeId
                select s).ToList();

            return studioChallengeResultAvgs;
        }
    }
}
