using System.Collections.Generic;
using System.Linq;
using FCA.Data.Entities.FCA;
using FCA.Data.ResponseModels;

namespace FCA.Data.Repositories
{
    public interface IChallengeRepository : IGenericRepository<Challenge>
    {
        ChallengeResponse GetTodayChallenge(int challengeId);
    }

    public class ChallengeRepository : GenericRepository<FCAContext, Challenge>, IChallengeRepository
    {
        private readonly FCAContext _fcaContext;
        public ChallengeRepository(FCAContext fcaContext) : base(fcaContext)
        {
            _fcaContext = fcaContext;
        }

        public ChallengeResponse GetTodayChallenge(int challengeId)
        {
            var challengeResponse = (from c in _fcaContext.Challenge
                                     join cme in _fcaContext.ChallengeMetricEntry on c.ChallengeId equals cme.ChallengeId
                                     join m in _fcaContext.MetricEntry on cme.MetricEntryId equals m.MetricEntryId
                                     where c.ChallengeId == challengeId
                                     select new ChallengeResponse
                                     {
                                         ChallengeId = c.ChallengeId,
                                         MetricEntry = m,
                                         StudioId = c.StudioId,
                                         CountryId = c.CountryId,
                                         ChallengeCategoryId = c.ChallengeCategoryId,
                                         DateCreated = c.DateCreated,
                                         DateUpdated = c.DateUpdated,
                                         ChallengeUUId = c.ChallengeUUId,
                                         EndDate = c.EndDate,
                                         StartDate = c.StartDate
                                     }).FirstOrDefault();

            return challengeResponse;
        }
    }
}