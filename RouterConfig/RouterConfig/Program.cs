using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;


namespace RouterConfig
{
    internal class Program
    {
        public static byte[] GetBytes(string bitString)
        {
            return Enumerable.Range(0, bitString.Length / 8).
                Select(pos => Convert.ToByte(
                    bitString.Substring(pos * 8, 8),
                    2)
                ).ToArray();
        }
        static void Main(string[] args)
        {
            try
            {
                var instance = Convert.ToInt32(args.Where(x => x.Contains("instance=")).First().Replace("instance=", ""));
                var iplist = args
                    .Where(x => !x.Contains("instance="))
                    .SelectMany(arg => File.ReadAllLines(arg));

                var config = File.ReadAllText(AppContext.BaseDirectory + "RouterConf.txt");

                var configs = iplist
                    .Select(ip =>
                    {
                        instance += 1;
                        var prefix = ip.Contains("/") ? Convert.ToInt32(ip.Split("/")[1]) : 32;
                        ip = ip.Contains("/") ? ip.Split("/")[0] : ip;

                        var subnet = string.Empty.PadLeft(prefix, '1').PadRight(32, '0');
                        var subnetByes = GetBytes(subnet);
                        var mask = new IPAddress(subnetByes);


                        return (ip, mask, instance);
                    })
                    .Select(x => config
                    .Replace("#INSTANCE", x.instance.ToString())
                    .Replace("#DESTIP", x.ip)
                    .Replace("#SUBNET", x.mask.ToString())
                    .Replace("#FILTERNAME",$"tiktok{x.instance}")
                    )
                    .Aggregate((a, c) => $"{a}{Environment.NewLine}{c}");

                Console.WriteLine(configs);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }
    }
}
