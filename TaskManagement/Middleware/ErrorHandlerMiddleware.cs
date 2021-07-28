using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TaskManagement.Helpers;

namespace TaskManagement.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                //ошибки отлавливаются (в браузере в инспекторе приходят ошибки),
                //но на представлении все запросы висят через data-ajax-update,
                //и почему-то в таком случае Response.WriteAsync ничего не пишет туда,
                //где должно было быть частичное представление
                var response = context.Response;
                response.ContentType = "application/json";
                
                switch (error)
                {
                    case AppException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var serializedException = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Message = error.Message,
                });
                await response.WriteAsync(serializedException);
            }
        }
    }
}
