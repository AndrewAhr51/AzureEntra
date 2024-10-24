using Azure.Identity;
using EntraIdBL.Helper;
using EntraIdBL.Interfaces;
using EntraIdBL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace EntraIdBL.Services
{
    public class RegionalMembers : IRegionalMembers
    {
        public string _accessToken;
        private string _clientId;
        private string _tenantId;
        private string _clientSecret;
        private string _sharePointUri;

        public RegionalMembers(string clientId, string tenantId, string clientSecret, string sharePointUri)
        {
            _clientId = clientId;
            _tenantId = tenantId;
            _clientSecret = clientSecret;
            _sharePointUri = sharePointUri;
        }

        public async Task<IActionResult> ProcessRegionalMembers(string baseUrl, string siteName, string listName, string regionName, List<EntraIdRecord> entraIdRecords)
        {
            var siteUrl = "https://neudesic.sharepoint.com" + siteName;

            try
            {
                var _accessToken = TokenHelper.GetAccessToken(_clientId, _tenantId, _clientSecret).Result;

                if (string.IsNullOrEmpty(_accessToken))
                {
                    return new StatusCodeResult(403);
                }

                TokenHelper.DecodeToken(_accessToken); // Decode the token to get the user's email address (upn) and the tenant id (tid

                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var clientSecretCredential = new ClientSecretCredential(
                                _tenantId, _clientId, _clientSecret);

                var _graphClient = new GraphServiceClient(clientSecretCredential);

                var site = await _graphClient.Sites[baseUrl + ":" + siteName]
                    .GetAsync();

                var lists = await _graphClient.Sites[site.Id]
                    .Lists
                    .GetAsync(requestConfiguration =>
                        requestConfiguration.QueryParameters.Filter = $"displayName eq '{listName}'");

                var neudesiconsultantsList = lists.Value?.FirstOrDefault();

                var itemsToDelete = new List<string>();
                if (neudesiconsultantsList.Items != null)
                {
                    foreach (var item in neudesiconsultantsList.Items)
                    {
                        var region = item.Fields.AdditionalData.ContainsKey("Region") ? item.Fields.AdditionalData["Region"]?.ToString() : null;
                        if (region == regionName)
                        {
                            itemsToDelete.Add(item.Id);
                        }
                    }

                    foreach (var itemId in itemsToDelete)
                    {
                        await _graphClient.Sites[siteUrl].Lists[neudesiconsultantsList.Id].Items[itemId].DeleteAsync();
                    }
                }

                foreach (var item in entraIdRecords)
                {
                    var newItem = new ListItem
                    {
                        Fields = new FieldValueSet
                        {
                            AdditionalData = new Dictionary<string, object>
                                {
                                    { "Region", item.Region },
                                    { "UserPrincipalName", item.UserPrincipalName },
                                    { "Title", item.DisplayName },
                                    { "JobTitle", item.JobTitle },
                                    { "EmailAddress", item.EmailAddress },
                                    { "OfficeLocation", item.OfficeLocation },
                                    { "Department", item.Department },
                                    { "Manager", item.Manager }
                                }
                        }
                    };
                    
                    await _graphClient.Sites[site.Id].Lists[neudesiconsultantsList.Id].Items
                        .PostAsync(newItem);
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(403);
            }
        }
    }
}
