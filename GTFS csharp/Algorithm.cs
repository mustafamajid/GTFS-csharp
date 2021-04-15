using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFS_csharp
{
    public class Algorithm
    {
       
        public static List<path> two_stops_solutions_list;
        public static List<route> CheckedRoutes;        
        public static List<Qeueu> CallQeueu;
        public static void Findpaths(string inti_id, string des_id)
        {
            two_stops_solutions_list = new List<path>();
            CheckedRoutes = new List<route>();
            path passedway = new path(inti_id, GTFSData.Stops[GTFSData.StopsIndex[inti_id]].Stop_name, des_id, GTFSData.Stops[GTFSData.StopsIndex[des_id]].Stop_name, new List<move>());
            CallQeueu = new List<Qeueu>();
            CallQeueu.Add(new Qeueu(passedway,inti_id,0));
            while(CallQeueu.Count>0)
            {
                CheckStop(CallQeueu[0].waypassed, CallQeueu[0].current_stop_id, CallQeueu[0].tran_number);
                CallQeueu.RemoveAt(0);

            }
        }
        public static bool CheckedRoutesContaine(route r)
        {
            bool result = false;
            foreach (route croute in CheckedRoutes)
            {
                if (croute.route_id == r.route_id && (croute.stops.SequenceEqual(r.stops) || r.stops.SequenceEqual(croute.stops) || GTFSData.ContainsSubsequence(croute.stops, r.stops)))
                {
                    result = true;
                }
            }
            return result;
        }

        public static void CheckStop(path passedway, string cs_id, int trans_number)// current stop id is passed here
        {

            if (trans_number <= GTFSData.Trans_Number_limit && cs_id != passedway.destination_id)
            {
                Stopdata cs = GTFSData.Stops[GTFSData.StopsIndex[cs_id]];// cs stand for current stop (the stop to be checked by the function
                Stopdata destination = GTFSData.Stops[GTFSData.StopsIndex[passedway.destination_id]];
                foreach (route r in cs.routes)
                {
                    if (r.stops.IndexOf(cs_id) < r.visited_index || r.visited_index == -1)
                    {
                        if (r.stops.Contains(destination.Stop_id) && r.stops.IndexOf(cs_id) < r.stops.IndexOf(destination.Stop_id))
                        {
                            two_stops_solutions_list.Add(MakeMove((path)passedway.Clone(), cs, destination, r));
                        }
                        int i;
                        for (i = r.stops.IndexOf(cs_id) + 1; i < r.stops.Count && r.stops.IndexOf(cs_id) != r.stops.Count -1; i++)
                        {
                            Stopdata nextstop = GTFSData.Stops[GTFSData.StopsIndex[r.stops[i]]];
                            path newpassedway = MakeMove((path)passedway.Clone(), cs, nextstop, r);
                            Qeueu newq = new Qeueu((path)newpassedway.Clone(), nextstop.Stop_id, trans_number + 1);
                            CallQeueu.Add(newq);//هنا اسوي الكول من ستاك
                        }
                    }
                }
                update_vistiedindex(cs_id);
            }
        }

        public static void update_vistiedindex(string stopid)
        {
            Stopdata s = GTFSData.Stops[GTFSData.StopsIndex[stopid]];

            foreach(route r in s.routes)
            {
                int newindex = r.stops.IndexOf(stopid);
                int i;
                for(i=newindex;i<r.stops.Count;i++)
                {
                    string sid = r.stops[i];
                    updateroute(sid, stopid);
                }
            }
        }
        public static void updateroute(string stopid, string istop_id)
        {
            Stopdata s = GTFSData.Stops[GTFSData.StopsIndex[stopid]];
            foreach (route r in s.routes)
            {
                if (r.stops.Contains(istop_id))
                {
                    int newindex = r.stops.IndexOf(istop_id);
                    if ((r.visited_index > newindex || r.visited_index == -1)&&r.route_id != "walk")
                    { r.visited_index = newindex; }
                }
            }
        }
        public static path MakeMove(path p, Stopdata cs, Stopdata des, route r)
        {
            if(r.route_id=="walk")
            {
                p.way.Add(new move(cs.Stop_id, cs.Stop_name, des.Stop_id, des.Stop_name, r.route_id, "walk", "walk"));
                goto a;
            }
            move newmove = new move(cs.Stop_id, cs.Stop_name, des.Stop_id, des.Stop_name, r.route_id, GTFSData.RoutsFile[GTFSData.RouteFileIndex[r.route_id]].route_short_name,GTFSData.trip_service[r.tripid]);
            p.way.Add(newmove);
            a:
            return (path)p.Clone();
        }

    }
}
