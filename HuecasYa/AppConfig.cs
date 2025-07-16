using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HuecasYa
{
    public static class AppConfig
    {
        public static string apiUrl { get; set; }

        static AppConfig()
        {
            apiUrl = "http://192.168.31.34:5295";
        }
    }
}
