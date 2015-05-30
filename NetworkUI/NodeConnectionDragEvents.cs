using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;

namespace NetworkUI
{
    public class NodeConnectionCreatedEventArgs : RoutedEventArgs
    {
        public NodeConnectionCreatedEventArgs(
            RoutedEvent routedEvent, object source,
            object from, object to) :
            base(routedEvent, source)
        {
            From = from;
            To = to;
        }


        /// <summary>
        /// The DataContext for the drag source
        /// </summary>
        public object From { get; private set; }

        /// <summary>
        /// The DataContext for the drop target
        /// </summary>
        public object To { get; private set; }
    }

    public delegate void NodeConnectionCreatedEventHandler(object sender, NodeConnectionCreatedEventArgs e);


    public class NodeConnectionDraggingEventArgs : RoutedEventArgs
    {
        public NodeConnectionDraggingEventArgs(
            RoutedEvent routedEvent, object source,
            object fromItem, Point toPoint) :
            base(routedEvent, source)
        {
            FromNodeItem = fromItem;
            ToPoint = toPoint;
        }

        /// <summary>
        /// The DataContext for the drag source
        /// </summary>
        public object FromNodeItem { get; private set; }

        /// <summary>
        /// The position of the drag location
        /// </summary>
        public Point ToPoint { get; private set; }
    }

    public delegate void NodeConnectionDraggingEventHandler(object sender, NodeConnectionDraggingEventArgs e);
}
