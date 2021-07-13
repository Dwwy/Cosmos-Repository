using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Repository
{
    public class Repository<TItem> : IRepository<TItem> where TItem:IItem
    {
        private Container container;
        private int retry;
        public Repository(CosmosClient client, string databaseId, string containerId, int retryAttempts)
        {
            container = client.GetContainer(databaseId, containerId);
            retry = retryAttempts;
        }
        /// <summary>
        /// Query for multiple items from the container.
        /// </summary>
        /// <param name="query">A Query in String.</param>
        /// <param name="options">QueryRequestOptions for the query process</param>
        /// <param name="retryAttempt">Define number of retry attempts</param>
        /// <returns>
        /// Returns a list of items in Generic type.
        /// </returns>
        public async Task<List<TItem>> ReadMultipleByQuery(String query, QueryRequestOptions options, int retryAttempt = 0)
        {
            if (retryAttempt == 0)
            {
                retryAttempt = retry;
            }
            for (int i = 0; i< retryAttempt; i++)
            {
                try
                {
                    
                    FeedIterator<TItem> queryResult = container.GetItemQueryIterator<TItem>(new QueryDefinition(query), requestOptions: options);
                    List<TItem> menu = new List<TItem>();
                    while (queryResult.HasMoreResults)
                    {
                        FeedResponse<TItem> currentResultSet = await queryResult.ReadNextAsync();
                        Debug.WriteLine(currentResultSet.RequestCharge);
                        foreach (TItem m in currentResultSet)
                        {
                            m.retryattempt = i;
                            menu.Add(m);
                        }
                    }
                    
                    return menu;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return null;
        }
        /// <summary>
        /// Query for one item from the container.
        /// </summary>
        /// <param name="query">A Query in String.</param>
        /// <param name="options">QueryRequestOptions for the query process</param>
        /// <param name="retryAttempt">Define number of retry attempts</param>
        /// <returns>
        /// Returns an item in Generic Type
        /// </returns>
        public async Task<TItem> ReadOneByQuery(String query, QueryRequestOptions options, int retryAttempt = 0)
        {
            if (retryAttempt == 0)
            {
                retryAttempt = retry;
            }
            for (int i = 0; i < retryAttempt; i++)
            {
                try
                {
                    FeedIterator<TItem> queryResult = container.GetItemQueryIterator<TItem>(new QueryDefinition(query), requestOptions: options);
                    FeedResponse<TItem> current = await queryResult.ReadNextAsync();
                    Debug.WriteLine(current.RequestCharge);
                    List<TItem> menu = new List<TItem>();
                    menu.AddRange(current);

                    return menu[0];
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return default(TItem);
        }

        /// <summary>
        /// Reads an item asynchronously in the form of point read.
        /// </summary>
        /// <param name="id">The ID of the item to be read from the container</param>
        /// <param name="key">The Partition Key of the item that is to be read from the container</param>
        /// <param name="retryAttempt">Define number of retry attempts</param>
        /// <returns>
        /// Returns content in String format.
        /// </returns>
        public async Task<String> ReadItemStream(String id,PartitionKey key, int retryAttempt = 0)
        {
            if (retryAttempt == 0)
            {
                retryAttempt = retry;
            }
            for (int i = 0; i < retryAttempt; i++)
            {
                try
                {

                    using (ResponseMessage response = await container.ReadItemStreamAsync(id, key))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            Debug.WriteLine(response.Headers.RequestCharge);
                            Stream stream = response.Content;
                            StreamReader streamReader = new StreamReader(stream);
                            return streamReader.ReadToEnd();

                        }

                    }
                } 
                catch (Exception e)
                {
                    throw e;
                }
            }
            return null;

        }
        /// <summary>
        /// Read item asynchronously in the form of point read.
        /// </summary>
        /// <param name="id">The ID of the item to be read from the container.</param>
        /// <param name="key">The Partition Key of the item that is to be read from the container.</param>
        /// <param name="retryAttempt">Define number of retry attempts</param>
        /// <returns>
        /// Returns item in Generic type.
        /// </returns>
        public async Task<TItem> ReadItemAsync(String id, PartitionKey key, int retryAttempt = 0)
        {
            while (true)
            {
                try
                {
                    ItemResponse<TItem> response = await container.ReadItemAsync<TItem>(id, key);
                    Debug.WriteLine(response.RequestCharge);
                    if (response.Resource != null)
                    {
                        return response.Resource;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// Reads single or multiple item from the container using query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <param name="retryAttempt">Define number of retry attempts</param>
        /// <returns>
        /// Returns content in String format
        /// </returns>

        public async Task<String> ReadByStreamQuery(String query, QueryRequestOptions options, int retryAttempt = 0)
        {
            if (retryAttempt == 0)
            {
                retryAttempt = retry;
            }
            for (int i = 0; i < retryAttempt; i++)
            {
                try
                {
                    FeedIterator queryResult = container.GetItemQueryStreamIterator(new QueryDefinition(query), null, requestOptions: options);
                    ResponseMessage current = await queryResult.ReadNextAsync();
                    Stream stream = current.Content;
                    StreamReader streamReader = new StreamReader(stream);
                    String t = streamReader.ReadToEnd();
                    if (streamReader.ReadToEnd() != null || streamReader.ReadToEnd()!= "")
                    {
                        return t;
                    }

                }
                catch (Exception e)
                {
                    throw e;
                }
               
            }
            return null;
        }
        /// <summary>
        /// Insert the item specified by the parameter into the container.
        /// </summary>
        /// <param name="item">Generic Type object</param>
        /// <param name="retryAttempt">Define number of retry attempts</param>
        /// <returns>
        /// Returns a boolean indicating success or failure.
        /// </returns>
        public async Task<TItem> Create(TItem item, int retryAttempt = 0)
        {
            if (retryAttempt == 0)
            {
                retryAttempt = retry;
            }
            for (int i = 0; i < retryAttempt; i++)
            {
                try
                {
                    ItemResponse<TItem> itemResponse = await container.CreateItemAsync(item, item.PartitionKey);
                    Debug.WriteLine(itemResponse.RequestCharge);
                    return item;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return default(TItem);
        }
        /// <summary>
        /// Deletes an item with ID and Partition Key provided.
        /// </summary>
        /// <param name="id">The ID of the item to be read from the container.</param>
        /// <param name="key">The Partition Key of the item that is to be read from the container.</param>
        /// <returns>
        /// Returns a boolean indicating success or failure.
        /// </returns>
        public async Task<bool> Delete(String id, PartitionKey key)
        {
            try
            {
                ItemResponse<TItem> itemResponse = await container.DeleteItemAsync<TItem>(id, key);
                Debug.WriteLine(itemResponse.RequestCharge);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Updates an item.
        /// </summary>
        /// <remarks>
        /// ID and Partition Key must be constant during update operation or else a new item will be created.
        /// </remarks>
        /// <param name="item">Generic item type.</param>
        /// <returns>
        /// Returns a boolean indicating success or failure.
        /// </returns>
        public async Task<TItem> Update(TItem item)
        {
            try
            {
                ItemResponse<TItem> itemResponse = await container.UpsertItemAsync<TItem>(item, item.PartitionKey);
                Debug.WriteLine(itemResponse.RequestCharge);

                return item;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
