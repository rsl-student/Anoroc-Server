﻿using Anoroc_User_Management.Interfaces;
using Anoroc_User_Management.Models;
using Anoroc_User_Management.Models.ItineraryFolder;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Anoroc_User_Management.Services
{
    public class SQL_DatabaseService : IDatabaseEngine
    {

        // Documentation for SQL and JSON:
        // https://docs.microsoft.com/en-us/sql/relational-databases/json/json-data-sql-server?view=sql-server-ver15

        //The Following 4 lines connect to the database but not using Entity Framework
        protected SqlConnection Connection;
        public int MaxDate;
        public int Hours;
        /// <summary>
        /// Connect the Service by adding the Connection string
        /// </summary>
        /// <param name="connection_string">The connection string defined in appsettings.json to connect to the database</param>
        public SQL_DatabaseService(string connection_string)
        {
            Connection = new SqlConnection(connection_string);
        }
        // -----------------------------------------
        // Location SQL
        // -----------------------------------------
        //Setting up the connection to Entity Framework Database Context:

        readonly AnorocDbContext _context;
        /// <summary>
        /// Get an instance of the Service to be used locally
        /// </summary>
        /// <param name="context">The instance of service that allows the use of the dbContext object to manage the database</param>
        public SQL_DatabaseService(AnorocDbContext context, int maxdate, int hours)
        {
            MaxDate = maxdate;
            Hours = hours;
            _context = context;
        }
        public List<Location> Select_List_Locations()
        {
            var thePast = DateTime.Now.AddHours(-Hours).Ticks;
            try
            {
                var location = _context.Locations
                    .Where(l => l.Created.Ticks > thePast)
                    .Include(b => b.Region)
                    .Include(b => b.Cluster);
                if (location != null)
                    return location.ToList();
                else
                    return null;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        public List<Location> Select_List_Carrier_Locations()
        {
            var locations = _context.Locations
                .Where(l => l.Carrier_Data_Point == true)
                .Include(l => l.Region)
                .Include(b => b.Cluster);
            if (locations != null)
                return locations.ToList();
            else
                return null;
        }
        public List<Location> Select_Locations_By_Area(Area area)
        {
            var thePast = DateTime.Now.AddHours(-Hours).Ticks;
            var locations = _context.Locations
                .Where(loc => (loc.Region.Area_ID == area.Area_ID) && (loc.Created.Ticks>thePast))
                .Include(l => l.Region)
                .Include(b => b.Cluster)
                .ToList();

            if (locations != null)
                return locations;
            else
                return null;
        }
        public List<Location> Select_Locations_By_ID(long id)
        {
            var locations = _context.Locations
                .Where(l => l.Location_ID == id)
                .Include(l => l.Region)
                .Include(b => b.Cluster);
            if (locations != null)
                return locations.ToList();
            else
                return null;
        }
        public List<Location> Select_Unclustered_Locations(Area area)
        {
            var locations = _context.Locations
                .Where(l => l.ClusterReferenceID == null)
                .Where(c => !_context.Clusters
                    .Select(i => i.Center_LocationLocation_ID)
                    .Contains(c.Location_ID))
                .Include(a => a.Region)
                .Include(b => b.Cluster);
            if (locations != null)
                return locations.ToList();
            else
                return null;
        }
        public void Update_Carrier_Locations(string access_token, bool status)
        {
            _context.Locations
                .Where(l => l.AccessToken == access_token)
                .ToList()
                .ForEach(l => l.Carrier_Data_Point = status);
            _context.SaveChanges();
            Update_Old_Carrier_Locations(access_token, status);
        }
        public List<Area> Select_Unique_Areas()
        {
            var returnList = new List<Area>();
            var nonDistincList = _context.Areas.ToList();
            foreach(Area area in nonDistincList)
            {
                if (!returnList.Contains(area))
                    returnList.Add(area);
            }
            return returnList;
        }
        public bool Insert_Location(Location location)
        {
            var areas = Select_Unique_Areas(); 
            try
            {
                Area areadb = AreaListContains(areas, location.Region);

                if (areadb == null)
                {
                    Insert_Area(location.Region);
                }
                else
                {
                    location.Region = areadb;
                }

                _context.Locations.Add(location);
                _context.SaveChanges();
                return true;
            }
            catch  (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        private Area AreaListContains(List<Area> areas, Area locationArea)
        {
            Area returnVal = null;
            if(areas != null)
            {
                areas.ForEach(area =>
                {
                    if (locationArea.AreaEquals(area))
                        returnVal = area;
                });
            }
            return returnVal;
        }
        public bool Delete_Location(Location location)
        {
            try
            {
                _context.Locations.Remove(location);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public List<Location> Select_Location_By_Cluster_Reference(long reference)
        {
            var location = _context.Locations
                .Where(l => l.ClusterReferenceID == reference);
            if (location != null)
                return location.ToList();
            else
                return null;
        }
        public bool Update_Location(Location location)
        {
            try
            {
                _context.Locations.Update(location);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        /*public bool Delete_Locations_Older_Than_Hours(int hours)
        {         
            try
            {
                var locations = _context.Locations.Where(l =>
                    l.Created <= DateTime.Now.AddHours(-hours))
                    .ToList();
                foreach(Location location in locations)
                {
                    //Insert_Old_Location(location);
                    Delete_Location(location);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }*/

        // -----------------------------------------
        // Cluster SQL
        // -----------------------------------------
        public List<Cluster> Select_List_Clusters()
        {
            var thePast = DateTime.Now.AddHours(-Hours).Ticks;
            var returnList =_context.Clusters
                /*.Include(c => c.Coordinates)*/
                .Include(c => c.Center_Location)
                .ToList();
            var clusters = new List<Cluster>();
            foreach (var item in returnList)
            {
                var obj = _context.Entry(item)
                    .Collection(c => c.Coordinates)
                    .Query()
                    .ToList();
                foreach (var location in item.Coordinates)
                {
                    location.Cluster = null;
                    location.Region = _context.Areas
                        .Where(a => a.Area_ID == location.RegionArea_ID)
                        .FirstOrDefault();
                }
            }
            return returnList;
        }

        public bool Update_Cluster(Cluster cluster)
        {
            try
            {
                _context.Clusters.Update(cluster);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool Delete_Cluster(Cluster cluster)
        {
            try
            {
                _context.Clusters.Remove(cluster);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool Insert_Cluster(Cluster cluster)
        {
            try
            {
                _context.Clusters.Add(cluster);
                /*foreach(Location coord in cluster.Coordinates)
                {
                    coord.ClusterReferenceID=cluster.Cluster_Id;
                    *//*_context.Locations.Attach(coord);
                    _context.Entry(coord).Property(l => l.ClusterReferenceID).IsModified = true;
                    _context.SaveChanges();*//*
                }*/
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public List<Cluster> Select_Clusters_By_Area(Area area)
        {
            var areas = Select_Unique_Areas();
            var thePast = DateTime.Now.AddHours(-Hours).Ticks;
            Area areadb = AreaListContains(areas, area);
            if (areadb != null)
            {
                var clusters = _context.Clusters
                 .Where(cl => (cl.Center_Location.RegionArea_ID == areadb.Area_ID) && (cl.Cluster_Created.Ticks > thePast))
                 .Include(c => c.Coordinates)
                 .Include(l => l.Center_Location);
                if (clusters != null)
                    return clusters.ToList();
                else
                    return null;
            }
            else
                return null;
        }
        public long Get_Cluster_ID()
        {
            return 0;
        }
        /*public void Delete_Clusters_Older_Than_Hours(int hours)
        {
            try
            {
                var clusters = _context.Clusters.Where(c =>
                c.Cluster_Created <= DateTime.Now.AddHours(-hours)
                ).ToList();
                foreach (Cluster cluster in clusters)
                {
                    //Insert_Old_Cluster(cluster);
                    Delete_Cluster(cluster);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }*/
        // -----------------------------------------
        // User SQL
        // -----------------------------------------
        public List<User> Select_List_Users()
        {
            return _context.Users.ToList();
        }
        public bool Update_User(User user)
        {
            try
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool Delete_User(User user)
        {
            try
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool Insert_User(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public Location Get_Location_ByLongitude(double longitude)
        {
            try
            {
                Location getLocation = (from location in _context.Locations where location.Longitude == longitude select location).First();

                return getLocation;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public string Get_Firebase_Token(string access_token)
        {
            try
            {
                User getUser = (from user in _context.Users where user.AccessToken == access_token select user).First();
                if (getUser != null)
                    return getUser.Firebase_Token;
                else
                    return "-1";
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return "-1";
            }
        }

        public void Insert_Firebase_Token(string access_token, string firebase_token)
        {
            try
            {
                User updatedUser = (from user in _context.Users where user.AccessToken  ==  access_token select user).First();
                updatedUser.Firebase_Token = firebase_token;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

       
        public void Update_Carrier_Status(string access_token, string carrier_status)
        {
            bool user_status;
            if ((carrier_status.ToUpper()).Equals("POSITIVE") || (carrier_status.ToUpper()).Equals("TRUE"))
                user_status = true;
            else
                user_status = false;
            try
            {
                User updatedUser = _context.Users
                    .Where(u => u.AccessToken== access_token)
                    .FirstOrDefault();
                if (updatedUser != null)
                {
                    updatedUser.carrierStatus = user_status;
                    _context.Users.Update(updatedUser);
                    Update_Carrier_Locations(updatedUser.AccessToken, updatedUser.carrierStatus);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
       
        public string Get_User_Email(string access_token)
        {
            User user = _context.Users.Where(user => user.AccessToken == access_token).FirstOrDefault();
            if (user != null)
                return user.Email;
            else
                return "";
        }

        public User Get_User_ByID(string access_token)
        {
            User user = _context.Users.Where(user => user.AccessToken == access_token).FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            else
                return null;
        }

        public void populate()
        {
            string json;
            using (StreamReader r = new StreamReader("TempData/Points.json"))
            {
                json = r.ReadToEnd();
                Points items = JsonConvert.DeserializeObject<Points>(json);
                int count = 0;
                foreach (Point point in items.PointArray)
                {
                    Location location=null;
                    if(count <=50)
                    {
                        location = new Location(point.Latitude, point.Longitude, DateTime.Now, new Area("South Africa", "Gauteng", "Pretoria"));
                    }
                    else if(count <=100 && count>50)
                    {
                        location = new Location(point.Latitude, point.Longitude, DateTime.Now.AddDays(-1), new Area("South Africa", "Gauteng", "Pretoria"));
                    }
                    else if (count > 100 && count < 150)
                    {
                        location = new Location(point.Latitude, point.Longitude, DateTime.Now.AddDays(-2), new Area("South Africa", "Gauteng", "Pretoria"));
                    }
                    else
                    {
                        location = new Location(point.Latitude, point.Longitude, DateTime.Now.AddDays(-3), new Area("South Africa", "Gauteng", "Pretoria"));
                    }
                    if (Insert_Location(location))
                    {
                        Debug.WriteLine("Inserted Location: " + JsonConvert.SerializeObject(location));
                    }
                }
            }
        }

        public bool Validate_Access_Token(string access_token)
        {
            try
            {
                var searchUser = _context.Users.Where(user=>user.AccessToken==access_token).FirstOrDefault();
                if (searchUser != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public string Get_User_Access_Token(string email)
        {
            try
            {
                User user = _context.Users
                    .Where(u => u.Email.Equals(email))
                    .FirstOrDefault();
                if (user != null)
                    return user.AccessToken;
                else
                    return "";
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        // -----------------------------------------
        // Area Table SQL
        // -----------------------------------------

        public bool Insert_Area(Area area)
        {
            try
            {
                var check =Select_Unique_Areas();
                if (!check.Contains(area))
                    _context.Areas.Add(area);
                return true;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool Delete_Area(Area area)
        {
            try
            {
                var check = Select_Unique_Areas();
                if (check.Contains(area))
                    _context.Areas.Remove(area);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public Area Select_Area_By_Id(long id)
        {
            Area area=_context.Areas
                .Where(area => area.Area_ID == id).FirstOrDefault();
            if (area != null)
                return area;
            else
                return null;
        }
        // -----------------------------------------
        // Old Cluster Table SQL
        // -----------------------------------------

        public List<Cluster> Select_All_Old_Clusters()
        {
            var clusters = _context.Clusters
                .Where(ol => ol.Cluster_Created > DateTime.Now.AddDays(-MaxDate))
                .ToList();
            return clusters;
        }

        /*public void Delete_Old_Clusters_Older_Than_Days(int days)
        {
            try
            {
                var clusters = _context.Clusters.Where(c =>
                c.Cluster_Created.DayOfYear <= DateTime.Now.AddDays(-days).DayOfYear
                ).ToList();
                foreach (Cluster cluster in clusters)
                {
                    _context.Clusters.Remove(cluster);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }*/


        public Cluster Get_Cluster_ByID(long cluster_id)
        {
            try
            {
                Cluster getCluster = (from cluster in _context.Clusters where cluster.Cluster_Id == cluster_id select cluster).First();

                return getCluster;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public List<Cluster> Select_Old_Clusters_By_Area(Area area)
        {
            var thePast = DateTime.Now.AddHours(-Hours).Ticks;
            var oldClusters = _context.Clusters
                .Where(oc => (oc.Coordinates.FirstOrDefault().RegionArea_ID == area.Area_ID) && (oc.Cluster_Created.Ticks < thePast));
            if (oldClusters != null)
                return oldClusters.ToList();
            else
                return null;
        }
        public bool Insert_Old_Cluster(Cluster cluster)
        {
            try
            {                
                if (cluster != null)
                {
                    OldCluster old = new OldCluster(cluster);
                    old = Populate_Coordinates(old);
                    _context.OldClusters.Add(old);
                    _context.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public OldCluster Populate_Coordinates(OldCluster cluster)
        {
            try
            {
                cluster.Coordinates = _context.OldLocations
                    .Where(o => o.Old_Cluster_Reference_ID == cluster.Reference_ID)
                    .ToList();
                return cluster;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }

        }

        // -----------------------------------------
        // Old Locations Table SQL
        // -----------------------------------------
        public List<Location> Select_Old_Unclustered_Locations(Area area)
        {
            var thePast = DateTime.Now.AddHours(-Hours).Ticks;
            var oldLocation = _context.Locations
                .Where(ol => (ol.RegionArea_ID==area.Area_ID) && (ol.Created.Ticks<thePast));
            if (oldLocation != null)
                return oldLocation.ToList();
            else
                return null;
        }
        public List<Location> Select_All_Old_Locations()
        {
            var locations = _context.Locations
                .Where(ol => ol.Created > DateTime.Now.AddDays(-MaxDate))
                .ToList();
            return locations;
        }

        public void Update_Old_Carrier_Locations(string access_token, bool status)
        {
            _context.OldLocations
               .Where(l => l.Access_Token == access_token)
               .ToList()
               .ForEach(l => l.Carrier_Data_Point = status);
            _context.SaveChanges();
        }
        public bool Insert_Old_Location(Location location)
        {
            try
            {
                OldLocation old = new OldLocation(location);
                old.Region = _context.Areas
                    .Where(a => a.Area_ID == location.RegionArea_ID)
                    .FirstOrDefault();
                _context.OldLocations.Add(old);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        /*public void Delete_Old_Locations_Older_Than_Days(int days)
        {
            try
            {
                var locations = _context.Locations.Where(l =>
                l.Created.DayOfYear <= DateTime.Now.AddDays(-days).DayOfYear
                ).ToList();
                foreach (Location location in locations)
                {
                    _context.Locations.Remove(location);
                }
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }*/
        // -----------------------------------------
        // Itinerary Risk Table SQL
        // -----------------------------------------

        public void Insert_Itinerary_Risk(ItineraryRisk risk)
        {
            try
            {
                PrimitiveItineraryRisk insert = new PrimitiveItineraryRisk(risk);
                _context.ItineraryRisks.Add(insert);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public List<ItineraryRisk> Get_All_Itinerary_Risks()
        {
            List<ItineraryRisk> returnList = new List<ItineraryRisk>();
            try
            {
                _context.ItineraryRisks
                    .ToList()
                    .ForEach(i => returnList.Add(new ItineraryRisk(i)));
                return returnList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public List<ItineraryRisk> Get_Itinerary_Risks_By_Token(string token)
        {
            List<ItineraryRisk> returnList = new List<ItineraryRisk>();
            try
            {
                _context.ItineraryRisks
                    .Where(i => i.AccessToken == token)
                    .ToList()
                    .ForEach(i => returnList.Add(new ItineraryRisk(i)));
                return returnList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}
