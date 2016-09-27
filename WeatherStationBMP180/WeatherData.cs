using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStationBMP180
{
    public class WeatherData
    {
        public string deviceId { get; set; }
        public double temperature { get; set; }
        public double pressure { get; set; }
    }
}
