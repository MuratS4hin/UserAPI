using UserApi.Models;
using UserApi.Repository;
using UserApi.Settings;

namespace UserApi.Services
{
    public class UserService : BaseServiceRepository<User>
    {
        
        public UserService(DBContext context, IUserDatabaseSettings settings) : base(context, settings.CollectionName)
        {
        }
    }
}