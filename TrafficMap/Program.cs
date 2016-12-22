/*
 * 开发环境
 * VS2015,.net 4.5.2
 * 陈晓雨 2016-12-22 
 */


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

            #region 问题1-5 检测路线是否存在
            List<string> ways = new List<string>() { "A-B-C", "A-D", "A-D-C", "A-E-B-C-D", "A-E-D" };
            foreach (var way in ways)
            {
                Console.WriteLine($"Output #{ways.IndexOf(way) + 1}: {map.CheckWay(way)}");
            }
            #endregion

            #region  问题6 
            List<Way> P6 = map.FindWays("C-C");
            var P6Result = from y in P6
                           where y.Routes.Count <= 3
                           select y;
            Console.WriteLine($"Output #6: {P6Result.Count()}"); ;
            #endregion

            #region  问题7
            List<Way> P7 = map.FindWays("A-C");
            var P7Result = from y in P7
                           where y.Routes.Count == 4
                           select y;
            Console.WriteLine($"Output #7: {P7Result.Count()}");
            #endregion

            #region 问题8
            Console.WriteLine($"Output #8: {map.FindShortest("A-C")}");
            #endregion

            #region 问题9
            Console.WriteLine($"Output #9: {map.FindShortest("B-B")}");
            #endregion

            #region  问题10
            List<Way> P10 = map.FindWays("C-C");
            var P10Result = from y in P10
                            where y.Distance < 30
                            select y;
            Console.WriteLine($"Output #10: {P10Result.Count()}"); ;
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
        /// 所有的单向线路
        /// </summary>
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
            Way line;
            line = CheckRoutes(way);
            string result = "NO SUCH ROUTE";
            if (line != null)
            {
                result = line.Distance.ToString();
            }
            return result;
        }
        public Way CheckRoutes(string way)
        {
            Way line = new Way();
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
        public string FindShortest(string strway)
        {

            Way way;
            way = FindShortestWay(strway);
            string result = "NO SUCH ROUTE";
            if (way != null)
            {
                result = way.Distance.ToString();
            }
            return result;
        }
        public Way FindShortestWay(string strway)
        {

            Way way = new Way();
            string[] waypoint = strway.Split('-');
            for (int i = 0; i < waypoint.Length - 1; i++)
            {
                Way subroutes = FindShortestWay(waypoint[i], waypoint[i + 1]);
                if (subroutes != null && subroutes.Routes.Count > 0)
                {
                    way.Routes.AddRange(subroutes.Routes);
                }
                else
                {
                    //有一个节点不通
                    way = null;
                    break;
                }
            }
            return way;
        }
        private Stack<string> haschecked = new Stack<string>();
        public Way FindShortestWay(string start, string nextstop)
        {

            List<Way> lines = new List<Way>();

            if (Stations.ContainsKey(start))
            {

                foreach (var x in Stations[start].Routes)
                {

                    if (x.NextStop == nextstop)
                    {
                        //如果这条路线的终点站就是要找的终点站
                        Way line = new Way();
                        line.Routes.Add(x);
                        lines.Add(line);
                    }
                    else
                    {
                        if (!haschecked.Contains(x.NextStop))
                        {

                            haschecked.Push(x.NextStop);
                            //尝试查找以这条终点站为始发站的路线是否能到达 查询的终点站
                            Way tmpresult = FindShortestWay(x.NextStop, nextstop);
                            if (tmpresult != null)
                            {
                                Way line = new Way();
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

        #region 查找点到点之间可能的路线路线长度 条件 总长度<=30
        private const int MaxDistance = 30;
        private int wayDistance = 0;
        public List<Way> FindWays(string way)
        {
            List<Way> Ways = new List<Way>();
            wayDistance = 0;
            string[] waypoint = way.Split('-');
            for (int i = 0; i < waypoint.Length - 1; i++)
            {
                List<Way> sub_ways = FindWays(waypoint[i], waypoint[i + 1]);
                if (sub_ways != null)
                {
                    if (Ways.Count == 0)
                    {
                        Ways = sub_ways;
                    }
                    else
                    {
                        List<Way> tmpways = new List<Way>();
                        foreach (var x in Ways)
                        {
                            foreach (var y in sub_ways)
                            {
                                Way l = new Way();
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
            return Ways;
        }
        /// <summary>
        /// 查找两点之间尽可能出现的距离，前提 距离小于等于30(有条件的递归,防止死循环)
        /// </summary>
        /// <param name="start"></param>
        /// <param name="nextstop"></param>
        /// <returns></returns>
        public List<Way> FindWays(string start, string nextstop)
        {

            List<Way> ways = new List<Way>();
            if (Stations.ContainsKey(start))
            {
                foreach (var x in Stations[start].Routes)
                {
                    if (x.NextStop == nextstop)
                    {
                        if (wayDistance + x.Distance <= MaxDistance)
                        {
                            //如果这条路线的终点站就是要找的终点站
                            Way way = new Way();
                            way.Routes.Add(x);
                            ways.Add(way);

                            #region 查找此站出发是否能返回此站
                            wayDistance += x.Distance;
                            List<Way> sub_ways = FindWays(x.NextStop, nextstop);
                            wayDistance -= x.Distance;
                            if (sub_ways != null)
                            {
                                List<Way> tmpways = new List<Way>();
                                foreach (var y in sub_ways)
                                {
                                    Way l = new Way();
                                    l.Routes.AddRange(way.Routes);
                                    l.Routes.AddRange(y.Routes);
                                    tmpways.Add(l);
                                }
                                ways.AddRange(tmpways);
                            }
                            #endregion
                        }

                    }
                    else
                    {
                        //如果尝试查找以这条终点站为起点的路线，先计算其距离是否已经超标
                        if (wayDistance + x.Distance <= MaxDistance)
                        {
                            wayDistance += x.Distance;
                            List<Way> sub_ways = FindWays(x.NextStop, nextstop);
                            wayDistance -= x.Distance;
                            if (sub_ways != null)
                            {
                                //如果有线路可走
                                Way way = new Way();
                                way.Routes.Add(x);


                                List<Way> tmpways = new List<Way>();
                                foreach (var y in sub_ways)
                                {
                                    Way tmpw = new Way();
                                    tmpw.Routes.AddRange(way.Routes);
                                    tmpw.Routes.AddRange(y.Routes);
                                    tmpways.Add(tmpw);
                                }
                                ways.AddRange(tmpways);
                            }
                        }
                    }
                }

            }
            if (ways.Count == 0) ways = null;
            return ways;
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
    /// <summary>
    /// 路线
    /// </summary>
    public class Way
    {
        public Way()
        {
            Routes = new List<Route>();
        }
        public List<Route> Routes;
        public int Distance
        {
            get { return Routes.Sum(x => x.Distance); }
        }

        public override string ToString()
        {
            string tmp = "";
            foreach (var x in Routes)
            {
                tmp += x.Name + "->";
            }
            return $"{ Distance}：{tmp}";
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
