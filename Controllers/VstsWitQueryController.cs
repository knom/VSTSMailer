using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using VSTSWebApi.Helpers;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Linq.Dynamic.Core;

namespace VSTSWebApi.Controllers
{
    [Route("api/[controller]")]
    public class VstsWitQueryController : Controller
    {
        [HttpPost,
            Produces(typeof(object[])),
            SwaggerOperation("ExecuteWitQuery"),
            CustomSwaggerData(Key = "summary", Value = "Execute WIT query"),
            CustomSwaggerData(Key = "description", Value = "Execute WIT query grouped by field"),
            SwaggerResponse(200, typeof(object[])),
            SwaggerResponse(401)]
        public async Task<ActionResult> ExecuteWitQuery(
            [CustomSwaggerData(Key = "x-ms-summary", Value = "Full path of the query"),
             CustomSwaggerData(Key = "description", Value = "e.g. My Queries/My Team/Team Activities"),
             FromBody()]
            string queryName,
            //[CustomSwaggerData(Key = "x-ms-summary", Value = "VSTS User + Token"),
            // CustomSwaggerData(Key = "description", Value = "Base64 encoded 'username:PAT-token'"),
            // FromHeader]
            // string vstsPAT,
            [CustomSwaggerData(Key = "x-ms-summary", Value = "Enrich the result with the ISV partner field (TEDCOMEX.PARTNER)")]
             bool enrichPartnerField)
        {
            if (String.IsNullOrEmpty(queryName))
            {
                return BadRequest("QueryName cannot be empty!");
            }

            string authtoken = String.Empty;

            string authHeader = Request.Headers["Authorization"].SingleOrDefault();
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    authtoken = authHeaderVal.Parameter;
                }
            }
            if (authtoken == String.Empty)
            {
                return Unauthorized();
            }

