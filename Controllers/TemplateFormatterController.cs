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
    public class TemplateFormatterController : Controller
    {
        [HttpPost,
            CustomSwaggerData(Key = "summary", Value = "Format data with a Razor Template"),
            Produces("text/plain"),
            SwaggerResponse(200),
            SwaggerResponse(400),
            AllowAnonymous]
        public ActionResult Format(
            [FromBody, CustomSwaggerData(Key ="x-ms-summary", Value = "Format JSON using Razor Templates")]
            TemplateFormatRequest request)
        {
            var ecore = new EngineCore(new InMemoryTemplateManager(request.Template));
            var engine = new RazorLightEngine(ecore, new DefaultPageLookup(new DefaultPageFactory(p => ecore.KeyCompile(p))));

            if (request.Model == null)
                return new BadRequestObjectResult("Model is empty!");

            try
            {
                var result = engine.Parse("default", (dynamic)request.Model);
                return new OkObjectResult(result);
            }
            catch (TemplateParsingException ex)
            {
                string s = String.Concat(ex.ParserErrors.Select(p => p.ToString()));

                return new BadRequestObjectResult(s);
            }
            catch (TemplateCompilationException ex)
            {
                string s = String.Concat(ex.CompilationErrors.Select(p => p.ToString()));
                return new BadRequestObjectResult(s);
            }
        }
    }

    internal class InMemoryTemplateManager : ITemplateManager
    {
        private string _template;
        internal InMemoryTemplateManager(string template)
        {
            _template = template;
        }
        public ITemplateSource Resolve(string key)
        {
            return new LoadedTemplateSource(_template);
        }
    }
}