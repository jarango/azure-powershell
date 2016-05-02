﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.Batch.Properties;
using Microsoft.Azure.Management.Batch;
using Microsoft.Azure.Management.Batch.Models;
using Microsoft.Azure.Management.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Batch;
using Microsoft.Rest.Azure;
using CloudException = Hyak.Common.CloudException;

namespace Microsoft.Azure.Commands.Batch.Models
{
    public partial class BatchClient
    {
        /// <summary>
        /// Creates a new Batch account
        /// </summary>
        /// <param name="resourceGroupName">The name of the resource group in which to create the account</param>
        /// <param name="accountName">The account name</param>
        /// <param name="location">The location to use when creating the account</param>
        /// <param name="tags">The tags to associate with the account</param>
        /// <param name="storageId">The resource id of the storage account to be used for auto storage.</param>
        /// <returns>A BatchAccountContext object representing the new account</returns>
        public virtual BatchAccountContext CreateAccount(string resourceGroupName, string accountName, string location, Hashtable[] tags, string storageId)
        {
            // use the group lookup to validate whether account already exists. We don't care about the returned
            // group name nor the exception
            if (GetGroupForAccountNoThrow(accountName) != null)
            {
                throw new CloudException(Resources.AccountAlreadyExists);
            }

            Dictionary<string, string> tagDictionary = Helpers.CreateTagDictionary(tags, validate: true);

            var response = BatchManagementClient.Account.Create(resourceGroupName, accountName, new BatchAccountCreateParameters()
            {
                Location = location,
                Tags = tagDictionary,
                AutoStorage = new AutoStorageBaseProperties()
                {
                    StorageAccountId = storageId
                }
            });

            var context = BatchAccountContext.ConvertAccountResourceToNewAccountContext(response);
            return context;
        }

        /// <summary>
        /// Updates an existing Batch account
        /// </summary>
        /// <param name="resourceGroupName">The name of the resource group the account is under. If unspecified, it will be looked up.</param>
        /// <param name="accountName">The account name</param>
        /// <param name="tags">New tags to associate with the account</param>
        /// <param name="storageId"></param>
        /// <returns>A BatchAccountContext object representing the updated account</returns>
        public virtual BatchAccountContext UpdateAccount(string resourceGroupName, string accountName, Hashtable[] tags, string storageId)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                // use resource mgr to see if account exists and then use resource group name to do the actual lookup
                resourceGroupName = GetGroupForAccount(accountName);
            }

            Dictionary<string, string> tagDictionary = Helpers.CreateTagDictionary(tags, validate: true);

            // need to the location in order to call
            var getResponse = BatchManagementClient.Account.Get(resourceGroupName, accountName);

            var response = BatchManagementClient.Account.Create(resourceGroupName, accountName, new BatchAccountCreateParameters()
            {
                Location = getResponse.Location,
                Tags = tagDictionary,
                AutoStorage = new AutoStorageBaseProperties()
                {
                    StorageAccountId = storageId
                }
            });

            BatchAccountContext context = BatchAccountContext.ConvertAccountResourceToNewAccountContext(response);

