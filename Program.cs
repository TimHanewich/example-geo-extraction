using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Testng
{
    class Program
    {
        //https://dejanstojanovic.net/aspnet/2019/february/reading-coordinates-from-the-photo-in-net-core/
        //https://docs.microsoft.com/en-us/dotnet/api/system.drawing.imaging.propertyitem.id?view=net-5.0#System_Drawing_Imaging_PropertyItem_Id
        //https://docs.microsoft.com/en-us/windows/win32/gdiplus/-gdiplus-constant-property-item-descriptions?redirectedfrom=MSDN

        static void Main(string[] args)
        {
            Image i = new Bitmap(System.IO.File.OpenRead(args[0]));
            PropertyItem[] props = i.PropertyItems;
            
            float lat = 0;
            float lon = 0;
            byte LatitudeReference = 0;
            byte LongitudeReference = 0;

            foreach (PropertyItem pi in props)
            {
                if (pi.Id == 2) //Latitude
                {
                    lat = GetCoordinateFromExif(pi);
                }
                else if (pi.Id == 4) //Longitude
                {
                    lon = GetCoordinateFromExif(pi);
                }
                else if (pi.Id == 1) //Latitude reference - Either 'n' or 's' in ASCII code.
                {
                    LatitudeReference = pi.Value[0];
                }
                else if (pi.Id == 3) //Longitude reference - Either 'e' or 'w' in ASCII code.
                {
                    LongitudeReference = pi.Value[0];
                }
            }

            //Do the conversion if it is south latitude or west longitude
            if (LatitudeReference == 83) //83 is ASCII for 's', meaning south. So flip the latitude
            {
                lat = lat * -1;
            }

            //Do the conversion if it is east or west longtiude
            if (LongitudeReference == 87)
            {
                lon = lon * -1;
            }

            Console.WriteLine(lat.ToString() + ", " + lon.ToString());
        
        }

        private static float GetCoordinateFromExif(PropertyItem pi)
        {             
            uint degreesN = BitConverter.ToUInt32(pi.Value, 0);
            uint degreesD = BitConverter.ToUInt32(pi.Value, 4);
            uint minutesN = BitConverter.ToUInt32(pi.Value, 8);
            uint minutesD = BitConverter.ToUInt32(pi.Value, 12);
            uint secondsN = BitConverter.ToUInt32(pi.Value, 16);
            uint secondsD = BitConverter.ToUInt32(pi.Value, 20);

            float degrees = Convert.ToSingle(degreesN) / Convert.ToSingle(degreesD);
            float minutes = Convert.ToSingle(minutesN) / Convert.ToSingle(minutesD);
            float seconds = Convert.ToSingle(secondsN) / Convert.ToSingle(secondsD);

            float coord = degrees + (minutes / 60f) + (seconds / 3600f);

            return coord;
        }
    }
}
