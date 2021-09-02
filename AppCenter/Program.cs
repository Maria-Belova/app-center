using AppCenter.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppCenter
{
    public class Program
    {
        private static HttpClient _client;

        private static readonly string OwnerName = ConfigurationManager.AppSettings["OwnerName"];
        private static readonly string AppName = ConfigurationManager.AppSettings["AppName"];
        private static readonly string ApiToken = ConfigurationManager.AppSettings["ApiToken"];

        private static readonly string BaseUrl = $"https://api.appcenter.ms/v0.1/apps/{OwnerName}/{AppName}";

        public static async Task Main(string[] args)
        {
            await RunAsync();
        }

        private static async Task RunAsync()
        {
            _client = new HttpClient();

            _client.DefaultRequestHeaders.Add("X-API-Token", ApiToken);

            List<BranchModel> branches = await ApiClient.GetDataListAsync<BranchModel>(_client, $"{BaseUrl}/branches");

            await DisplayBranchesAsync(branches);
        }

        private static async Task DisplayBranchesAsync(List<BranchModel> branches)
        {
            IEnumerable<string> branchesNames = branches.Select(x => x.branch.name);

            Console.WriteLine($"Actual branches: {String.Join(',', branchesNames)}");

            Console.WriteLine($"Enter '1' if you want to build all branches");

            string action = Console.ReadLine();

            if (action != "1")
            {
                return;
            }

            await BuildAllBranchesAsync(branches);
        }

        private static async Task BuildAllBranchesAsync(List<BranchModel> branches)
        {
            Console.WriteLine("Build started ...");

            List<Task> tasksList = new List<Task>();

            foreach (BranchModel branch in branches)
            {
                Task task = BuildBranchAsync(branch.branch.name, branch.branch.commit.sha);
                tasksList.Add(task);
            }

            await Task.WhenAll(tasksList);

            Console.WriteLine("All branches were build");
        }

        private static async Task<Task> BuildBranchAsync(string branchName, string sourceVersion)
        {
            string url = $"{BaseUrl}/branches/{branchName}/builds";

            BranchBuildResponse buildResponse = await ApiClient.PostDataAsync<BranchBuildResponse>(_client, url, sourceVersion);

            int createdBuildId = buildResponse.id;

            BranchBuildResponse completedResponse = await CheckBuildStatusAsync(createdBuildId);

            TimeSpan buildDuration = completedResponse.finishTime.Subtract(completedResponse.startTime);

            string logsUrl = await GetLogsLinkAsync(createdBuildId);

            Console.WriteLine($"{branchName} build {completedResponse.status}/{completedResponse.result} in {buildDuration.TotalSeconds} seconds. Link to build logs: {logsUrl}");

            Console.WriteLine("");

            return Task.CompletedTask;
        }

        private static async Task<BranchBuildResponse> CheckBuildStatusAsync(int id)
        {
            BranchBuildResponse buildResponses = await ApiClient.GetDataAsync<BranchBuildResponse>(_client, $"{BaseUrl}/builds/{id}");

            while (buildResponses.status != "completed")
            {
                await Task.Delay(10000);

                buildResponses = await ApiClient.GetDataAsync<BranchBuildResponse>(_client, $"{BaseUrl}/builds/{id}");
            }

            return buildResponses;
        }

        private static async Task<string> GetLogsLinkAsync(int id)
        {
            string url = $"{BaseUrl}/builds/{id}/downloads/logs";

            UrlModel logUrl = await ApiClient.GetDataAsync<UrlModel>(_client, url);

            return logUrl.uri;
        }
    }
}
