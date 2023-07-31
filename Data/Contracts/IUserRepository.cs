using System.Threading;
using System.Threading.Tasks;
using Entities.User;

namespace Data.Contracts
{
    public interface IUserRepository:IRepository<User>
    {
        Task<User> GetUserAndPass(string userName, string password, CancellationToken cancellationToken);
        Task AddAsync(User user,string password,CancellationToken cancellationToken);
        Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken);
    }
}