using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Locality
{
    public class LogService
    {
        public static void Log(string log)
        {
            Fiddler.FiddlerApplication.Log.LogString("[" + ConfigService.AppName + "] " + log);
        }
    }
}
