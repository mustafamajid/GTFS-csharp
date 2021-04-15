using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTFS_csharp
{
    
    public partial class Form1 : Form
    {
       
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stop.ReadStopsFile();
            Trip.ReadTripFile();
            StopTime.StopTimesReader();
            RouteFile.ReadRouteFile();
            // The next funcation contain multipul function call
            // and convert it to Stops data object in te GTFSData.Stops  List 
            // and calculate the Distance and add it to the AllRoutes list
            // and found the routes list which belong to each list
            // so its must called after read all other files
            List<path> RL = new List<path>();
            System.Diagnostics.Stopwatch t1= new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch t2 = new System.Diagnostics.Stopwatch();
            RL = RL;
            Stop.FindWalkableAndTranstionRoutes();
            ServiceData.FillServiceDataList();
            GTFSData.RoutsFile = GTFSData.RoutsFile;
            GTFSData.RouteFileIndex = GTFSData.RouteFileIndex;
            GTFSData.StopsFile = GTFSData.StopsFile;
            GTFSData.TripsFile = GTFSData.TripsFile;
            GTFSData.StopTimesFile = GTFSData.StopTimesFile;
            GTFSData.AllRoutes = GTFSData.AllRoutes;
            int i;
            int j;
            i = GTFSData.StopsIndex["2400205"];
            j = GTFSData.StopsIndex["2300507"];
            GTFSData.Stops = GTFSData.Stops;
            Stop.Removedublicatewalk();
            Stopdata s1 = GTFSData.Stops[i];
            Stopdata s2 = GTFSData.Stops[j];
            double d = Stop.GetDistanceBetweenPoints(Convert.ToDouble(s1.Stop_lat), Convert.ToDouble(s1.Stop_lon), Convert.ToDouble(s2.Stop_lat), Convert.ToDouble(s2.Stop_lon));
            t1.Start();
            Algorithm.Findpaths("1502307", "7100607");
            t1.Stop();
            textBox1.Text = t1.Elapsed.ToString();
            //RedisActions.InsertTORedis(Algorithm.two_stops_solutions_list);
            //RedisActions.InsertAllSolToRedis();
            List<PathWithTime> Pl = new List<PathWithTime>();
            List<PathWithTime> Pl2 = new List<PathWithTime>();
            DateTime t = new DateTime(2020, 01, 01, 13, 00, 00);
            GTFSData.search_d = 29;
            GTFSData.search_m = 05;
            GTFSData.search_year = 2019;
            t2.Start();
            RL = RedisActions.GetSolFromRedis("1502307", "7100607");
            t2.Stop();
            textBox2.Text = t2.Elapsed.ToString();
            Pl = timecalculator.GetPathTime(Algorithm.two_stops_solutions_list, t);
            Pl2 = timecalculator.GetPathTime(RL, t);
            Algorithm.two_stops_solutions_list = Algorithm.two_stops_solutions_list;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            RedisActions.inserttolist("m", "mustafa");
            RedisActions.inserttolist("m", "hayder");
            RedisActions.inserttolist("m", "5");
            foreach(string l in RedisActions.readlist("m"))
            {
                richTextBox1.Text = richTextBox1.Text + l + "\n";
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
          
        }
    }
}
