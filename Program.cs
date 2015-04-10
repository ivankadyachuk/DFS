using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace DFS
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, List<int>> graph_map = get_file("SCC.txt");

            //foreach (var item in graph_map)
            //{
            //    foreach (var item2 in item.Value)
            //    {
            //        Console.WriteLine("key:{0} value:{1}", item.Key, item2);
            //    }

            //}
            //foreach (var item in Dfs_Loop(graph_map))
            //{
            //    Console.WriteLine(item);
            //}
            var stackSize = 10000000;
            List<int> result = new List<int>();
            Thread thread = new Thread(new ThreadStart(delegate { result= kosaraju(graph_map); }), stackSize);
            thread.Start();
            thread.Join();
            result.Sort();
            result.Reverse();
           
            foreach (var item in result.Take(5))
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }
        static int t, counter;
        static bool[] visited;
        static int[] finished_time;

        static void Init(int n) 
        {
            t = 0;
            counter = 1;
            finished_time = new int[n];
            visited = new bool[n];

        }


        static Dictionary<int, List<int>> get_file(string path)
        {
            var result = new Dictionary<int, List<int>>();
            using (StreamReader textIn = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                string line = string.Empty;
                while ((line = textIn.ReadLine()) != null)
                {
                    string[] keys = line.Split(' ');
                    int key1 = int.Parse(keys[0]);
                    int key2 = int.Parse(keys[1]);
                    if (!result.Keys.Contains(key1))
                        result.Add(key1, new List<int>() { key2 });
                    else
                        result[key1].Add(key2);
                    if (!result.Keys.Contains(key2))
                        result.Add(key2, new List<int>());
                }
            }
            return result;
        }

        
        static void Dfs(Dictionary<int, List<int>> graph_map, int start)
        {
            visited[start - 1] = true;

            foreach (var child in graph_map[start])
            {
                if (!visited[child - 1])
                {
                    Dfs(graph_map, child);
                    counter++;
                }
            }
            t++;
            finished_time[start - 1] = t;
        }

        static List<int> Dfs_Loop(Dictionary<int, List<int>> graph_map)
        {
            int len = graph_map.Keys.Count;
            Init(len);
            var count_list = new List<int>();
            for (int i = len; i > 0; i--)
            {
                if (!visited[i-1])
                {
                    counter = 1;
                    Dfs(graph_map, i);
                    count_list.Add(counter);
                }
            }
            return count_list;
        }

        static Dictionary<int, List<int>> Transpose_Graph(Dictionary<int, List<int>> graph_map)
        {
            var graph_map_rev = new Dictionary<int, List<int>>();
            foreach (var item in graph_map.Keys)
            {
                if (!graph_map_rev.ContainsKey(item))
                    graph_map_rev.Add(item, new List<int>());
                foreach (var item2 in graph_map[item])
                {
                      if (!graph_map_rev.ContainsKey(item2))
                          graph_map_rev.Add(item2, new List<int>());
                      graph_map_rev[item].Add(item2);
                }
            }
            return graph_map_rev;
        }

        static Dictionary<int, List<int>> Get_Graph_Finish(Dictionary<int, List<int>> graph_map) 
        {
            var graph_finish = new Dictionary<int, List<int>>();
            foreach (var item in graph_map.Keys)
            {
                if (!graph_finish.ContainsKey(finished_time[item-1]))
                    graph_finish.Add(finished_time[item - 1], new List<int>());
                foreach (var item2 in graph_map[item])
                {
                    if (!graph_finish.ContainsKey(finished_time[item2 - 1]))
                        graph_finish.Add(finished_time[item2 - 1], new List<int>());
                    graph_finish[finished_time[item2 - 1]].Add(finished_time[item - 1]);
                }
            }
            return graph_finish;
        }

        static List<int> kosaraju(Dictionary<int, List<int>> graph_map) 
        {
            var graph_map_rev = Transpose_Graph(graph_map);
            Dfs_Loop(graph_map_rev);
            var graph_finish = Get_Graph_Finish(graph_map_rev);
            return Dfs_Loop(graph_finish);
        }
    }
}
