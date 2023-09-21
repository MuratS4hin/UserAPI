using System;
using System.Linq;
using FluentValidation;
using UserApi.Middleware.ExceptionMiddleware;
using UserApi.Models;
using UserApi.Services;


namespace UserApi.Validators
{
    public class RequestedUserValidator : AbstractValidator<RequestedUser>
    {
        private readonly UserService _userService;
        
        public RequestedUserValidator(UserService userService)
        {
            _userService = userService;
            
            RuleFor(user => user.Name)
                .NotEmpty()
                .Length(0, 30)
                .Matches("^[a-zA-ZİıĞÇçŞşÖöÜü ]")
                .OnFailure(user => throw new BadRequest());
            
            RuleFor(user => user.Password)
                .NotEmpty()
                .Length(0,13)
                .OnFailure(user => throw new BadRequest());

            RuleFor(user => user.Email).EmailAddress()
                .OnFailure(user => throw new BadRequest());
            
            RuleFor(user => user.Birthday)
                .Must(BeAValidDate)
                .OnFailure(user => throw new BadRequest());
            
            RuleFor(user => user.UsersRole)
                .Must(BeAValidPosition)
                .OnFailure(user => throw new BadRequest());

            RuleFor(user => user.Name)
                .Must(MongoUserNames)
                .OnFailure(user => throw new Conflict());
        }
        
        private bool BeAValidDate(string value)
        {
            return DateTime.TryParse(value, out _);
        }

        private bool BeAValidPosition(string value) 
            => value == "Admin" || value == "User";

        private bool MongoUserNames(string value) 
            => _userService.Find().All(user => user.Name != value);
        
    }
    
}