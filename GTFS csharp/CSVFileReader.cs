using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GTFS_csharp
{
    public class RouteFile
    {
        public string route_id;
        public string agency_id;
        public string route_type;
        public string route_short_name;
        public string route_long_name;
        public string route_desc;
        public string route_color;
        public string route_text_color;
        public string route_url;
        public RouteFile(string id,string agency,string rtype,string short_name,string longname,string desc,string color,string textcolor,string url)
        {
            route_id = id;
            agency_id = agency;
            route_type = rtype;
            route_short_name = short_name;
            route_long_name = longname;
            route_desc = desc;
            route_color = color;
            route_text_color = textcolor;
            route_url = url;

        }
        public static void ReadRouteFile()
        {
            List<RouteFile> Result = new List<RouteFile>();
            List<string> FileLines = new List<string>();
            FileLines = File.ReadAllLines("routes.csv").Skip(1).ToList();
            string[] field;
            int i = 0;
            foreach (string line in FileLines)
            {
                field = line.Split(',');
                Result.Add(new RouteFile(field[0], field[1], field[2], field[3], field[4], field[5], field[6], field[7], field[8]));
                GTFSData.RouteFileIndex.Add(field[0], i);
                i++;


            }
            Result.Add(new RouteFile("walk", "walk", "walk", "walk", "walk", "walk", "walk", "walk", "walk"));
            GTFSData.RouteFileIndex.Add("walk", i);
            GTFSData.RoutsFile = Result;
           


        }
    }

    public class Stop
    {
        String Stop_id;
        String Stop_name;
        String Stop_shortname;
        String Stop_desc;
        String Stop_comment;
        String Stop_street;
        String Stop_lat;
        String Stop_lon;
        String Location_type;
        String Parent_station;
        String Stop_angle;
        String Wheelchair_boarding;

        public Stop(string ID, string name, string shortname, string desc, string comment, string street, string lat, string lon, string typ, string parent, string angle, string wheel)
        {
            Stop_id = ID;
            Stop_name = name;
            Stop_shortname = shortname;
            Stop_desc = desc;
            Stop_comment = comment;
            Stop_street = street;
            Stop_lat = lat;
            Stop_lon = lon;
            Location_type = typ;
            Parent_station = parent;
            Stop_angle = angle;
            Wheelchair_boarding = wheel;

        }
        public static void ReadStopsFile()
        {
            List<Stop> Result = new List<Stop>();
            List<string> FileLines = new List<string>();
            FileLines = File.ReadAllLines("stops.csv").Skip(1).ToList();
            string [] field;

            foreach (string line  in FileLines)
            {
                field = line.Split(',');
                Result.Add(new Stop(field[0], field[1], field[2], field[3], field[4], field[5], field[6], field[7], field[8], field[9], field[10], field[11]));


            }
            GTFSData.StopsFile= Result;
            FillStopsList();


        }
        public static void Removedublicatewalk()
        {
            foreach(Stopdata s in GTFSData.Stops)
            {
                for(int i=0; i<s.routes.Count;i++)
                {
                    if(s.routes[i].route_id=="walk"&& s.routes[i].stops.IndexOf(s.Stop_id)==1)
                    {
                        s.routes.RemoveAt(i);
                        
                    }
                }
            }
        }
        public static void FillStopsList()
        {
            foreach (Stop s in GTFSData.StopsFile)
            {
                Stopdata NewStop = new Stopdata();
                NewStop.Stop_id = s.Stop_id;
                NewStop.Stop_name = s.Stop_name;
                NewStop.Stop_shortname = s.Stop_shortname;
                NewStop.Stop_desc = s.Stop_desc;
                NewStop.Stop_comment = s.Stop_comment;
                NewStop.Stop_street = s.Stop_street;
                NewStop.Stop_lat = s.Stop_lat;
                NewStop.Stop_lon = s.Stop_lon;
                NewStop.Location_type = s.Location_type;
                NewStop.Parent_station = s.Parent_station;
                NewStop.Wheelchair_boarding = s.Wheelchair_boarding;
                NewStop.routes = new List<route>();

                GTFSData.Stops.Add(NewStop);

            }
            int i = 0;
            for (i=0;i<GTFSData.Stops.Count; i++)
            {
                GTFSData.StopsIndex.Add(GTFSData.Stops[i].Stop_id, i);
            }
           
        }
        public static double GetDistanceBetweenPoints(double lat1, double long1, double lat2, double long2)
        {
            double distance = 0;

            double dLat = (lat2 - lat1) / 180 * Math.PI;
            double dLong = (long2 - long1) / 180 * Math.PI;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(lat2) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(lat1 / 180 * Math.PI), 2);
            //Denominator part of the function
            double dr = Math.Pow(radiusE * Math.Cos(lat1 / 180 * Math.PI), 2)
                            + Math.Pow(radiusP * Math.Sin(lat1 / 180 * Math.PI), 2);
            double radius = Math.Sqrt(nr / dr);

            //Calaculate distance in metres.
            distance = radius * c;
            return distance;
        }
        public static void FindWalkableAndTranstionRoutes()
        {
            int i, j;
            for (i=0;i<GTFSData.Stops.Count-1;i++)
            {
                for(j=i+1;j<GTFSData.Stops.Count;j++)
                {
                    double lat1 = Convert.ToDouble(GTFSData.Stops[i].Stop_lat);
                    double lon1 = Convert.ToDouble(GTFSData.Stops[i].Stop_lon);

                    double lat2 = Convert.ToDouble(GTFSData.Stops[j].Stop_lat);
                    double lon2= Convert.ToDouble(GTFSData.Stops[j].Stop_lon);

                    double distance = GetDistanceBetweenPoints(lat1, lon1, lat2, lon2);
                    if (distance <= GTFSData.WalkableDistance)
                    { 
                        List<string> l1 = new List<string>();
                        l1.Add(GTFSData.Stops[i].Stop_id);
                        l1.Add(GTFSData.Stops[j].Stop_id);

                        List<string> l2 = new List<string>();
                        l2.Add(GTFSData.Stops[j].Stop_id);
                        l2.Add(GTFSData.Stops[i].Stop_id);
                        GTFSData.AllRoutes.Add(new route("walk",l1,"walk",-1));
                        GTFSData.AllRoutes.Add(new route("walk", l2, "walk",-1));

                    }

                }
            }
            //This Code to file the stops object with available route for each Stop
            foreach( route r in GTFSData.AllRoutes)
            {
                foreach(string id in r.stops)
                {
                    int ind = GTFSData.StopsIndex[id];
                    GTFSData.Stops[ind].routes.Add((route)r.Clone());
                }
            }
        
        }
    }
    public class Trip
    {
        public string trip_id;
        public string route_id;
        public string service_id;
        public string direction_id;
        public string shape_id;
        public string trip_headsign;
        public string wheelchair_accessible;
        public string exceptional;
        public string bikes_allowed;
        
        Trip(string id,string rout_id,string serv_id,string dir_id,string shapeid,string headsign,string wheel,string excep ,string bike)
        {
            trip_id = id;
            route_id = rout_id;
            service_id = serv_id;
            direction_id = dir_id;
            shape_id = shapeid;
            trip_headsign = headsign;
            wheelchair_accessible = wheel;
            exceptional = excep;
            bikes_allowed = bike;


        }

        public static void ReadTripFile()
        {
            List<Trip> Result = new List<Trip>();
            List<string> FileLines = new List<string>();
            FileLines = File.ReadAllLines("trips.csv").Skip(1).ToList();
            string[] field;

            foreach (string line in FileLines)
            {
                field = line.Split(',');
                Result.Add(new Trip(field[0], field[1], field[2], field[3], field[4], field[5], field[6], field[7], field[8]));
                GTFSData.StopRoutDic.Add(line.Split(',')[0], line.Split(',')[1]);

            }
            GTFSData.TripsFile= Result;
            GTFSData.trip_service = new Dictionary<string, string>();

            for(int i=0;i<GTFSData.TripsFile.Count;i++)
            {
                GTFSData.trip_service.Add(GTFSData.TripsFile[i].trip_id, GTFSData.TripsFile[i].service_id);
            }

        }
        public static void FillStopsList()
        {

        }

    }

    public class StopTime
    {
       public string  trip_id;
       public string stop_sequence;
       public  string stop_id;
       public  string stop_headsign;
       public  DateTime arrival_time;
       public  DateTime departure_time;
       public string pickup_type;
       public string drop_off_type;
       public string shape_dist_traveled;
       public string Route_id;
        StopTime(string Tripid,string stopseq,string stopid,string headsign,string arrival,string depature,string pickup,string drop,string shapdis,string rout)
        {
            trip_id = Tripid;
            stop_sequence = stopseq;
            stop_id = stopid;
            stop_headsign = headsign;
            string adate = "2020-01-01 ";
            string ddate = "2020-01-01 ";
            if(arrival.IndexOf(":")==1)
            {
                arrival = "0" + arrival;
            }
            if (depature.IndexOf(":") == 1)
            {
                depature = "0" + depature;
            }
            if (arrival.Substring(0,2)=="13")
            {
                arrival = arrival;

            }
            if (arrival.Substring(0,2)=="24")
            {
                arrival = "00" + arrival.Substring(2);
                adate = "2020-01-02 ";
            }
            if (depature.Substring(0, 2) == "24")
            {
                depature = "00" + depature.Substring(2);
                ddate = "2020-01-02 ";
            }


            arrival_time = DateTime.ParseExact(adate +arrival, "yyyy-MM-dd HH:mm:ss",null);
            departure_time = DateTime.ParseExact(ddate + depature, "yyyy-MM-dd HH:mm:ss", null);
            pickup_type = pickup;
            drop_off_type = drop;
            shape_dist_traveled = shapdis;
            Route_id = rout;

        }
        public static void StopTimesReader()
        {
            List<StopTime > Result = new List<StopTime>();
            List<string> FileLines = new List<string>();
            FileLines = File.ReadAllLines("stop_times.csv").Skip(1).ToList();
            string[] field;
            foreach (string line in FileLines)
            {
                field = line.Split(',');
                Result.Add(new StopTime(field[0], field[1], field[2], field[3], field[4], field[5], field[6], field[7], field[8],GTFSData.StopRoutDic[field[0]]));
               
            }
            GTFSData.StopTimesFile = Result;

            int i;

            string t;
            List<string> r = new List<string>();
            string r_id;
            for (i=0;i<GTFSData.StopTimesFile .Count-1;i++)
            {
                r_id = GTFSData.StopTimesFile[i].Route_id;
                r.Add(GTFSData.StopTimesFile[i].stop_id);
                t = GTFSData.StopTimesFile[i].trip_id;

                if (GTFSData.StopTimesFile[i].trip_id!=GTFSData.StopTimesFile[i+1].trip_id )
                {
                    route newrout = new route(r_id ,r,t,-1);
                    if (!GTFSData.AllRoutesContaine(newrout ))
                    {
                        GTFSData.AllRoutes.Add(new route(newrout.route_id,new List<string>(newrout.stops),t ,-1));
                    }
                    r.Clear();
                    
                }
            }
        }
       
    }
    public class calender
    {
        public string service_id;
        public string service_desc;
        public string start_date;
        public string end_date;

        public calender(string id,string desc,string sdate,string edate)
        {
            service_id = id;
            service_desc = desc;
            start_date = sdate;
            end_date = edate;
        }

        public static void ReadCalnderFile()
        {
            List<calender> Result = new List<calender >();
            List<string> FileLines = new List<string>();
            FileLines = File.ReadAllLines("calendar.csv").Skip(1).ToList();
            string[] field;

            foreach (string line in FileLines)
            {
                field = line.Split(',');
                Result.Add(new calender(field[0], field[1], field[2], field[3]));
            }
            GTFSData.CalenderFile = Result;

        }

    }
    public class calender_date
    {
        public string service_id;
        public string date;
        public string exception_type;
        public calender_date(string id,string d, string exp)
        {
            service_id = id;
            date = d;
            exception_type = exp;
        }

        public static void ReadCalnder_dateFile()
        {
            List<calender_date> Result = new List<calender_date>();
            List<string> FileLines = new List<string>();
            FileLines = File.ReadAllLines("calendar_dates.csv").Skip(1).ToList();
            string[] field;
            foreach (string line in FileLines)
            {
                field = line.Split(',');
                Result.Add(new calender_date(field[0], field[1], field[2]));
            }
            GTFSData.Calender_dateFile = Result;
        }


    }


}
