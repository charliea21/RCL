using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RCL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("welcome to RCL (roblox cookie logger) please join any roblox game and your cookie will be printed");
            bool pf = false;
            while (!pf)
            {
                Thread.Sleep(500);
                if (Process.GetProcessesByName("RobloxPlayerBeta").Length > 0)
                    pf = true;
            }
            Console.WriteLine("detected roblox");
            Thread.Sleep(500);
            Console.WriteLine("getting cookie");
            Process rbx = Process.GetProcessesByName("RobloxPlayerBeta")[0];
            Console.WriteLine("using rbx process " + rbx.Id.ToString());
            Console.WriteLine("gathering command line");
            string cli = GetCommandLine(rbx);
            string parsed = cli.Split(' ')[5];
            Console.WriteLine("working");
            string cookie = CookieConversion(parsed);
            Console.WriteLine("cookie recieved");
            Console.WriteLine("your cookie:\n");
            Console.WriteLine(cookie);
            Console.ReadKey();
        }

        private static string GetCommandLine(Process process)
        {
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id.ToString()))
            {
                using (ManagementObjectCollection source = managementObjectSearcher.Get())
                    return source.Cast<ManagementBaseObject>().SingleOrDefault<ManagementBaseObject>()?["CommandLine"]?.ToString();
            }
        }

        private static string CookieConversion(string auth)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://www.roblox.com/Login/Negotiate.ashx?suggest={0}", auth));
                httpWebRequest.Headers.Add("RBXAuthenticationNegotiation", ": https://www.roblox.com");
                httpWebRequest.Headers.Add("RBX-For-Gameauth", "true");
                httpWebRequest.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                    return new Regex(".ROBLOSECURITY=(.*?);").Match(response.Headers.Get("Set-Cookie")).Groups[1].Value;
            }
            catch
            {
                return "Auth Ticket Expired";
            }
        }
    }
}
