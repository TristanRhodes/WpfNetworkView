using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using ShapesNetworkModel;

namespace ShapesSample.Controls
{
    /// <summary>
    /// Thumb class for use as a resize anchor. Operates on models which implement IResizableModel.
    /// </summary>
    public class ResizeThumb : Thumb
    {
        private IResizableModel _model;

        public ResizeThumb()
        {
            this.DataContextChanged += new System.Windows.DependencyPropertyChangedEventHandler(ResizeThumb_DataContextChanged);
            this.DragDelta += new DragDeltaEventHandler(ResizeThumbComponent_DragDelta);
        }


        void ResizeThumb_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var model = e.NewValue as IResizableModel;

            if (model == null)
                return;

            _model = model;
        }

        void ResizeThumbComponent_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Guard
            if (_model == null)
                return;

            //Horizontal Aspect
            if (this.HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                var newWidth = _model.Width - e.HorizontalChange;

                if (_model.MinWidth < newWidth &&
                    newWidth < _model.MaxWidth)
                {
                    _model.X += e.HorizontalChange;
                    _model.Width = newWidth;
                }
            }
            else if (this.HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                _model.Width += e.HorizontalChange;
            }

            //Vertical Aspect
            if (this.VerticalAlignment == System.Windows.VerticalAlignment.Top)
            {
                var newHeight = _model.Height - e.VerticalChange;

                if (_model.MinHeight < newHeight &&
                    newHeight < _model.MaxHeight)
                {
                    _model.Y += e.VerticalChange;
                    _model.Height = newHeight;
                }
            }
            else if (this.VerticalAlignment == System.Windows.VerticalAlignment.Bottom)
            {
                _model.Height += e.VerticalChange;
            }

        }
    }
}
