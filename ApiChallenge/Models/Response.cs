using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace ApiChallenge.Models
{
    public class Response<T>
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public Dictionary<string, string[]> Errors { get; set; }

        public T Data { get; set; }
        public Response(string code, string message)
        {
            this.Code = code;
            Message = message;
        }
        public Response(string code, string message, T data)
        {
            this.Code = code;
            Message = message;
            Data = data;
        }

        public Response(string code, string message, T data, ModelStateDictionary modelState)
        {
            this.Code = code;
            Message = message;
            Data = data;
            Errors = modelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
        }
    }
}