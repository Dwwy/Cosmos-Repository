using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IRepository<TItem>
    {
        /// <include file='docs.xml' path='doc/members/member[@name="M:Repository.Repository`1.ReadMultipleByQuery(System.String,Microsoft.Azure.Cosmos.QueryRequestOptions)"]/*'/>
        Task<List<TItem>> ReadMultipleByQuery(String query, QueryRequestOptions options,int retryAttempt = 0);
        /// <include file='docs.xml' path='doc/members/member[@name="M:Repository.Repository`1.ReadOneByQuery(System.String,Microsoft.Azure.Cosmos.QueryRequestOptions)"]/*'/>
        Task<TItem> ReadOneByQuery(String query, QueryRequestOptions options, int retryAttempt = 0);
        /// <include file='docs.xml' path='doc/members/member[@name="M:Repository.Repository`1.ReadItemStream(System.String,Microsoft.Azure.Cosmos.PartitionKey)"]/*'/>
        Task<String> ReadItemStream(String id, PartitionKey key, int retryAttempt = 0);
        /// <include file='docs.xml' path='doc/members/member[@name="M:Repository.Repository`1.ReadItemAsync(System.String,Microsoft.Azure.Cosmos.PartitionKey)"]/*'/>
        Task<TItem> ReadItemAsync(String id, PartitionKey key, int retryAttempt = 0);
        /// <include file='docs.xml' path='doc/members/member[@name="M:Repository.Repository`1.ReadByStreamQuery(System.String,Microsoft.Azure.Cosmos.QueryRequestOptions)"]/*'/>
        Task<String> ReadByStreamQuery(String query, QueryRequestOptions options, int retryAttempt = 0);
        /// <include file='docs.xml' path='doc/members/member[@name="M:Repository.Repository`1.Create(`0)"]/*'/>
        Task<TItem> Create(TItem item, int retryAttempt = 0);
        /// <include file='docs.xml' path='doc/members/member[@name="M:Repository.Repository`1.Delete(System.String,Microsoft.Azure.Cosmos.PartitionKey)"]/*'/>
        Task<bool> Delete(String id, PartitionKey key);
        /// <include file='docs.xml' path='doc/members/member[@name="M:Repository.Repository`1.Update(`0)"]/*'/>
        Task<TItem> Update(TItem item);

    }
}
