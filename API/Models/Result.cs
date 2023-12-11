using System;
namespace API.Models
{
        public class Result<T>
        {
                public T? Payload { get; set; }
                public string? ErrorMessage { get; set; }
                public bool IsSuccess { get; set; }

                public Result(T payload)
                {
                        Payload = payload;
                        IsSuccess = true;
                }

                public Result(string? errorMessage)
                {
                        ErrorMessage = errorMessage;
                        IsSuccess = false;
                }
        }
}

