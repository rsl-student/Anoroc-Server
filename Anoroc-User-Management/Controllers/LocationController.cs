﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Anoroc_User_Management.Interfaces;
using Anoroc_User_Management.Models;
using Anoroc_User_Management.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;

namespace Anoroc_User_Management.Controllers
{
    /// <summary>
    /// API Endpoint for all location data from client
    /// </summary>



    //[Produces("application/json")]
    //[Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {

        IClusterService Cluster_Service;

        public LocationController(IClusterService clusterService)
        {
            Cluster_Service = clusterService;
        }

       
        [HttpPost("Clusters/Pins")]
        public string Cluster_Pins([FromBody] Token token_object)
        {
            Area area = token_object.Object_To_Server;
            return Cluster_Service.GetClustersPins(area);
        }

        
      
        [HttpPost("Clusters/Simplified")]
        public string Clusters_ClusterWrapper([FromBody] Token token_object)
        {
            Area area2 = new Area();
            return new JavaScriptSerializer().Serialize(Cluster_Service.GetClusters(area2));
        }


        [HttpPost("GEOLocation")]
        public string GEOLocationAsync([FromBody] Token token_object)
        {
            if(token_object.access_token == "thisisatoken")
            {
                Location location = token_object.Object_To_Server;
                if(location.Carrier_Data_Point)
                {
                    //TODO:
                    //go to cluster
                }
                else
                {
                    //go to crossed path service
                }
                return "Hello";
            }
            else
                return "No";
        }

        //Function for Demo purposes, get the lcoation from the database to show funcitonality
        [HttpGet("toString")]
        public string toString()
        {
            return "stuff";
        }
    }
}