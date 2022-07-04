using System.Text;
using System.Xml;

namespace ConvertCsvTpGpx
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <2)
            {
                Console.WriteLine("ConvertCsvTpGpx csvFilePathWithElevation gpxFileToAugment");
                return;
            }

            string csvFilePathToConvert = args[0];
            string gpxConvertedFilePath = args[1] ?? Path.ChangeExtension(csvFilePathToConvert, "gpx");

            IEnumerable<WayPoint> wayPoints = ReadWayPoints(csvFilePathToConvert);

            CreateGpxFile(gpxConvertedFilePath, wayPoints);

        }

        private static void CreateGpxFile(string gpxConvertedFilePath, IEnumerable<WayPoint> wayPoints)
        {
            // Read the GPX file.
            string gpxFileContent = File.ReadAllText(gpxConvertedFilePath);

            // Find the section of the way points. Save the XML before that and after that to write back
            int beginWayPoints = gpxFileContent.IndexOf("<trkpt ");
            int endWayPoints = gpxFileContent.IndexOf("</trkseg>");
            string begin = gpxFileContent.Substring(0, beginWayPoints);
            string end = gpxFileContent.Substring(endWayPoints);

            // Write the 3 sections before, waypoints from the collection, and after
            StringBuilder sb = new StringBuilder();
            sb.Append(begin);
            foreach(WayPoint wayPoint in wayPoints)
            {
                sb.AppendLine($"<trkpt lat=\"{wayPoint.latitude}\" lon=\"{wayPoint.longitude}\"><ele>{wayPoint.elevation}</ele></trkpt>");
  // 2d              sb.Append($"<trkpt lat=\"{wayPoint.latitude}\" lon=\"{wayPoint.longitude}\"/>");
            }
            sb.Append(end);

            int indexExtension = gpxConvertedFilePath.IndexOf(".gpx");
            string transformedFile = gpxConvertedFilePath.Substring(0, indexExtension)
                + "-elev"
                + gpxConvertedFilePath.Substring(indexExtension);

            File.WriteAllText(transformedFile, sb.ToString());
        }

        private static IEnumerable<WayPoint> ReadWayPoints(string csvFilePathToConvert)
        {
            string[] lines = File.ReadAllLines(csvFilePathToConvert);
            if (lines.Length == 0)
                yield break;
            string[] cells = lines[0].Split(",");
            int indexLatitude = Array.IndexOf(cells, "Lat");
            int indexLongitude = Array.IndexOf(cells, "Lng");
            int indexElevation = Array.IndexOf(cells, "Elevation (meters)");

            for(int row=1; row<lines.Length; ++row)
            {
                cells = lines[row].Split(",");
                double latitude = double.Parse(cells[indexLatitude]);
                double longitude = double.Parse(cells[indexLongitude]);
                double elevation = double.Parse(cells[indexElevation]);
                yield return new WayPoint(longitude, latitude, elevation);
            }

        }
    }

    public class WayPoint
    {
        public WayPoint(double longitude, double latitude, double elevation)
        {
            this.longitude = longitude;
            this.latitude = latitude;
            this.elevation = elevation;
        }
        public double latitude;  //degres
        public double longitude; //degres
        public double elevation; // meters

        public override string ToString()
        {
            return $"{longitude} {latitude} {elevation}";
        }
    }
}