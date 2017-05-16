using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RazorLight;
using RazorLight.Templating;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using VSTSWebApi.Helpers;
using VSTSWebApi.Models;

namespace VSTSWebApi.Controllers
{
    [Route("api/[controller]")]
    public class TemplateHelperController : Controller
    {
        [HttpPost,
            Route("User"),
            CustomSwaggerData(Key = "summary", Value = "Parse User to JSON"),
            CustomSwaggerData(Key = "description", Value = "Parse user to JSON"),
            Produces("text/json"),
            SwaggerResponse(200,typeof(UserInfo)),
            AllowAnonymous]
        public ActionResult ParseUser(
            [FromBody, 
            CustomSwaggerData(Key = "x-ms-summary", Value = "User Info String"),
            CustomSwaggerData(Key = "description", Value = "e.g. 'Max Musterman <asd@asd.com>'"),
            ]
            string userJson)
        {
            var user = new UserInfo
            {
                Name = RazorHelpers.GetShortUser(userJson),
                EMail = RazorHelpers.GetUserEmail(userJson)
            };

            return Ok(user);
        }
    }
}