﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MindSphereSdk.AssetManagement;
using MindSphereSdk.Common;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppCredentials appCredentials = new AppCredentials(
                "iiotdgli-testapplication-1.0.0",
                "***",
                "testapplication",
                "1.0.0",
                "iiotdgli",
                "iiotdgli"
            );

            HttpClient httpClient = new HttpClient();

            AssetManagementClient assetClient = new AssetManagementClient(appCredentials, httpClient);

            List<Asset> test = (await assetClient.ListAssetsAsync()).ToList();
            foreach (var item in test)
            {
                Console.WriteLine(item.AssetId);
                if (item.Location != null) {
                    Console.WriteLine(item.Location.Country);
                }
            }
            
            Console.ReadKey();
        }

    }
}
