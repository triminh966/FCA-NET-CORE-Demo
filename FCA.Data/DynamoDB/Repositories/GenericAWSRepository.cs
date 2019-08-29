using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using FCA.Core.Secrets;
using System.Collections.Generic;

namespace FCA.Data.DynamoDB.Repositories
{
    public interface IGenericAWSRepository<TEntity> where TEntity : class
    {
        TEntity Get(object uuid, object rangeKey = null);
        IEnumerable<TEntity> GetAll();
        bool Delete(TEntity entity);
        void Delete(List<TEntity> listEntities);
        void AddOrUpdate(TEntity entity);
        void AddOrUpdate(List<TEntity> listEntities);
    }

    public abstract class GenericAWSRepository<TEntity> : IGenericAWSRepository<TEntity> where TEntity : class
    {
        private readonly IFcaSecrets _fcaSecrets;

        public GenericAWSRepository(IFcaSecrets fcaSecrets)
        {
            _fcaSecrets = fcaSecrets;
        }

        public virtual void AddOrUpdate(List<TEntity> listEntities)
        {
            using (var client = new AmazonDynamoDBClient(_fcaSecrets.AwsCredentials, _fcaSecrets.AwsRegion))
            using (var context = new DynamoDBContext(client))
            {
                var entryBatch = context.CreateBatchWrite<TEntity>(_fcaSecrets.DefaultDbOperationConfig);
                entryBatch.AddPutItems(listEntities);
                entryBatch.ExecuteAsync();
            }
        }

        public virtual void AddOrUpdate(TEntity entity)
        {
            using (var client = new AmazonDynamoDBClient(_fcaSecrets.AwsCredentials, _fcaSecrets.AwsRegion))
            using (var context = new DynamoDBContext(client, _fcaSecrets.DefaultDbOperationConfig))
            {
                context.SaveAsync(entity);
            }
        }

        public virtual bool Delete(TEntity entity)
        {
            using (var client = new AmazonDynamoDBClient(_fcaSecrets.AwsCredentials, _fcaSecrets.AwsRegion))
            using (var context = new DynamoDBContext(client, _fcaSecrets.DefaultDbOperationConfig))
            {
                context.DeleteAsync(entity);
                if (context.LoadAsync(entity) != null) return false;
                return true;
            }
        }

        public virtual void Delete(List<TEntity> listEntities)
        {
            using (var client = new AmazonDynamoDBClient(_fcaSecrets.AwsCredentials, _fcaSecrets.AwsRegion))
            using (var context = new DynamoDBContext(client))
            {
                var entryBatch = context.CreateBatchWrite<TEntity>(_fcaSecrets.DefaultDbOperationConfig);
                entryBatch.AddDeleteItems(listEntities);
                entryBatch.ExecuteAsync();
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            List<TEntity> result;
            using (var client = new AmazonDynamoDBClient(_fcaSecrets.AwsCredentials, _fcaSecrets.AwsRegion))
            using (var context = new DynamoDBContext(client))
            {
                var scanConfig = new ScanOperationConfig();
                result = context.FromScanAsync<TEntity>(scanConfig, _fcaSecrets.DefaultDbOperationConfig).GetRemainingAsync().Result;
            }

            return result;
        }

        public virtual TEntity Get(object uuid, object rangeKey = null)
        {
            TEntity result;
            using (var client = new AmazonDynamoDBClient(_fcaSecrets.AwsCredentials, _fcaSecrets.AwsRegion))
            using (var context = new DynamoDBContext(client, _fcaSecrets.DefaultDbOperationConfig))
            {
                result = context.LoadAsync<TEntity>(uuid, rangeKey).Result;
            }
            return result;
        }
    }
}
