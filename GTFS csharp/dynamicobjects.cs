using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFS_csharp
{
    public class Stopdata
    {
        public String Stop_id;
        public String Stop_name;
        public String Stop_shortname;
        public String Stop_desc;
        public String Stop_comment;
        public String Stop_street;
        public String Stop_lat;
        public String Stop_lon;
        public String Location_type;
        public String Parent_station;
        public String Stop_angle;
        public String Wheelchair_boarding;
        public List <route> routes;

    }

    public class route : ICloneable
    {
        public string route_id;
        public List<string> stops;
        public string tripid;
        public int visited_index;
        public route(string routid,List<string> stoplist,string trip_id,int Visited_index)
        {
            route_id = routid;
            stops = stoplist;
            tripid = trip_id;
            visited_index = Visited_index;
        }
        public virtual object  Clone() { return new route(this.route_id,this.stops,this.tripid,this.visited_index ); }
    }

    public class move
    {

        public string start_stop_id;
        public string start_stop_name;
        public string end_stop_id;
        public string end_stop_name;
        public string route_id;
        public string route_name;
        public string serviceID;
        public move(string startid,string startname,string endid,string endname,string routeid,string routename,string sid)
        {
            start_stop_id = startid;
            start_stop_name = startname;
            end_stop_id = endid;
            end_stop_name = endname;
            route_id = routeid;
            route_name = routename;
            serviceID = sid;
        }
        public move()
        {

        }
    }

    public class path:ICloneable
    {
        public string initial_id;
        public string initial_name;
        public string destination_id;
        public string destination_name;
        public List<move> way;
        public path(string init_id,string init_name,string dest_id,string dest_name,List<move> waypassed)
        {
            initial_id = init_id;
            initial_name = init_name;
            destination_id = dest_id;
            destination_name = dest_name;
            way = waypassed;
        }
        public static List<move> clonemovelist(List<move> l)
        {
            List<move> r = new List<move>();

            foreach(move i in l)
            {
                r.Add(new move(i.start_stop_id, i.start_stop_name, i.end_stop_id, i.end_stop_name, i.route_id, i.route_name,i.serviceID));
            }
            return r;

        }
        public virtual object Clone() { return new path(this.initial_id,this.initial_name,this.destination_id,this.destination_name,clonemovelist( this.way)); }
    }
        public class Qeueu
    {
        public path waypassed;
        public string current_stop_id;
        public int tran_number;
        public Qeueu(path way,string currentstopid,int trans)
        {
            waypassed = way;
            current_stop_id = currentstopid;
            tran_number = trans;
        }
    }
    public class ServiceData
    {
        public string service_id;
        public DateTime start_date;
        public DateTime end_date;
        public List<DateTime> exep;
        public ServiceData(string id,DateTime sd,DateTime ed)
        {
            service_id = id;
            start_date = sd;
            end_date = ed;
            exep = new List<DateTime>();
        }
        public static DateTime ConvertToDateTime(string s)
        {
            int y =Convert.ToInt16( s.Substring(0, 4));
            int m = Convert.ToInt16(s.Substring(4,2).TrimStart('0'));
            int d = Convert.ToInt16(s.Substring(6).TrimStart('0'));
            return (new DateTime(y, m, d));
        }
        public static void FillServiceDataList()
        {
            calender.ReadCalnderFile();
            calender_date.ReadCalnder_dateFile();
            List<ServiceData> result = new List<ServiceData>();
            int i = 0;
            for(i=0;i<GTFSData.CalenderFile.Count;i++)
            {
                string sid = GTFSData.CalenderFile[i].service_id;
                DateTime st = ConvertToDateTime(GTFSData.CalenderFile[i].start_date);
                DateTime ed = ConvertToDateTime(GTFSData.CalenderFile[i].end_date);

                GTFSData.ServiceDataList.Add(new ServiceData(sid, st, ed));
                GTFSData.ServicDataListDctionary.Add(sid, i);
            }
            for(i=0;i<GTFSData.Calender_dateFile.Count;i++)
            {
                string sid = GTFSData.Calender_dateFile[i].service_id;
                DateTime d = ConvertToDateTime(GTFSData.Calender_dateFile[i].date);
                int ind = GTFSData.ServicDataListDctionary[sid];
                GTFSData.ServiceDataList[ind].exep.Add(d);
            }

        }

    }


 }
