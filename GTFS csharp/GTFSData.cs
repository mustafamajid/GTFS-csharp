using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFS_csharp
{
    
    public class GTFSData
    {

        public static List<Stop> StopsFile = new List<Stop>();

        public static List<RouteFile> RoutsFile = new List<RouteFile>();
        public static Dictionary<string, int> RouteFileIndex = new Dictionary<string, int>();
        public static Dictionary<string, string> StopRoutDic = new Dictionary<string,string>();
        public static List<Trip> TripsFile = new List<Trip>();
        public static List<StopTime> StopTimesFile = new List<StopTime>();
        public static List<Stopdata> Stops = new List<Stopdata>();
        public static Dictionary<string, int> StopsIndex = new Dictionary<string, int>();
        public static List<route> AllRoutes = new List<route>();
        public static int Trans_Number_limit = 3;
        public static int WalkableDistance = 100;
        public static List<calender> CalenderFile = new List<calender>();
        public static List<calender_date> Calender_dateFile = new List<calender_date>();
        public static Dictionary<string, string> trip_service = new Dictionary<string, string>();
        public static Dictionary<string, int> calenderdate_index = new Dictionary<string, int>();
        public static List<ServiceData> ServiceDataList = new List<ServiceData>();// Contain Service Class objects List 
        public static Dictionary<string, int> ServicDataListDctionary = new Dictionary<string, int>();//
        public static int search_year;
        public static int search_m;
        public static int search_d;

        public static void ch()
        {
            Stopdata d;
            d = GTFSData.Stops[1];
            d.Stop_name= "Mustafa";

        }
        public static bool AllRoutesContaine(route r)
        {
            bool result = false;

            foreach (route croute in AllRoutes)
            {
                if (croute.route_id==r.route_id &&( croute.stops.SequenceEqual(r.stops)||r.stops.SequenceEqual(croute.stops)||GTFSData.ContainsSubsequence(croute.stops ,r.stops )))
                {
                    result = true;
                }
            }
            return result;
        }
        public  static bool ContainsSubsequence<T>(List<T> l1, List<T> l2)
        {
            List<T> sequence;
            List<T> subsequence;
            if(l1.Count>=l2.Count )
            { sequence = l1;subsequence = l2; }
            else
            { sequence = l2;subsequence = l1; }
            return
                Enumerable
                    .Range(0, sequence.Count - subsequence.Count + 1)
                    .Any(n => sequence.Skip(n).Take(subsequence.Count).SequenceEqual(subsequence));
        }
    }
}
