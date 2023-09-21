using System;
using Microsoft.AspNetCore.Http;
using UserApi.Models;

namespace UserApi.Middleware.ExceptionMiddleware
{
    public class NotFound : ExceptionModel
    {
        public NotFound()
        {
            ErrorCode = 1000;
            StatusCode = 404;
            Message = "User not found";
        }
    }

    public class BadRequest : ExceptionModel
    {
        public BadRequest()
        {
            ErrorCode = 1002;
            StatusCode = 400;
            Message = "You entered invalid or wrong entries";
        }
    }

    public class Conflict : ExceptionModel
    {
        public Conflict()
        {
            ErrorCode = 1003;
            StatusCode = 409;
            Message = "This username has been taken";
        }
    }

    public class NotAuthorized : ExceptionModel
    {
        public NotAuthorized()
        {
            ErrorCode = 1004;
            StatusCode = 401;
            Message = "You are not authorized";
        }
    }

    public class UnsupportedMediaType : ExceptionModel
    {
        public UnsupportedMediaType()
        {
            ErrorCode = 1005;
            StatusCode = 415;
            Message = "This media type does not supported";
        }
    }
}