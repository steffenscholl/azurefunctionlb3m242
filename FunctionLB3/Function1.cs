using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FunctionLB3
{

    public class MyHttpTrigger
    {
        private readonly HttpClient _client;

        public MyHttpTrigger(IHttpClientFactory httpClientFactory)
        {
            this._client = httpClientFactory.CreateClient();
        }

        [FunctionName("MyHttpTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
            "https://api.github.com/repos/facebook/react/branches");
            request.Headers.Add("Accept", "application/vnd.github.v3+json");
            request.Headers.Add("User-Agent", "M242-IoTKitV3");
            var response =  _client.SendAsync(request);


            if (response.Result.IsSuccessStatusCode)
            {
                using var responseStream = response.Result.Content.ReadAsStreamAsync();

                var res = System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<GitHubBranch>>(responseStream.Result).Result;
                var s = new branchListsModel() { ListBranches = res };
                var responseLList = new List<string>();


                foreach (var item in res)
                {
                    responseLList.Add(item.Name);
                }


                return new OkObjectResult(responseLList);
            }
            return new OkObjectResult(response);
        }
    }
    public class GitHubBranch
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
    public class branchListsModel
    {
        public IEnumerable<GitHubBranch> ListBranches { get; set; }
    }
    //public static class Function1
    //{
    //    [FunctionName("Function1")]
    //    public static async Task<IActionResult> Run(
    //        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "getgit")] HttpRequest req,
    //        ILogger log)
    //    {
    //        log.LogInformation("C# HTTP trigger function processed a request.");

    //        string name = req.Query["name"];

    //        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    //        dynamic data = JsonConvert.DeserializeObject(requestBody);
    //        name = name ?? data?.name;

    //        string responseMessage = string.IsNullOrEmpty(name)
    //            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
    //            : $"Hello, {name}. This HTTP triggered function executed successfully.";

    //        return new OkObjectResult(responseMessage);
    //    }
    //}
}
