using System.Collections.Generic;
using System.Linq;
using FCA.API.Models;
using FCA.Core.Enums;
using FCA.Data.Entities;
using FCA.Data.Repositories;

namespace FCA.API.Services
{
    public interface IChallengeTrackerService
    {
        /// <summary>
        /// Get Studio Challlenge Result Average
        /// </summary>
        /// <param name="challengeId"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        IEnumerable<StudioChallengeResultAvgModel> GetStudioChalllengeResultAvg(int challengeId,
            StudioChallengeResultAvgParameters queryParams);
    }

    public class ChallengeTrackerService : IChallengeTrackerService
    {
        private readonly IStudioChallengeResultAvgRepository _studioChallengeResultAvgRepository;
        private readonly IStudioRepository _studioRepository;
        private readonly IChallengeRepository _challengeRepository;

        public ChallengeTrackerService(IStudioRepository studioRepository, IStudioChallengeResultAvgRepository studioChallengeResultAvgRepository, IChallengeRepository challengeRepository)
        {
            _studioRepository = studioRepository;
            _studioChallengeResultAvgRepository = studioChallengeResultAvgRepository;
            _challengeRepository = challengeRepository;
        }

        public IEnumerable<StudioChallengeResultAvgModel> GetStudioChalllengeResultAvg(int challengeId,
            StudioChallengeResultAvgParameters queryParams)
        {
            var todayChallenge = _challengeRepository.GetTodayChallenge(challengeId);
            if (todayChallenge == null) return null;

            List<StudioChalllengeResultAvg> studioChalllengeResultAvgs;
            var studioIds = _studioRepository.GetStudioByDivision(queryParams.Division, queryParams.StateCode, queryParams.CountryCode);
            if (studioIds != null && studioIds.Any())
            {
                studioChalllengeResultAvgs = _studioChallengeResultAvgRepository.GetStudioChalllengeResultAvgs(studioIds, challengeId);
            }
            else
            {       
                studioChalllengeResultAvgs = _studioChallengeResultAvgRepository.FindBy(x => x.ChallengeId == challengeId).ToList();
            }

            foreach (var studioChalllengeResultAvg in studioChalllengeResultAvgs)
            {
                dynamic allStudioAverage;
                if (todayChallenge.MetricEntry.EntryTypeId != (int) EntryTypes.Time)
                {
                    allStudioAverage = studioChalllengeResultAvgs.Select(x => x.AverageResult).Average();
                } 


            }

            return null;
        }
    }
}
