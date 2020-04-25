using System;
using System.Collections.Generic;
using System.Text;

namespace IlmoitusTaulu.Model
{
    using Microsoft.Azure.Cosmos.Table;
    public class DateTimeEntity : TableEntity
    {
        public DateTimeEntity()
        {
            PartitionKey = "rss";
            RowKey = "timestamp";
        }

        public DateTimeEntity(DateTime dateTime)
        {
            PartitionKey = "rss";
            RowKey = "timestamp";
        }
    }
}
