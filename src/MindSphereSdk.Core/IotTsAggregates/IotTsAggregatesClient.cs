﻿using MindSphereSdk.Core.Common;
using MindSphereSdk.Core.Connectors;
using MindSphereSdk.Core.Helpers;
using MindSphereSdk.Core.Serialization;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MindSphereSdk.Core.IotTsAggregates
{
    /// <summary>
    /// Querying aggregated time series data for performance assets based on pre-calculated aggregate values
    /// </summary>
    public class IotTsAggregatesClient : SdkClient
    {
        private readonly string _baseUri = "/api/iottsaggregates/v4";

        internal IotTsAggregatesClient(MindSphereConnector mindSphereConnector)
            : base(mindSphereConnector)
        {
        }

        /// <summary>
        /// Get aggregated time series data for one aspect of an asset
        /// </summary>
        public async Task<IEnumerable<T>> GetAggregateTimeSeriesAsync<T>(GetAggregateTimeSeriesRequest request) where T : AggregateSet
        {
            // prepare URI string
            QueryStringBuilder queryBuilder = new QueryStringBuilder();
            queryBuilder.AddQuery("from", request.From);
            queryBuilder.AddQuery("to", request.To);
            queryBuilder.AddQuery("select", request.Select);
            queryBuilder.AddQuery("assetId", request.AssetId);
            queryBuilder.AddQuery("aspectName", request.AspectName);
            queryBuilder.AddQuery("intervalValue", request.IntervalValue);
            queryBuilder.AddQuery("intervalUnit", request.IntervalUnit);
            queryBuilder.AddQuery("count", request.Count);
            string uri = _baseUri + "/aggregates" + queryBuilder.ToString();

            // make request
            string response = await HttpActionAsync(HttpMethod.Get, uri);
            var tsAggregateWrapper = JsonConverter.Deserialize<AggregateWrapper<T>>(response);
            var tsAggregate = tsAggregateWrapper.Aggregates;
            return tsAggregate;
        }
    }
}
