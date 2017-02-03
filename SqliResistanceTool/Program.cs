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
            c.SiteRoot =new Uri( "http://localhost/");
            c.LoginPage =new Uri(c.SiteRoot, "/login.php");
            c.LoginData.Add("username", "admin");
            c.LoginData.Add("password", "password");
            c.Start();
        }
    }
}
