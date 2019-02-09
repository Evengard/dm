using System.Threading.Tasks;
using DM.Web.Core.Authentication.Credentials;
using Microsoft.AspNetCore.Http;

namespace DM.Web.Core.Authentication
{
    public interface IWebAuthenticationService
    {
        Task Authenticate(HttpContext httpContext);
        Task Authenticate(LoginCredentials credentials, HttpContext httpContext);
        Task Logout(HttpContext httpContext);
        Task LogoutAll(HttpContext httpContext);
    }
}