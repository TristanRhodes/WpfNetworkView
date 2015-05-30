using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ShapesNetworkModel
{
    internal class SquareShapeViewModel : PathNodeViewModel
    {
        private static Point[] _points = new Point[]
        {
            new Point(0.0,0.0),
            new Point(1.0, 0.0),
            new Point(1.0, 1.0),
            new Point(0.0, 1.0),
            new Point(0.0,0.0)
        };

        internal SquareShapeViewModel()
            : this(null) { }

        internal SquareShapeViewModel(string name)
            : base(name, _points) { }
    }

    internal class HexagonShapeViewModel : PathNodeViewModel
    {
        private static Point[] _points = new Point[]
        {
            new Point(0.0, 0.5), 
            new Point(0.25, 0.0),
            new Point(0.75, 0.0),
            new Point(1.0, 0.5), 
            new Point(0.75, 1.0),
            new Point(0.25, 1.0),
            new Point(0.0, 0.5)
        };

        internal HexagonShapeViewModel()
            : this(null) { }

        internal HexagonShapeViewModel(string name)
            : base(name, _points) { }
    }

    internal class TriangleShapeViewModel : PathNodeViewModel
    {
        private static Point[] _points = new Point[]
        {
            new Point(0.0, 0.0),
            new Point(1.0, 0.0),
            new Point(0.5, 1.0),
            new Point(0.0, 0.0)
        };

        internal TriangleShapeViewModel()
            : this(null) { }

        internal TriangleShapeViewModel(string name)
            : base(name, _points) { }
    }

    internal class DiamondShapeViewModel : PathNodeViewModel
    {
        private static Point[] _points = new Point[]
        {
            new Point(0.5, 0.0),
            new Point(1.0, 0.5),
            new Point(0.5, 1.0),
            new Point(0.0, 0.5),
            new Point(0.5, 0.0)
        };

        internal DiamondShapeViewModel()
            : this(null) { }

        internal DiamondShapeViewModel(string name)
            : base(name, _points) { }
    }

    internal class StarShapeViewModel : PathNodeViewModel
    {
        private static Point[] _points = new Point[]
        {
            new Point(0.0, 0.3),
            new Point(0.4, 0.3),
            new Point(0.5, 0.0),
            new Point(0.6, 0.3),
            new Point(1.0, 0.3),
            new Point(0.7, 0.56),
            new Point(0.8, 1.0),
            new Point(0.5, 0.7),
            new Point(0.2, 1.0),
            new Point(0.3, 0.56),
            new Point(0.0, 0.30)
        };

        internal StarShapeViewModel()
            : this(null) { }

        internal StarShapeViewModel(string name)
            : base(name, _points) { }
    }
}
