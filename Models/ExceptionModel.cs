using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace UserApi.Models
{
    public abstract class ExceptionModel : Exception
    {
        public int StatusCode { set; get; }
        public new string Message { set; get; }
        public int ErrorCode { set; get; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new
            {
                statusCode = StatusCode,
                message = Message,
                errorCode = ErrorCode
            });
        }
    }
}