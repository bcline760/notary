using System.Net;

using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Notary.Logging;

namespace Notary.Api.Controllers
{
    [ApiController]
    public abstract class NotaryController : ControllerBase
    {
        protected ILog Log { get; }

        protected NotaryController(ILog log)
        {
            Log = log;
        }

        /// <summary>
        /// Execute a service method
        /// </summary>
        /// <typeparam name="TOut">The service method return type</typeparam>
        /// <param name="serviceMethod">A delegate for the service method to be executed</param>
        /// <param name="successCode">The HTTP status code used to return on success</param>
        /// <param name="failCode">The HTTP status code used to return on failure</param>
        /// <returns>An HTTP action result with message and status code</returns>
        protected async Task<IActionResult> ExecuteServiceMethod<TOut>(
            Func<Task<TOut>> serviceMethod,
            bool useHttpCreated = false
        ) where TOut : class
        {
            IActionResult result = null;

            try
            {
                var response = await serviceMethod();

                if (response == null)
                {
                    result = NotFound();
                }
                else if (useHttpCreated)
                {
                    result = CreatedAtAction(Request.Path, response);
                }
                else
                {
                    result = Ok(response);
                }
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                result = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return result;
        }

        /// <summary>
        /// Execute a service method
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut">The service method return type</typeparam>
        /// <param name="serviceMethod">A delegate for the service method to be executed</param>
        /// <param name="svcParam">The parameter to be passed into the service</param>
        /// <param name="successCode"></param>
        /// <param name="failCode"></param>
        /// <returns>An HTTP action result with message and status code</returns>
        protected async Task<IActionResult> ExecuteServiceMethod<TIn, TOut>(
            Func<TIn, Task<TOut>> serviceMethod,
            TIn svcParam,
            bool useHttpCreated = false) where TOut : class
        {
            IActionResult result = null;

            try
            {
                var response = await serviceMethod(svcParam);

                if (response == null)
                {
                    result = NotFound();
                }
                else if (useHttpCreated)
                {
                    result = CreatedAtAction(Request.Path, response);
                }
                else
                {
                    result = Ok(response);
                }
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                result = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return result;
        }

        /// <summary>
        /// Execute a service method
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="serviceMethod">A delegate for the service method to be executed</param>
        /// <param name="successCode"></param>
        /// <returns>An HTTP action result with message and status code</returns>
        protected async Task<IActionResult> ExecuteServiceMethod<TIn>(
            Func<TIn, Task> serviceMethod,
            TIn svcParam,
            bool useHttpCreated = false)
        {
            IActionResult result = null;

            try
            {
                await serviceMethod(svcParam);

                result = useHttpCreated ? Created(Request.Path, null) : Ok();
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                result = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return result;
        }

        /// <summary>
        /// Execute a service method
        /// </summary>
        /// <typeparam name="TIn">First argument type</typeparam>
        /// <typeparam name="TIn2">Second argument type</typeparam>
        /// <param name="serviceMethod">A delegate for the service method to be executed</param>
        /// <param name="param1">First argument of method</param>
        /// <param name="param2">Second argument of method</param>
        /// <param name="code">The HTTP status code to return upon completion</param>
        /// <returns></returns>
        protected async Task<IActionResult> ExecuteServiceMethod<TIn, TIn2>(
            Func<TIn, TIn2, Task> serviceMethod,
            TIn param1,
            TIn2 param2,
            bool useHttpCreated = false)
        {
            IActionResult result = null;

            try
            {
                await serviceMethod(param1, param2);

                result = useHttpCreated ? Created(Request.Path, null) : Ok();
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                result = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return result;
        }

        /// <summary>
        /// Execute a service method without a return asynchronously
        /// </summary>
        /// <typeparam name="TIn">First argument type</typeparam>
        /// <typeparam name="TIn2">Second argument type</typeparam>
        /// <param name="serviceMethod">A delegate for the service method to be executed</param>
        /// <param name="param1">First argument of method</param>
        /// <param name="param2">Second argument of method</param>
        /// <param name="param3">Third argument of method</param>
        /// <param name="code">The HTTP status code to return upon completion</param>
        /// <returns></returns>
        protected async Task<IActionResult> ExecuteServiceMethod<TIn, TIn2, TIn3>(
            Func<TIn, TIn2, TIn3, Task> serviceMethod,
            TIn param1,
            TIn2 param2,
            TIn3 param3,
            bool useHttpCreated = false)
        {
            IActionResult result = null;

            try
            {
                await serviceMethod(param1, param2, param3);
                result = useHttpCreated ? Created(Request.Path, null) : Ok();
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                result = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return result;
        }

        /// <summary>
        /// Execute a service method
        /// </summary>
        /// <typeparam name="TIn">The type used by the first service method parameter</typeparam>
        /// <typeparam name="TIn2">The type used by the second service method parameter</typeparam>
        /// <typeparam name="TOut">The service method return type</typeparam>
        /// <param name="serviceMethod">A delegate for the service method to be executed</param>
        /// <param name="svcParam">The first service method parameter</param>
        /// <param name="svcParam2">The second service method parameter</param>
        /// <param name="successCode">The HTTP status code used to return on success</param>
        /// <param name="failCode">The HTTP status code used to return on failure</param>
        /// <returns>An HTTP action result with message and status code</returns>
        protected async Task<IActionResult> ExecuteServiceMethod<TIn, TIn2, TOut>(
            Func<TIn, TIn2, Task<TOut>> serviceMethod,
            TIn svcParam,
            TIn2 svcParam2,
            bool useHttpCreated = false) where TOut : class
        {
            IActionResult result = null;

            try
            {
                var response = await serviceMethod(svcParam, svcParam2);
                if (response == null)
                {
                    result = NotFound();
                }
                else if (useHttpCreated)
                {
                    result = CreatedAtAction(Request.Path, response);
                }
                else
                {
                    result = Ok(response);
                }
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                result = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return result;
        }

        protected async Task<IActionResult> ExecuteServiceMethod<TIn, TIn2, TIn3, TOut>(
            Func<TIn, TIn2, TIn3, Task<TOut>> serviceMethod,
            TIn param1,
            TIn2 param2,
            TIn3 param3,
            bool useHttpCreated = false
        ) where TOut : class
        {
            IActionResult result = null;
            try
            {
                var response = await serviceMethod(param1, param2, param3);
                if (response == null)
                {
                    result = NotFound();
                }
                else if (useHttpCreated)
                {
                    result = CreatedAtAction(Request.Path, response);
                }
                else
                {
                    result = Ok(response);
                }
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                result = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return result;
        }
    }
}
