using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tentamen_Server_Side_Programming.Models.Weather;

namespace Tentamen_Server_Side_Programming.Services.Interface
{
    public interface IImageWriteService
    {
        public byte[] DrawImage(byte[] byteArr, StationMeasurement stationMeasurement);
    }
}
