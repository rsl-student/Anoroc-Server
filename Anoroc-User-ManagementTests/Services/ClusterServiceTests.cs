﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Anoroc_User_Management.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Nancy.Json;
using Newtonsoft.Json;

using Anoroc_User_Management.Models;
using GeoCoordinatePortable;
using System.Diagnostics;

namespace Anoroc_User_Management.Services.Tests
{
    [TestClass()]
    public class ClusterServiceTests
    {
        [TestMethod()]
        public void Calculate_RadiusTest()
        {
            ClusterService clusterService = new ClusterService();
            string json = clusterService.GetClusters();

            double calulatedRadius = 388.701308010461;

            Assert.AreEqual(clusterService.Cluster_Wrapper_List[0].Cluster_Radius, calulatedRadius);
        }

        [TestMethod()]
        public void Calculate_CenterTest()
        {
            ClusterService clusterService = new ClusterService();
            string json = clusterService.GetClusters();

            GeoCoordinate coordinate = new GeoCoordinate(-25.78621883392334, 28.281863975524903);

            Assert.AreEqual(clusterService.Cluster_Wrapper_List[0].Center_Pin.Coordinate, coordinate);
        }
    }
}