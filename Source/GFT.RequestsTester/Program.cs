using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace GFT.RequestsTester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var times = new List<Double>();
            HttpClient http= new HttpClient();
            http.BaseAddress = new Uri("https://gft-product-service.azurewebsites.net/products/");
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Local(1), PlusServer(2), Both(3)? -1 to QUIT!!!");
                var typeValue = Console.ReadLine();
                if (int.TryParse(typeValue, out int type) && 
                    (type>=-1 && type<=3))
                {
                    string[] environments = null;
                    if (type == 1)
                        environments = new string[] { "local" };
                    else if (type == 2)
                        environments = new string[] { "plusserver" };
                    else if (type == 3)
                        environments = new string[] { "local", "plusserver" };
                    else if (type == -1)
                        break;

                    Console.WriteLine("How many requests?");
                    var ctValue = Console.ReadLine();
                    Stopwatch sw = new Stopwatch();
                    double totalTime = 0;
                    if (int.TryParse(ctValue, out int counter))
                    {
                        for (int i = 1; i <= counter; i++)
                        {
                            sw.Start();
                            Console.WriteLine($"Request {i}...");
                            foreach (var env in environments)
                            {
                                Console.WriteLine($"Request {i}@{env}...");
                                var response = await http.GetAsync(env);
                                response.EnsureSuccessStatusCode();                                
                            }
                            sw.Stop();//Stopping before the 500ms
                            times.Add(sw.Elapsed.TotalMilliseconds);
                            totalTime += sw.Elapsed.TotalMilliseconds;
                            Console.WriteLine($"Waiting... Total Avg: {totalTime / i:0.##} ms.");                            
                            sw.Reset();
                            System.Threading.Thread.Sleep(500);
                        }
                        sw.Stop();
                        Console.WriteLine($"================================");
                        Console.WriteLine($"Requests: {counter}");
                        Console.WriteLine($"Avg Time: {times.Average():0.##} ms.");
                        Console.WriteLine($"Min Time: {times.Min():0.##} ms.");
                        Console.WriteLine($"Max Time: {times.Max():0.##} ms.");
                        Console.WriteLine($"================================");
                        Console.WriteLine($"PRESS ANY KEY TO RESTART");
                        Console.ReadKey();
                    }                    
                }              
            }
        }
    }
}
