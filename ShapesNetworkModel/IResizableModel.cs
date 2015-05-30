using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapesNetworkModel
{
    /// <summary>
    /// Interface for any model that is movable.
    /// </summary>
    public interface IMovableModel
    {
        /// <summary>
        /// The X coordinate for the position of the model. This is assumed be bound to Canvas.Top(X)
        /// </summary>
        double X { get; set; }

        /// <summary>
        /// The Y coordinate for the position of the model. This is assumed be bound to Canvas.Top(Y)
        /// </summary>
        double Y { get; set; }
    }

    /// <summary>
    /// Interface for any model that is resizable (and movable)
    /// </summary>
    public interface IResizableModel : IMovableModel
    {
        /// <summary>
        /// The width of the model. This is assumed be bound to Width.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Minimum width of the model
        /// </summary>
        double MinWidth { get; }

        /// <summary>
        /// Maximum width of the model
        /// </summary>
        double MaxWidth { get; }


        /// <summary>
        /// The height of the model. This is assumed to be bound to Height.
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Minimum width of the model
        /// </summary>
        double MinHeight { get; }

        /// <summary>
        /// Maximum width of the model
        /// </summary>
        double MaxHeight { get; }
    }
}
