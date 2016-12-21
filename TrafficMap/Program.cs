using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficMap
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> route = new List<string> { "AB5", "BC4", "CD8", "DC8", "DE6", "AD5", "CE2", "EB3", "AE7" };
            TrafficMap map = new TrafficMap(route);

            #region  问题7
            Console.WriteLine($"Output #6: {map.FindWay("C-C", 3)}"); ;
            #endregion


            #region 问题1-5
            List<string> ways = new List<string>() { "A-B-C", "A-D", "A-D-C", "A-E-B-C-D", "A-E-D" };
            foreach (var way in ways)
            {
                Console.WriteLine($"Output #{ways.IndexOf(way) + 1}: {map.CheckWay(way)}");
            }
            #endregion

            #region  问题7
            Console.WriteLine($"Output #6: {map.FindWay("C-C", 3)}"); ;
            #endregion

            #region  问题7

            #endregion

            #region 问题8
            Console.WriteLine($"Output #8: {map.FindShortestDistance("A-C")}");
            #endregion

            #region 问题9
            Console.WriteLine($"Output #9: {map.FindShortestDistance("B-B")}");
            #endregion

            Console.ReadKey();
        }


    }

    /// <summary>
    /// 交通图
    /// </summary>
    public class TrafficMap
    {
        /// <summary>
        /// 用来存放已经尝试过的始发站
        /// </summary>

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

        #region 检测路线是否存在
        public string CheckWay(string way)
        {
            Line line;
            line = CheckRoutes(way);
            string result = "NO SUCH ROUTE";
            if (line != null)
            {
                result = line.Distance.ToString();
            }
            return result;
        }
        public Line CheckRoutes(string way)
        {
            Line line = new Line();
            string[] waypoint = way.Split('-');
            for (int i = 0; i < waypoint.Length - 1; i++)
            {
                Route route = GetRoute(waypoint[i], waypoint[i + 1]);
                if (route != null)
                {
                    line.Routes.Add(route);
                }
                else
                {
                    line = null;
                    break;
                }
            }
            return line;
        }

        public Route GetRoute(string start, string nextstop)
        {
            Route result = null;
            if (Stations.ContainsKey(start))
            {

                foreach (var x in Stations[start].Routes)
                {

                    if (x.NextStop == nextstop)
                    {
                        result = x;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion

        #region 查找最短路线
        public string FindShortestDistance(string way)
        {

            Line line;
            line = FindShortestWay(way);
            string result = "NO SUCH ROUTE";
            if (line != null)
            {
                result = line.Distance.ToString();
            }
            return result;
        }
        public Line FindShortestWay(string way)
        {

            Line line = new Line();
            string[] waypoint = way.Split('-');
            for (int i = 0; i < waypoint.Length - 1; i++)
            {
                Line subroutes = FindShortestWay(waypoint[i], waypoint[i + 1]);
                if (subroutes != null && subroutes.Routes.Count > 0)
                {
                    line.Routes.AddRange(subroutes.Routes);
                }
                else
                {
                    //有一个节点不通
                    line = null;
                    break;
                }
            }
            return line;
        }
        private Stack<string> haschecked = new Stack<string>();
        public Line FindShortestWay(string start, string nextstop)
        {

            List<Line> lines = new List<Line>();

            if (Stations.ContainsKey(start))
            {

                foreach (var x in Stations[start].Routes)
                {

                    if (x.NextStop == nextstop)
                    {
                        //如果这条路线的终点站就是要找的终点站
                        Line line = new Line();
                        line.Routes.Add(x);
                        lines.Add(line);
                    }
                    else
                    {
                        if (!haschecked.Contains(x.NextStop))
                        {

                            haschecked.Push(x.NextStop);
                            //尝试查找以这条终点站为始发站的路线是否能到达 查询的终点站
                            Line tmpresult = FindShortestWay(x.NextStop, nextstop);
                            if (tmpresult != null)
                            {
                                Line line = new Line();
                                line.Routes.Add(x);
                                line.Routes.AddRange(tmpresult.Routes);
                                lines.Add(line);
                            }
                            haschecked.Pop();

                        }
                    }
                }
            }
            if (lines.Count > 0)
            {
                return lines.OrderBy(x => x.Distance).First();
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 查找点到点之间的有路线
        /// <summary>
        /// 查找路线
        /// </summary>
        /// <param name="way">期望路线</param>
        /// <param name="maxstops">最多换乘</param>
        /// <returns></returns>
        public List<Line> FindWay(string way, int maxstops)
        {
            List<Line> Ways = new List<Line>();

            string[] waypoint = way.Split('-');
            for (int i = 0; i < waypoint.Length - 1; i++)
            {
                List<Line> sub_ways = FindWays(waypoint[i], waypoint[i + 1]);
                if (sub_ways != null)
                {
                    if (Ways.Count == 0)
                    {
                        Ways = sub_ways;
                    }
                    else
                    {
                        List<Line> tmpways = new List<Line>();
                        foreach (var x in Ways)
                        {
                            foreach (var y in sub_ways)
                            {
                                Line l = new Line();
                                l.Routes.AddRange(x.Routes);
                                l.Routes.AddRange(y.Routes);
                                tmpways.Add(l);
                            }
                        }
                        Ways = tmpways;
                    }
                }
                else
                {
                    //有一个节点不通
                    Ways = null;
                    break;
                }
            }
            return Ways.Where(x => x.Routes.Count <= 3).ToList<Line>();
        }
        public List<Line> FindWays(string start, string nextstop)
        {

            List<Line> lines = new List<Line>();

            if (Stations.ContainsKey(start))
            {

                foreach (var x in Stations[start].Routes)
                {

                    if (x.NextStop == nextstop)
                    {
                        //如果这条路线的终点站就是要找的终点站
                        Line line = new Line();
                        line.Routes.Add(x);
                        lines.Add(line);
                    }
                    else
                    {
                        if (!haschecked.Contains(x.NextStop))
                        {

                            haschecked.Push(x.NextStop);
                            //尝试查找以这条终点站为始发站的路线是否能到达 查询的终点站
                            Line tmpresult = FindShortestWay(x.NextStop, nextstop);
                            if (tmpresult != null)
                            {
                                Line line = new Line();
                                line.Routes.Add(x);
                                line.Routes.AddRange(tmpresult.Routes);
                                lines.Add(line);
                            }
                            haschecked.Pop();

                        }
                    }
                }
            }
            return lines;
        }
        #endregion


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
        /// 单向路线
        /// </summary>
        public List<Route> Routes { get; set; }

    }
    public class Line
    {
        public Line()
        {
            Routes = new List<Route>();
        }
        public List<Route> Routes;
        public int Distance
        {
            get { return Routes.Sum(x => x.Distance); }
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
            int.TryParse(route[2].ToString(), out tmp);
            Distance = tmp;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
