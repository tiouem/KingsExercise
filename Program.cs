using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

namespace kings
{
    class Program
    {
        private const string url = "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";
        static async Task Main(string[] args)
        {
            List<King> kings = new List<King>();
            string content;
            try
            {

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    content = await response.Content.ReadAsStringAsync();
                }

            }
            catch (System.Exception ex)
            {
                throw new Exception("Error fetching data from ulr: ", ex);
            }

            try
            {
                kings = JsonSerializer.Deserialize<List<King>>(content);
            }
            catch (System.Exception ex)
            {
                throw new Exception("Error deserializing result: ", ex);
            }
            if (kings.Any())
            {

                Console.WriteLine(
                        @"
                         _  ___                          __   ______             _                 _ 
                        | |/ (_)                        / _| |  ____|           | |               | |
                        | ' / _ _ __   __ _ ___    ___ | |_  | |__   _ __   __ _| | __ _ _ __   __| |
                        |  < | | '_ \ / _` / __|  / _ \|  _| |  __| | '_ \ / _` | |/ _` | '_ \ / _` |
                        | . \| | | | | (_| \__ \ | (_) | |   | |____| | | | (_| | | (_| | | | | (_| |
                        |_|\_\_|_| |_|\__, |___/  \___/|_|   |______|_| |_|\__, |_|\__,_|_| |_|\__,_|
                                       __/ |                                __/ |                    
                                      |___/                                |___/                     
                        "
                );



                var mostCommonFirtsName = kings.Select(ks => new
                                                {
                                                    name = ks.nm.Split(' ').First()
                                                })
                                                .GroupBy(k => k.name)
                                                .Select(g => new
                                                {
                                                    name = g.Key,
                                                    count = g.Count()
                                                })
                                                .OrderByDescending(g => g.count)
                                                .First();

                var longestRulingMonarch = kings.Select(k => new
                                                {
                                                    name = k.nm,
                                                    yearsRuled = CalculateYearsRuled(k.yrs)
                                                })
                                                .OrderByDescending(k => k.yearsRuled)
                                                .First();


                var longestRulingHouse = kings.GroupBy(h => h.hse)
                                              .Select(g => new{
                                                  name = g.Key,
                                                  yearsRuled = g.Sum(x => CalculateYearsRuled(x.yrs))
                                              })
                                              .OrderByDescending(x=>x.yearsRuled)
                                              .First();


                Console.WriteLine($"Number of monarchs: {kings.Count}");
                Console.WriteLine($"Longest ruling monarch: {longestRulingMonarch.name} {longestRulingMonarch.yearsRuled} years");
                Console.WriteLine($"Longest ruling house: {longestRulingHouse.name} {longestRulingHouse.yearsRuled} years");
                Console.WriteLine($"Most common first name: {mostCommonFirtsName.name} {mostCommonFirtsName.count} times");

            }
            else
            {
                Console.WriteLine("No kings in the list.");
            }
            Console.ReadKey();
        }

        private static int CalculateYearsRuled(string yearsString)
        {
            var yearsRuled = 0;
            var years = yearsString.Split('-');
            if (years.Length == 2)
            {
                int.TryParse(years[0], out int firstYear);
                int.TryParse(years[1], out int secondYear);

                secondYear = secondYear > 0 ? secondYear : DateTime.UtcNow.Year;
                yearsRuled = secondYear - firstYear;
            }
            return yearsRuled;
        }
        public class King
        {
            public int id { get; set; }
            public string nm { get; set; }
            public string cty { get; set; }
            public string hse { get; set; }
            public string yrs { get; set; }

        }
    }
}
