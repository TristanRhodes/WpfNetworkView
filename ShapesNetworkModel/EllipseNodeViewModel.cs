using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using Utils;

namespace ShapesNetworkModel
{
    public class EllipseNodeViewModel : BaseNodeViewModel
    {
        public EllipseNodeViewModel()
        {
        }

        public EllipseNodeViewModel(string name)
             : base(name)
        {
        }


        protected override double GetBoundryDistance(Point target)
        {
            var source = this.GetRelativeCentre();
            var i = Geometry.GetEllipseIntersections(Width, Height, target.X - source.X, target.Y - source.Y);
            var d = Geometry.GetLengthOfLine(0, 0, i.X, i.Y);
            return d;
        }

        protected override void OnSizeChanged()
        {
            UpdateConnectorPositions();
        }
    }
}
