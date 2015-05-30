using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace ShapesSample.ValueConverters
{
    /// <summary>
    /// Convert collection of points into a geometry path.
    /// </summary>
    [ValueConversion(typeof(IEnumerable<Point>), typeof(Geometry))]
    public class PointsToPathConverter : IValueConverter
    {
        private static PointsToPathConverter _instance = new PointsToPathConverter();

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static PointsToPathConverter Instance
        {
            get { return _instance; }
        }


        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var points = (IEnumerable<Point>)value;

            if (!points.Any())
                return null;

            Point start = points.First();
            List<LineSegment> segments = new List<LineSegment>();

            foreach (var point in points.Skip(1))
            {
                segments.Add(new LineSegment(point, true));
            }

            PathFigure figure = new PathFigure(start, segments, false); //true if closed
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
