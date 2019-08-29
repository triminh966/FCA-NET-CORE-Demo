using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FCA.Data.Entities.FCA;

namespace FCA.Data.Entities
{
    public sealed class ChallengeCategory
    {
        public ChallengeCategory()
        {
            Challenge = new HashSet<Challenge>();
        }
        
        public int ChallengeCategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string ContentUrl { get; set; }

        public ICollection<Challenge> Challenge { get; set; }
    }
}
