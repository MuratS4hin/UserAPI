using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserApi.Middleware;
using UserApi.Models;
using UserApi.Services;


namespace UserApi.Controllers
{
    
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }
        

        [HttpGet]
        public List<User> Get()
        {
            var response = _userService.Find();
            return response;
        }
        
        [HttpGet("{id:Guid}")]
        public async Task<User> GetById(Guid id)
        {
            if (HttpContext.Connection.Id.Equals(null) || id.GetType() != typeof(Guid)) throw new BadRequest();
            var user = _userService.FindById(x => x.Id == id);
            if (user == null) throw new NotFound();
            return user;
        }
        
        [HttpPost]
        public ActionResult<User> Create([FromBody]RequestedUser requestedUser)
        {
            return _userService.Create(requestedUser.SetRequestedUserToUser());
        }

        [HttpPost("Many")]
        public List<RequestedUser> CreateMany([FromBody]List<RequestedUser> requestedUser)
        {
            foreach (var variable in requestedUser)
                _userService.Create(variable.SetRequestedUserToUser());
            return requestedUser;
        }
        
        [HttpPut("{id:Guid}")]
        public RequestedUser Update(Guid id, [FromBody] RequestedUser newValue)
        {
            if (newValue.Name == null && newValue.Birthday == null && newValue.Email == null && id  == null && newValue.Password == null)
                throw new BadRequest();
            
            if(newValue.Name != null)
                _userService.Update(user => user.Id == id, user => user.Name , newValue.Name);
            if (newValue.Password != null)
                _userService.Update(user => user.Id == id, user => user.Password , newValue.Password);
            if (newValue.Birthday != null)
                _userService.Update(user => user.Id == id, user => user.Birthday , newValue.Birthday);
            if (newValue.Email != null)
                _userService.Update(user => user.Id == id, user => user.Email , newValue.Email);
            
            if (newValue.Email != null || newValue.Birthday != null || newValue.Name != null || newValue.Password != null)
                _userService.Update(user => user.Id == id, user => user.UpdatedAt , DateTime.Now);
            
            return newValue;
        }
        
        [HttpPut("Replace/{id:Guid}")]
        public User ReplaceOne( Guid id, [FromBody]RequestedUser requestedUser)
        {
            return _userService.ReplaceOne(requestedUser.SetRequestedUserToUser(), user => user.Id == id);
        }

        [HttpDelete("{id:Guid}")]
        public User Delete(Guid id)
        {
            return _userService.Delete(user => user.Id == id);
        }
    }
}