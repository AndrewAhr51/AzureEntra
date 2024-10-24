using EntraIdBL.Helper;
using EntraIdBL.Interfaces;
using EntraIdBL.Models;
using Microsoft.Graph;
using Azure.Identity;
using Microsoft.Graph.Models;

namespace EntraIdBL.Services
{
    public class GraphClient : IGraphClient
    {
        public string _accessToken;
        private string _clientId;
        private string _tenantId;
        private string _clientSecret;
        private string _sharePointUri;

        public GraphClient(string clientId, string tenantId, string clientSecret, string sharePointUri)
        {
            _clientId = clientId;
            _tenantId = tenantId;
            _clientSecret = clientSecret;
            _sharePointUri = sharePointUri;

            _accessToken = TokenHelper.GetAccessToken(_clientId, _tenantId, _clientSecret).Result;
        }

        public async Task<List<EntraIdRecord>> GetGroupMembers(string _region, string _groupId)
        {
            List<EntraIdRecord> activeAccounts = new List<EntraIdRecord>();

            try
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var clientSecretCredential = new ClientSecretCredential(
                                _tenantId, _clientId, _clientSecret);
                var _graphClient = new GraphServiceClient(clientSecretCredential, scopes);

                var groupMembers = await _graphClient.Groups[_groupId].Members.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new[] { "userPrincipalName", "displayName", "jobTitle", "mail", "officeLocation", "department" };
                    requestConfiguration.QueryParameters.Top = 999;
                });

                if (groupMembers != null)
                {
                    foreach (var member in groupMembers.Value)
                    {
                        if (member is User user)
                        {
                            var currentMember = await _graphClient.Users[user.UserPrincipalName].GetAsync(requestConfiguration1 =>
                            {
                                requestConfiguration1.QueryParameters.Select = new[] { "userPrincipalName", "accountEnabled" };
                            });

                            if (currentMember.AccountEnabled == true)
                            {
                                var managerDisplayName = string.Empty;
                                var manager = await _graphClient.Users[user.UserPrincipalName].Manager.GetAsync();
                                var managerData = manager as User;
                                if (managerData != null)
                                {
                                    managerDisplayName = managerData.DisplayName;
                                }
                                
                                activeAccounts.Add(new EntraIdRecord
                                {
                                    UserPrincipalName = user.UserPrincipalName,
                                    Region = _region,
                                    DisplayName = user.DisplayName,
                                    JobTitle = user.JobTitle,
                                    EmailAddress = user.Mail,
                                    OfficeLocation = user.OfficeLocation,
                                    Department = user.Department,
                                    Manager = managerDisplayName
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return activeAccounts;
        }


        public async Task<List<EntraIdRecord>> InsertNeudesicConsultants(string _region, string _groupId)
        {
            List<EntraIdRecord> activeAccounts = new List<EntraIdRecord>();

            try
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var clientSecretCredential = new ClientSecretCredential(
                                _tenantId, _clientId, _clientSecret);
                var _graphClient = new GraphServiceClient(clientSecretCredential, scopes);

                var groupMembers = await _graphClient.Groups[_groupId].Members.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new[] { "userPrincipalName", "displayName", "jobTitle", "mail", "officeLocation", "department" };
                    requestConfiguration.QueryParameters.Top = 999;
                });

                if (groupMembers != null)
                {
                    foreach (var member in groupMembers.Value)
                    {
                        if (member is User user)
                        {
                            var currentMember = await _graphClient.Users[user.UserPrincipalName].GetAsync(requestConfiguration1 =>
                            {
                                requestConfiguration1.QueryParameters.Select = new[] { "userPrincipalName", "accountEnabled" };
                            });

                            if (currentMember.AccountEnabled == true)
                            {
                                activeAccounts.Add(new EntraIdRecord
                                {
                                    UserPrincipalName = user.UserPrincipalName,
                                    Region = _region,
                                    DisplayName = user.DisplayName,
                                    JobTitle = user.JobTitle,
                                    EmailAddress = user.Mail,
                                    OfficeLocation = user.OfficeLocation,
                                    Department = user.Department
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return activeAccounts;
        }


        public async Task<List<EntraIdRecord>> GetGroupMembersWithManager(string groupId)
        {
            List<EntraIdRecord> activeAccounts = new List<EntraIdRecord>();

            try
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var clientSecretCredential = new ClientSecretCredential(
                                _tenantId, _clientId, _clientSecret);
                var _graphClient = new GraphServiceClient(clientSecretCredential, scopes);

                var groupMembers = await _graphClient.Groups[groupId].Members.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new[] { "userPrincipalName", "displayName", "jobTitle", "mail", "officeLocation", "department" };
                    requestConfiguration.QueryParameters.Top = 999;
                });

                if (groupMembers != null)
                {
                    foreach (var member in groupMembers.Value)
                    {
                        if (member is User user)
                        {
                            var currentMember = await _graphClient.Users[user.UserPrincipalName].GetAsync(requestConfiguration1 =>
                            {
                                requestConfiguration1.QueryParameters.Select = new[] { "userPrincipalName", "accountEnabled" };
                                requestConfiguration1.QueryParameters.Expand = new[] { "manager($select=displayName)" };
                            });
                            var managerDisplayName = string.Empty;

                            if (currentMember.AccountEnabled == true)
                            {
                                var managerData = currentMember.AdditionalData["manager"] as IDictionary<string, object>;
                                if (managerData != null && managerData.ContainsKey("displayName"))
                                {
                                    managerDisplayName = managerData["displayName"].ToString();
                                }

                                activeAccounts.Add(new EntraIdRecord
                                {
                                    UserPrincipalName = user.UserPrincipalName,
                                    DisplayName = user.DisplayName,
                                    JobTitle = user.JobTitle,
                                    EmailAddress = user.Mail,
                                    OfficeLocation = user.OfficeLocation,
                                    Department = user.Department,
                                    Manager = managerDisplayName
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return activeAccounts;
        }
    }
}

