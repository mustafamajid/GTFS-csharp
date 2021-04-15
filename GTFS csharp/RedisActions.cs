using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace GTFS_csharp
{
    public  static class RedisActions
    {
       public static ConnectionMultiplexer conn = ConnectionMultiplexer.Connect("localhost");
       public static IDatabase db = conn.GetDatabase();
        public static List<path> SolutionFromRedis;
        public static void inserttolist(string listname,string value)
        {
            db.ListRightPush(listname, value);
        }


        public static string[]  readlist(string listname)
        {
            System.Diagnostics.Stopwatch t2 = new System.Diagnostics.Stopwatch();
            t2.Start();
            RedisValue[] k = db.ListRange(listname, 0, -1);
            t2.Stop();
            string ss=t2.Elapsed.ToString();
            return (k.ToStringArray());
        }
        public static List<path> GetSolFromRedis(string s ,string e)
        {
            List<path> result = new List<path>();
            string[] Redislist = readlist(s + "___" + e);
            foreach (string l in Redislist)
            {
                string[] way = l.Split(new string[] { "***" }, StringSplitOptions.None);
                List<move> WayList = new List<move>();
                foreach(string mov in way)
                {
                    string[] m = mov.Split('|');
                    WayList.Add(new move(m[0],m[1],m[4],m[5],m[2],m[3],m[4]));
                }
                result.Add(new path(s, GTFSData.Stops[GTFSData.StopsIndex[s]].Stop_name, e, GTFSData.Stops[GTFSData.StopsIndex[e]].Stop_name, WayList));
            }
            return (result);

        }
        public static void InsertAllSolToRedis()
        {
            int i, j;
            for (i = 0; i < GTFSData.Stops.Count ; i++)
            {
                for(j=0;j<GTFSData.Stops.Count;j++)
                {
                    if(i!=j)
                    {
                        Algorithm.Findpaths(GTFSData.Stops[i].Stop_id, GTFSData.Stops[j].Stop_id);
                        RedisActions.InsertTORedis(Algorithm.two_stops_solutions_list);

                    }
                }
            }
        }
        public static void InsertTORedis(List<path> Sol)
        {
          foreach(path p in Sol)
            {
                string init_id = p.initial_id;
                string init_name = p.initial_name;
                string des_id = p.destination_id;
                string des_name = p.destination_name;
                string trans="";

                foreach (move way in p.way)
                {
                    trans = trans+"***" + way.start_stop_id + "|" + way.start_stop_name + "|" + way.route_id + "|" + way.route_name + "|" + way.end_stop_id + "|" + way.end_stop_name+"|"+way.serviceID;
                }

                if(trans.Substring(0,3)=="***")
                {
                    trans = trans.Substring(3);
                }
                db.ListRightPush(init_id+"___"+des_id,trans);

            }
        }

    }
}
