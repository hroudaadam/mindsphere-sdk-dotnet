﻿using MindSphereSdk.Core.AssetManagement;
using MindSphereSdk.Core.Authentication;
using MindSphereSdk.Core.EventManagement;
using MindSphereSdk.Core.Helpers;
using MindSphereSdk.Core.IotTimeSeries;
using MindSphereSdk.Core.IotTsAggregates;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MindSphereSdk.Core.Common
{
    /// <summary>
    /// MindSphere API SDK
    /// </summary>
    // TODO: change docs
    // TODO: arch diagram
    public class MindSphereApiSdk
    {
        private AssetManagementClient _assetManagementClient;
        private IotTimeSeriesClient _iotTimeSeriesClient;
        private IotTsAggregatesClient _iotTsAggregateClient;
        private EventManagementClient _eventManagementClient;

        private readonly MindSphereConnector _connector;

        public MindSphereApiSdk(ICredentials credentials, ClientConfiguration configuration, HttpClient httpClient = null)
        {
            Validator.Validate(configuration);
            httpClient = httpClient ?? new HttpClient();

            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }
            else if (credentials is AppCredentials appCredentials)
            {
                Validator.Validate(appCredentials);
                _connector = new AppMindSphereConnector(appCredentials, configuration, httpClient);
            }
            else if (credentials is UserCredentials userCredentials)
            {
                Validator.Validate(userCredentials);
                _connector = new UserMindSphereConnector(userCredentials, configuration, httpClient);
            }
            else
            {
                throw new ArgumentException("Invalid credentials type", nameof(credentials));
            }
        }

        /// <summary>
        /// Get Asset Management Client
        /// </summary>
        public AssetManagementClient GetAssetManagementClient()
        {
            if (_assetManagementClient == null)
            {
                _assetManagementClient = new AssetManagementClient(_connector);
            }
            return _assetManagementClient;
        }

        /// <summary>
        /// Get IoT Time Series Client
        /// </summary>
        public IotTimeSeriesClient GetIotTimeSeriesClient()
        {
            if (_iotTimeSeriesClient == null)
{
                _iotTimeSeriesClient = new IotTimeSeriesClient(_connector);
            }
            return _iotTimeSeriesClient;
        }

        /// <summary>
        /// Get IoT Time Series Aggregate Client
        /// </summary>
        public IotTsAggregatesClient GetIotTsAggregateClient()
        {
            if (_iotTsAggregateClient == null)
            {
                _iotTsAggregateClient = new IotTsAggregatesClient(_connector);
            }
            return _iotTsAggregateClient;
        }

        /// <summary>
        /// Get Event Management Client
        /// </summary>
        public EventManagementClient GetEventManagementClient()
        {
            if (_eventManagementClient == null)
            {
                _eventManagementClient = new EventManagementClient(_connector);
            }
            return _eventManagementClient;
        }
    }
}
