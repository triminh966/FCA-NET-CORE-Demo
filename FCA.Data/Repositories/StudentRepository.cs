using FCA.Data.Entities;
using System.Collections.Generic;

namespace FCA.Data.Repositories
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
    }

    public class StudentRepository : GenericRepository<PubSubContext, Student>, IStudentRepository
    {
        private readonly PubSubContext _pubSubContext;
        public StudentRepository(PubSubContext pubSubContext) : base(pubSubContext)
        {
            _pubSubContext = pubSubContext;
        }
    }
}