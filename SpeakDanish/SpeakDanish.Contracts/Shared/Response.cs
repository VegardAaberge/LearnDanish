using System;
using System.Xml.Linq;

namespace SpeakDanish.Contracts.Shared
{
	public class Response
	{
        public Response()
        {
        }

        public Response(bool success, string? message = null)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class Response<T> : Response
    {
        public Response()
        {
        }

        public Response(bool success, string? message = null, T? data = default(T)) : base(success, message)
        {
            Data = data;
        }

        public T? Data { get; set; }
    }
}

