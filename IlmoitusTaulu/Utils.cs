using IlmoitusTaulu.Model;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IlmoitusTaulu
{
    public class Utils
    {
        public static async Task<DateTimeEntity> InsertTimeStamp(CloudTable table, DateTimeEntity entity)
        {
            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                DateTimeEntity insertedDateTime = result.Result as DateTimeEntity;

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure Cosmos DB
                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return insertedDateTime;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public static async Task<DateTimeEntity> RetrieveEntityUsingPointQueryAsync(CloudTable table)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<DateTimeEntity>("rss", "timestamp");
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                DateTimeEntity dateTime = result.Result as DateTimeEntity;
                if (dateTime != null)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}", dateTime.PartitionKey, dateTime.RowKey, dateTime.Timestamp);
                }

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return dateTime;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

    }
}
