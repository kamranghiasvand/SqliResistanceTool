using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliResistanceTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Crawler.Crawler c = new Crawler.Crawler();
            c.SiteRoot = "http://localhost/";
            c.LoginPage = "/login.php";
            c.Start();
        }
    }
}