            string queryEncoded = Uri.EscapeUriString(queryName);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authtoken);

            // Get the query Id
            try
            {
                string url = $"https://***REMOVED***.visualstudio.com/DefaultCollection/TED%20Commercial/_apis/wit/queries/{queryEncoded}?api-version=2.2";
                string queryJson = await client.GetStringAsync(url);
                dynamic query = JObject.Parse(queryJson);
                string queryId = query.id;

                string queryResultJson = await client.GetStringAsync($"https://***REMOVED***.visualstudio.com/DefaultCollection/TED%20Commercial/_apis/wit/wiql/{queryId}");
                dynamic queryResult = JObject.Parse(queryResultJson);

                if (queryResult.workItems == null && queryResult.workItemRelations == null)
                {
                    return Ok(new object[0]);
                }

                // FLAT QUERY
                if (queryResult.workItems != null)
                {
                    IEnumerable<dynamic> workItems = queryResult.workItems;
                    if (workItems.Count() == 0)
                    {
                        return Ok(new object[0]);
                    }

                    string workItemIds = String.Join(", ", workItems.Select(p => (int)p.id).Distinct());

                    string workItemResultJson = await client.GetStringAsync($"https://***REMOVED***.visualstudio.com/DefaultCollection/_apis/wit/workitems?ids={workItemIds}&api-version=1.0&$expand=relations");

                    IEnumerable<dynamic> workItemParsed = ((dynamic)JObject.Parse(workItemResultJson)).value;

                    if (enrichPartnerField)
                    {
                        workItemParsed = EnrichPartnerField(workItemParsed, authtoken);
                    }

                    var result = (workItemParsed).Select(p => JsonHelper.DeserializeAndFlatten(p));

                    return Ok((object)result);
                }

                // HIERARCHICAL QUERY
                if (queryResult.workItemRelations != null)
                {
                    IEnumerable<dynamic> workItemRelations = queryResult.workItemRelations;
                    IEnumerable<dynamic> relations = workItemRelations.Where(p => p.source != null && p.target != null);

                    if (relations.Count() == 0)
                    {
                        return Ok(new object[0]);
                    }

                    string workItemIds = String.Join(", ", relations.Select(p => (int)p.source.id).Concat((relations.Select(p => (int)p.target.id))).Distinct());

                    string workItemResultJson = await client.GetStringAsync($"https://***REMOVED***.visualstudio.com/DefaultCollection/_apis/wit/workitems?ids={workItemIds}&api-version=1.0");

                    dynamic workItemParsed = JObject.Parse(workItemResultJson);

                    //var result = ((IEnumerable<dynamic>)workItemParsed.value).Select(p => JsonHelper.DeserializeAndFlatten(p));
                    var dict = new Dictionary<int, dynamic>();
                    foreach (dynamic item in workItemParsed.value)
                    {
                        dict.Add((int)item.id, item);
                    }

                    var output = relations.Select(item =>
                    {
                        dynamic ni = new JObject();

                        int source = item.source.id;
                        int target = item.target.id;
                        ni.source = JObject.FromObject(JsonHelper.DeserializeAndFlatten(dict[source]));
                        ni.target = JObject.FromObject(JsonHelper.DeserializeAndFlatten(dict[target]));

                        return ni;
                    }).ToList();

                    return Ok(output);
                }
                return Ok();
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"Error fetching data from VSTS: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return BadRequest("Error parsing VSTS JSON! Check whether the PAT token is valid!");
            }

        }

        [HttpPost(),
            Route("Grouped"),
            Produces(typeof(object[])),
            SwaggerOperation("ExecuteWitQueryGroupedBy"),
            CustomSwaggerData(Key = "summary", Value = "Execute WIT query grouped by field"),
            CustomSwaggerData(Key = "description", Value = "Execute a stored WorkItem query grouped by a field"),
            SwaggerResponse(401),
            SwaggerResponse(200, typeof(object[]))]
        public async Task<ActionResult> ExecuteWitQueryGroupedBy(
            [CustomSwaggerData(Key = "x-ms-summary", Value = "Full path of the query"),
            CustomSwaggerData(Key = "description", Value = "e.g. My Queries/My Team/Team Activities"),
            FromBody()]
            string queryName,
            //[CustomSwaggerData(Key = "x-ms-summary", Value = "VSTS User + Token"),
            //CustomSwaggerData(Key = "description", Value = "Base64 encoded 'username:PAT-token'"),
            //FromHeader]
            //string vstsPAT,
            [CustomSwaggerData(Key = "x-ms-summary", Value = "Enrich the result with the ISV partner name (TEDCOMEX.PARTNER)")]
            bool enrichPartnerField,
            [CustomSwaggerData(Key = "x-ms-summary", Value = "Name of the field to group by"),
             CustomSwaggerData(Key = "description", Value = "e.g. TEDCOMEX.PARTNERNAME")]
            string groupByField)
        {
            var result = await ExecuteWitQuery(queryName, enrichPartnerField);

            if (result is OkObjectResult)
            {
                var r = (OkObjectResult)result;
                var val = r.Value as IEnumerable<dynamic>;

                var i = val.GroupBy(p => p[groupByField]);

                var re = i.Select(p => new
                {
                    Key = p.Key,
                    Values = p
                });

                return Ok(re);
            }

            return result;
        }


        private static IEnumerable<dynamic> EnrichPartnerField(IEnumerable<dynamic> items, string authToken)
        {
            var openParentIds = new Dictionary<int, dynamic>();

            foreach (var item in items)
            {
                var parentId = (string)((IEnumerable<dynamic>)item.relations).SingleOrDefault(p => p.rel == "System.LinkTypes.Hierarchy-Reverse")?.url;

                if (parentId != null)
                {
                    openParentIds[Int32.Parse((string)item.id)] = new
                    {
                        id = Int32.Parse(
                            Regex.Match(parentId,
                                "\\/_apis\\/wit\\/workItems\\/([0-9]*)").Groups[1].Value)

                    };
                }
            }

            var parents = LoadWitParentItems(openParentIds, authToken);

            foreach (var item in items)
            {
                int id = (int)item.id;
                if (parents.ContainsKey(id))
                {
                    item.fields["TEDCOMEX.PARTNERNAME"] = parents[id].fields["System.Title"];
                }
                else
                {
                    item.fields["TEDCOMEX.PARTNERNAME"] = "";
                }
            }
            return items;
        }

        private static Dictionary<int, dynamic> LoadWitParentItems(Dictionary<int, dynamic> openParentIds, string authToken)
        {
            var resolvedParentIds = new Dictionary<int, dynamic>();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            while (openParentIds.Count() > 0)
            {
                string workItemIds = String.Join(", ", openParentIds.Values.Select(p => (int)p.id).Distinct());

                string workItemResultJson = client.GetStringAsync($"https://***REMOVED***.visualstudio.com/DefaultCollection/_apis/wit/workitems?ids={workItemIds}&api-version=1.0&$expand=relations").Result;

                var workItemParsed = (IEnumerable<dynamic>)(((dynamic)JObject.Parse(workItemResultJson)).value);

                var isvs = workItemParsed.Where(p => p.fields["System.WorkItemType"] == "ISV");

                foreach (var isv in isvs)
                {
                    var list = openParentIds.Where(p => (int)p.Value.id == (int)isv.id).ToList();

                    foreach (var parents in list)
                    {
                        int key = parents.Key;
                        resolvedParentIds.Add(key, isv);
                        openParentIds.Remove(key);
                    }
                }

                var notIsvs = workItemParsed.Where(p => p.fields["System.WorkItemType"] != "ISV");

                foreach (var notIsv in notIsvs)
                {
                    var list = openParentIds.Where(p => (int)p.Value.id == (int)notIsv.id).ToList();

                    foreach (var parents in list)
                    {
                        int key = parents.Key;

                        var oldItem = openParentIds[key];

                        openParentIds[key] = new
                        {
                            id = Int32.Parse(
                                Regex.Match(
                                    (string)((IEnumerable<dynamic>)notIsv.relations).SingleOrDefault(p => p.rel == "System.LinkTypes.Hierarchy-Reverse")?.url,
                                    "\\/_apis\\/wit\\/workItems\\/([0-9]*)").Groups[1].Value)
                        };

                        if (oldItem.id == openParentIds[key].id)
                        {
                            // In a loop
                            // Remove the item
                            openParentIds.Remove(key);
                        }
                    }
                }
            }
            return resolvedParentIds;
        }

    }
}
