﻿// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Microsoft.WindowsAzure.Storage;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.AzureTableStorage;

namespace Serilog
{
    public class BatchOptions
    {
        public int BatchSizeLimit { get; set; }
        public TimeSpan FlushPeriod { get; set; }
        public static BatchOptions NotBatchedOptions { get { return null; } }
        public static BatchOptions Default { get { return new BatchOptions(); } }

    }
    /// <summary>
    /// Adds the WriteTo.AzureTableStorage() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationAzureTableStorageExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events as records in the 'LogEventEntity' Azure Table Storage table in the given storage account.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="storageAccount">The Cloud Storage Account to use to insert the log entries to.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="storageTableName">Table name that log entries will be written to. Note: Optional, setting this may impact performance</param>
        /// <param name="batchOptions"></param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration AzureTableStorage(
            this LoggerSinkConfiguration loggerConfiguration,
            CloudStorageAccount storageAccount,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            string storageTableName = null,
            BatchOptions batchOptions = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (storageAccount == null) throw new ArgumentNullException("storageAccount");

            if (batchOptions == null)
            {
                return loggerConfiguration.Sink(new AzureTableStorageSink(storageAccount, formatProvider, storageTableName), restrictedToMinimumLevel);
            }
            else
            {
                var azureBatchingTableStorageSink = new AzureBatchingTableStorageSink(storageAccount, formatProvider, storageTableName, batchOptions.BatchSizeLimit, batchOptions.FlushPeriod);
                return loggerConfiguration.Sink(azureBatchingTableStorageSink, restrictedToMinimumLevel);
            }
        }

        /// <summary>
        /// Adds a sink that writes log events as records in the 'LogEventEntity' Azure Table Storage table in the given storage account.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="connectionString">The Cloud Storage Account connection string to use to insert the log entries to.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="storageTableName">Table name that log entries will be written to. Note: Optional, setting this may impact performance</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration AzureTableStorage(
            this LoggerSinkConfiguration loggerConfiguration,
            string connectionString,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            string storageTableName = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return AzureTableStorage(loggerConfiguration, storageAccount, restrictedToMinimumLevel, formatProvider, storageTableName);
        }
    }
}
