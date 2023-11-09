using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tentamen_Server_Side_Programming.Models.Weather;
using Tentamen_Server_Side_Programming.Services.Interface;

namespace Tentamen_Server_Side_Programming.Services
{
    public class ImageWriteService : IImageWriteService
    {
        public byte[] DrawImage(byte[] byteArr, StationMeasurement stationMeasurement)
        {
            using var stream = new MemoryStream(byteArr);
            using var image = new MagickImage(stream);

            image.Settings.FillColor = MagickColors.White;
            image.Settings.BorderColor = MagickColors.Black;
            image.Settings.FontWeight = FontWeight.Bold;
            image.Settings.FontPointsize = 28;

            image.Settings.StrokeColor = MagickColors.Black;
            image.Settings.StrokeWidth = 2;  

            int baseLine = 50;
            int lineHeight = 40;
            int padding = 30;

            var drawables = new List<IDrawable>
        {
            new DrawableText(padding, baseLine, $"{stationMeasurement.stationname} in {stationMeasurement.regio}"),
            new DrawableText(padding, baseLine + 1 * lineHeight, $"Temperature: {stationMeasurement.temperature}°C"),
            new DrawableText(padding, baseLine + 2 * lineHeight, $"WindDirection: {stationMeasurement.winddirection}"),
            new DrawableText(padding, baseLine + 3 * lineHeight, $"Windspeed: {stationMeasurement.windspeed}")
        };

            image.Draw(drawables);

            return image.ToByteArray();
        }
    }
}
