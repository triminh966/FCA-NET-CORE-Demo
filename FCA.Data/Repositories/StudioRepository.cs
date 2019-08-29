using System.Collections.Generic;
using System.Linq;
using FCA.Core;
using FCA.Data.DbContext;
using FCA.Data.Entities;

namespace FCA.Data.Repositories
{
    public interface IStudioRepository : IGenericRepository<Studio>
    {
        List<int> GetStudioByDivision(string division, string stateCode, string countryCode);
    }

    public class StudioRepository : GenericRepository<OTbaseContext, Studio>, IStudioRepository
    {
        private readonly OTbaseContext _oTbaseContext;
        public StudioRepository(OTbaseContext oTbaseContext) : base(oTbaseContext)
        {
            _oTbaseContext = oTbaseContext;
        }

        public List<int> GetStudioByDivision(string division, string stateCode, string countryCode)
        {
            var results = new List<int>();
            if (division != null)
            {
                switch (division.ToLower())
                {
                    case Constants.Division.State:
                        var studioLocationIds = _oTbaseContext.StudioLocation.Where(x => x.BillToState == stateCode).Select(x => x.StudioLocationId).ToList();
                        results = FindBy(x =>
                            x.StudioPhysicalLocationId != null &&
                            studioLocationIds.Contains((int)x.StudioPhysicalLocationId)).Select(x => x.StudioId)
                            .ToList();
                        break;
                    case Constants.Division.Country:
                        var countryId =
                            _oTbaseContext.Country.SingleOrDefault(x => x.CountryCode == countryCode)?.CountryId ?? 0;
                        studioLocationIds = _oTbaseContext.StudioLocation
                            .Where(s => s.BillToCountryId == countryId)
                            .Select(x => x.StudioLocationId)
                            .ToList();
                        results =
                            FindBy(s => s.StudioPhysicalLocationId != null &&
                                        studioLocationIds.Contains((int)s.StudioPhysicalLocationId))
                            .Select(x => x.StudioId)
                            .ToList();
                        break;
                }
            }

            return results;
        }
    }
}