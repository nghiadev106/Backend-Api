using Data.Infrastructure;
using Models.Models;

namespace Data.Repositories
{
    public interface IAnnouncementUserRepository : IRepository<AnnouncementUser>
    {
    }

    public class AnnouncementUserRepository : RepositoryBase<AnnouncementUser>, IAnnouncementUserRepository
    {
        public AnnouncementUserRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}