            return context;
        }

        /// <summary>
        /// Get details about the Batch account
        /// </summary>
        /// <param name="resourceGroupName">The name of the resource group the account is under. If unspecified, it will be looked up.</param>
        /// <param name="accountName">The account name</param>
        /// <returns>A BatchAccountContext object representing the account</returns>
        public virtual BatchAccountContext GetAccount(string resourceGroupName, string accountName)
        {
            // single account lookup - find its resource group if not specified
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetGroupForAccount(accountName);
            }
            var response = BatchManagementClient.Account.Get(resourceGroupName, accountName);

            return BatchAccountContext.ConvertAccountResourceToNewAccountContext(response);
        }

        /// <summary>
        /// Gets the keys associated with the Batch account
        /// </summary>
        /// <param name="resourceGroupName">The name of the resource group the account is under. If unspecified, it will be looked up.</param>
        /// <param name="accountName">The account name</param>
        /// <returns>A BatchAccountContext object with the account keys</returns>
        public virtual BatchAccountContext ListKeys(string resourceGroupName, string accountName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                // use resource mgr to see if account exists and then use resource group name to do the actual lookup
                resourceGroupName = GetGroupForAccount(accountName);
            }

            var context = GetAccount(resourceGroupName, accountName);
            var keysResponse = BatchManagementClient.Account.ListKeys(resourceGroupName, accountName);
            context.PrimaryAccountKey = keysResponse.Primary;
            context.SecondaryAccountKey = keysResponse.Secondary;

            return context;
        }

        /// <summary>
        /// Lists all accounts in a subscription or in a resource group if its name is specified
        /// </summary>
        /// <param name="resourceGroupName">The name of the resource group to search under for accounts. If unspecified, all accounts will be looked up.</param>
        /// <param name="tag">The tag to filter accounts on</param>
        /// <returns>A collection of BatchAccountContext objects</returns>
        public virtual IEnumerable<BatchAccountContext> ListAccounts(Hashtable tag, string resourceGroupName = default(string))
        {
            List<BatchAccountContext> accounts = new List<BatchAccountContext>();

            // no account name so we're doing some sort of list. If no resource group, then list all accounts under the
            // subscription otherwise all accounts in the resource group.
            var response = string.IsNullOrEmpty(resourceGroupName)
                ? BatchManagementClient.Account.List()
                : BatchManagementClient.Account.ListByResourceGroup(resourceGroupName);

            // filter out the accounts if a tag was specified
            IList<AccountResource> accountResources = new List<AccountResource>();
            if (tag != null && tag.Count > 0)
            {
                accountResources = Helpers.FilterAccounts(response, tag);
            }
            else
            {
                accountResources = response.ToList();
            }

            foreach (AccountResource resource in accountResources)
            {
                accounts.Add(BatchAccountContext.ConvertAccountResourceToNewAccountContext(resource));
            }

            var nextLink = response.NextPageLink;

            while (nextLink != null)
            {
                response = ListNextAccounts(nextLink);

                foreach (AccountResource resource in response)
                {
                    accounts.Add(BatchAccountContext.ConvertAccountResourceToNewAccountContext(resource));
                }

                nextLink = response.NextPageLink;
            }

            return accounts;
        }

        /// <summary>
        /// Generates new key for the Batch account
        /// </summary>
        /// <param name="resourceGroupName">The name of the resource group the account is under. If unspecified, it will be looked up.</param>
        /// <param name="accountName">The account name</param>
        /// <param name="keyType">The type of key to regenerate</param>
        /// <returns>The BatchAccountContext object with the regenerated keys</returns>
        public virtual BatchAccountContext RegenerateKeys(string resourceGroupName, string accountName, AccountKeyType keyType)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                // use resource mgr to see if account exists and then use resource group name to do the actual lookup
                resourceGroupName = GetGroupForAccount(accountName);
            }

            // build a new context to put the keys into
            var context = GetAccount(resourceGroupName, accountName);

            var regenResponse = BatchManagementClient.Account.RegenerateKey(resourceGroupName, accountName, new BatchAccountRegenerateKeyParameters
            {
                KeyName = keyType
            });

            context.PrimaryAccountKey = regenResponse.Primary;
            context.SecondaryAccountKey = regenResponse.Secondary;
            return context;
        }

        /// <summary>
        /// Deletes the specified account
        /// </summary>
        /// <param name="resourceGroupName">The name of the resource group the account is under. If unspecified, it will be looked up.</param>
        /// <param name="accountName">The account name</param>
        /// <returns>The status of delete account operation</returns>
        public virtual Task<Rest.Azure.AzureOperationResponse> DeleteAccount(string resourceGroupName, string accountName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                // use resource mgr to see if account exists and then use resource group name to do the actual lookup
                resourceGroupName = GetGroupForAccount(accountName);
            }
            return BatchManagementClient.Account.DeleteWithHttpMessagesAsync(resourceGroupName, accountName);
        }

        /// <summary>
        /// Lists the node agent SKUs matching the specified filter options.
        /// </summary>
        /// <param name="context">The account to use.</param>
        /// <param name="filterClause">The level of detail</param>
        /// <param name="maxCount">The number of results.</param>
        /// <param name="additionalBehaviors">Additional client behaviors to perform.</param>
        /// <returns>The node agent SKUs matching the specified filter.</returns>
        public IEnumerable<PSNodeAgentSku> ListNodeAgentSkus(
            BatchAccountContext context,
            string filterClause = default(string),
            int maxCount = default(int),
            IEnumerable<BatchClientBehavior> additionalBehaviors = null)
        {
            PoolOperations poolOperations = context.BatchOMClient.PoolOperations;
            ODATADetailLevel filterLevel = new ODATADetailLevel(filterClause: filterClause);

            IPagedEnumerable<NodeAgentSku> nodeAgentSkus = poolOperations.ListNodeAgentSkus(filterLevel, additionalBehaviors);
            Func<NodeAgentSku, PSNodeAgentSku> mappingFunction = p => { return new PSNodeAgentSku(p); };

            return PSPagedEnumerable<PSNodeAgentSku, NodeAgentSku>.CreateWithMaxCount(nodeAgentSkus, mappingFunction,
                maxCount, () => WriteVerbose(string.Format(Resources.MaxCount, maxCount)));
        }

        /// <summary>
        /// Lists all accounts in a subscription or in a resource group if its name is specified
        /// </summary>
        /// <param name="NextLink">Next link to use when querying for accounts</param>
        /// <returns>The status of list operation</returns>
        internal IPage<AccountResource> ListNextAccounts(string NextLink)
        {
            return BatchManagementClient.Account.ListNext(NextLink);
        }

        internal string GetGroupForAccountNoThrow(string accountName)
        {
            var response = ResourceManagementClient.Resources.List(new Management.Resources.Models.ResourceListParameters()
            {
                ResourceType = accountSearch
            });

            string groupName = null;

            foreach (var res in response.Resources)
            {
                if (res.Name == accountName)
                {
                    groupName = ExtractResourceGroupName(res.Id);
                }
            }

            return groupName;
        }

        internal string GetGroupForAccount(string accountName)
        {
            var groupName = GetGroupForAccountNoThrow(accountName);
            if (groupName == null)
            {
                throw new CloudException(Resources.ResourceNotFound);
            }

            return groupName;
        }

        private string ExtractResourceGroupName(string id)
        {
            var idParts = id.Split('/');
            if (idParts.Length < 4)
            {
                throw new CloudException(String.Format(Resources.MissingResGroupName, id));
            }

            return idParts[4];
        }
    }
}
