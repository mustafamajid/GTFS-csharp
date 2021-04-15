using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFS_csharp
{
    public class MoveWithTime : move
    {
        public DateTime StartTime;
        public DateTime EndTime;
        public MoveWithTime(string startid, string startname, string endid, string endname, string routeid, string routename,string sid, DateTime st, DateTime et):base(startid,  startname,  endid,  endname,  routeid,  routename,sid)
        {
            StartTime = st;// Equal to the given time that a jurny satrt-Departure time
            EndTime = et;//araival to ditination
        }
        public MoveWithTime():base ()
        {

        }
    }
    public class PathWithTime : path
    {
        List<MoveWithTime> WayWithTime;
        public PathWithTime(string init_id, string init_name, string dest_id, string dest_name, List<move> waypassed, List<MoveWithTime> WT) : base(init_id, init_name, dest_id, dest_name, waypassed)
        {
            WayWithTime = WT;
        }
    }
    class timecalculator
    {

        public static List<PathWithTime> GetPathTime(List<path> sol, DateTime time)
        {
            List<PathWithTime> result = new List<PathWithTime>();

            foreach(path p in sol)
            {
                result.Add(new PathWithTime(p.initial_id, p.initial_name, p.destination_id, p.destination_name, p.way, GetMovesWithTime(p.way, time)));
            }
            return result;
        }
      public static List<MoveWithTime> GetMovesWithTime(List<move> m,DateTime a)
        {
            List<MoveWithTime> MTList = new List<MoveWithTime>();
            List<MoveWithTime> result = new List<MoveWithTime>();
            for (int i=0;i<m.Count;i++)
            {
                MoveWithTime mt = GetRoutTime(m[i], a);
                if(mt.EndTime==DateTime.MinValue)
                {
                    goto a;
                }
                else
                {
                    MTList.Add(mt);
                }
                a = mt.EndTime;
            }
            result = MTList;

            a:
            return result;
        }

        public static MoveWithTime GetRoutTime(move m,DateTime a)
        {
            
            MoveWithTime result = new MoveWithTime(m.start_stop_id,m.start_stop_name,m.end_stop_id,m.end_stop_name,m.route_id,m.route_name,m.serviceID ,DateTime.MinValue,DateTime.MinValue);
            if(m.route_id=="walk")
            {
                result.StartTime = a;//start time is the same time the person reach to the stop
                result.EndTime = a.AddMinutes(3);//he will reach the distnation after 3 min of walk
                goto a;

            }
            if(checkCalenderDate(a,m.serviceID))
             {
                goto a;
            }
            int i, j;
            for(i=0; i<GTFSData.StopTimesFile.Count-1;i++)
            {

                if(GTFSData.StopTimesFile[i].Route_id==m.route_id&&GTFSData.StopTimesFile[i].stop_id==m.start_stop_id&& GTFSData.StopTimesFile[i].departure_time>a)
                {
                    string trip = GTFSData.StopTimesFile[i].trip_id;
                    for(j=i+1;j<GTFSData.StopTimesFile.Count&& GTFSData.StopTimesFile[j].trip_id ==trip && GTFSData.StopTimesFile[j].Route_id==m.route_id; j++)
                    {
                        if(GTFSData.StopTimesFile[j].stop_id==m.end_stop_id)
                        {
                            DateTime arr, dep;
                            arr = GTFSData.StopTimesFile[j].arrival_time;
                            dep = GTFSData.StopTimesFile[i].departure_time;
                            result.StartTime =new DateTime(dep.Year,dep.Month,dep.Day,dep.Hour,dep.Minute,dep.Second) ;
                            result.EndTime =new DateTime(arr.Year,arr.Month,arr.Day,arr.Hour,arr.Minute,arr.Second) ;
                            goto a;
                        }
                    }
                }
            }


            a:
            return result;
        }

        public static Boolean checkCalenderDate(DateTime d,string sid)
        {
            Boolean result = true;
            int ind = GTFSData.ServicDataListDctionary[sid];

            if(!(d <= GTFSData.ServiceDataList[ind].end_date && d>=GTFSData.ServiceDataList[ind].start_date))
            {
                result = false;
                goto a;
            }
            int y = d.Year;
            int m = d.Month;
            int day = d.Day;

            foreach(DateTime dat in GTFSData.ServiceDataList[ind].exep)
            {
                if(dat.Year==y&& dat.Month==m&& dat.Day==day)
                {
                    result = false;
                    goto a;
                }
            }

        a:

            return result;
        }

    }
}
