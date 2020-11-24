using UserApi.Models;
using UserApi.Repository;
using UserApi.Settings;

namespace UserApi.Services
{
    public class UserService : BaseRepository<User>
    {
        
        public UserService(Context context, IUserDatabaseSettings settings) : base(context, settings.CollectionName)
        {
        }
    }
}