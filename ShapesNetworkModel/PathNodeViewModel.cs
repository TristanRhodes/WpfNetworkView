using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Diagnostics;
using Utils;

namespace ShapesNetworkModel
{
    public class PathNodeViewModel : BaseNodeViewModel
    {
        /// <summary>
        /// The origional points supplied by the creating shape.
        /// </summary>
        private Point[] _basePath;
        private List<Point> _pathCollection;

        public PathNodeViewModel(Point[] basePath)
            :this(null, basePath)
        {
        }

        public PathNodeViewModel(string name, Point[] basePath)
             : base(name)
        {
            _basePath = basePath;

            RefreshPathPoints();
        }


        /// <summary>
        /// The path for the shape being drawn.
        /// </summary>
        public IEnumerable<Point> Path
        {
            get { return _pathCollection; }
        }


        private void RefreshPathPoints()
        {
            _pathCollection = new List<Point>();

            foreach (var point in _basePath)
            {
                _pathCollection.Add(MakePointScaleToSize(point));
            }

            //Notify of change
            OnPropertyChanged("Path");
        }


        private Tuple<Point, Point>[] GetRelativeLines(Point relativeCentre)
        {
            //Guard
            if (Path == null || Path.Count() < 2)
                return new Tuple<Point,Point>[0];

            List<Tuple<Point, Point>> lines = new List<Tuple<Point, Point>>();

            Point previous = Path.First();

            //Add lines from start + 1
            foreach (var current in Path.Skip(1))
            {
                lines.Add(new Tuple<Point, Point>(
                    MakePointRelative(previous),
                    MakePointRelative(current)));

                previous = current;
            }

            return lines.ToArray();
        }

        private Point MakePointScaleToSize(Point p)
        {
            var x = p.X * Width;
            var y = p.Y * Height;

            return new Point(x, y);
        }

        private Point MakePointRelative(Point p)
        {
            return new Point(p.X + X, p.Y + Y);
        }

        private Point MakePointUnrelative(Point p)
        {
            return new Point(p.X - X, p.Y - Y);
        }


        protected override double GetBoundryDistance(Point target)
        {
            //Calculate Source Point
            var source = this.GetRelativeCentre();

            Point currentBest = new Point(0, 0);
            double currentBestSd = double.NaN;
            double currentBestTd = double.NaN;

            // For each line on path
            var lines = GetRelativeLines(source);
            foreach (var line in lines)
            {
                //Unpack Line variables
                var lineFrom = line.Item1;
                var lineTo = line.Item2;

                //Determine Intersection (if any)
                var i = Geometry.GetIntersectionOfTwoLines(
                                            source.X, source.Y,
                                            target.X, target.Y,
                                            lineFrom.X, lineFrom.Y,
                                            lineTo.X, lineTo.Y);

                //If we don't have an intersection (Parallel lines) - skip.
                if (i == null)
                    continue;

                var intersection = i.Value;

                //If the intersection is outside of bounds - skip.
                if (!Geometry.IsIntersectionOnLine(lineFrom, lineTo, intersection))
                    continue;

                //Get Distance from Source
                var sd = Geometry.GetLengthOfLine(
                    source.X, source.Y,
                    intersection.X, intersection.Y);

                //Get Distance from Target
                var td = Geometry.GetLengthOfLine(
                    target.X, target.Y,
                    intersection.X, intersection.Y);

                //Test if intersection is new closest and replace
                if (double.IsNaN(currentBestTd) ||
                    td < currentBestTd)
                {
                    currentBest = intersection;
                    currentBestSd = sd;
                    currentBestTd = td;
                }
            }

            return double.IsNaN(currentBestSd) ? 0 : currentBestSd;
        }

        protected override void OnSizeChanged()
        {
            RefreshPathPoints();
            UpdateConnectorPositions();
        }
    }
}
