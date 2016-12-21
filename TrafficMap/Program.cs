using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> route = new List<string> { "AB5", "BC4", "CD8", "DC8", "DE68", "AD58", "CE28", "EB38", "AE7" };
            List<string> ways = new List<string>() { "A-B-C" };
            TrafficMap map = new TrafficMap(route);
            foreach (var way in ways)
            {
                Console.Write(map.FindShortestDistance(way));
            }
        }


    }

    /// <summary>
    /// 交通图
    /// </summary>
    public class TrafficMap
    {
        //所有的单向线路
        public List<Route> Routes { get; set; }
        /// <summary>
        /// 所有的车站
        /// </summary>
        public Dictionary<string, Station> Stations { get; set; }

        public TrafficMap(List<string> lstrouets)
        {
            Routes = new List<Route>();
            Stations = new Dictionary<string, Station>();
            //初始化所有路线
            foreach (var tmp in lstrouets)
            {
                Routes.Add(new Route(tmp));
            }

            //计算所有始发站，以及车站拥有的路线
            foreach (var route in Routes)
            {
                if (!Stations.ContainsKey(route.Start))
                {
                    Stations.Add(route.Start, new Station(route));
                }
                else
                {
                    Stations[route.Start].Routes.Add(route);
                }
            }
        }
        public int FindShortestDistance(string way)
        {
            return 0;
        }
        public List<Route> FindAWay(string way)
        {
            return new List<Route>();
        }

        public List<Route> FindAWay(string start, string nextstop)
        {
            return new List<Route>();
        }

    }

    public class Station
    {
        public Station(Route route)
        {
            this.Name = route.Start;
            this.Routes = new List<Route>();
            Routes.Add(route);
        }
        /// <summary>
        /// 站名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 下一站
        /// </summary>
        public List<Route> Routes { get; set; }

        public List<Route> FindShortestRoute(string nextstop)
        {
            return new List<Route>();
        }

    }

    /// <summary>
    /// 路线
    /// </summary>
    public class Route
    {

        public string Name { get; set; }
        /// <summary>
        /// 出发站
        /// </summary>
        public string Start { get; set; }
        /// <summary>
        /// 终点站
        /// </summary>
        public string NextStop { get; set; }
        /// <summary>
        /// 路线距离
        /// </summary>
        public int Distance { get; set; }

        public Route(string route)
        {
            Name = route;
            Start = route[0].ToString();
            NextStop = route[1].ToString();
            int tmp = 0;
            int.TryParse(route[0].ToString(), out tmp);
            Distance = tmp;
        }

    }




}
