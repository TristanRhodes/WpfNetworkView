using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace NetworkUI
{
    public partial class NetworkView
    {
        private NodeItem _currentDragSourceItem = null;
        private ConnectorItem _currentDragSourceConnector = null;

        /// <summary>
        /// The line (PART) that represents the new connection being added.
        /// </summary>
        private Line _dragConnectionLine = null;


        /// <summary>
        /// Begin dragging a connection from a node.
        /// </summary>
        /// <param name="dragSource"></param>
        public void BeginNodeConnectionDrag(object dragSourceItem)
        {
            if (!EnableNodeConnectionDragging)
                return;

            //Get Node Item
            var nodeItem = _nodeItemsControl.FindAssociatedNodeItem(dragSourceItem);
            if (nodeItem == null)
                throw new ApplicationException("Corresponding nodeItem not found.");

            _currentDragSourceItem = nodeItem;
            _currentDragSourceConnector = nodeItem.NewLinkConnector;

            isDraggingNodeConnection = true;

            UpdateDragConnectionLine(Mouse.GetPosition(this));

            this.Focus();

            ShowDragConnectionVisuals();
        }

        /// <summary>
        /// Cancel a node connection dragged from a node.
        /// </summary>
        public void CancelNodeConnectionDrag()
        {
            if (!EnableNodeConnectionDragging)
                return;

            HideConnectionDragVisuals();
        }


        private void UpdateDragConnectionLine(Point point)
        {
            if (!isDraggingNodeConnection)
                return;


            //Get the UI to update the connection dragging anchor
            var args = new NodeConnectionDraggingEventArgs
                (NodeConnectionDraggingEvent, this,
                            _currentDragSourceItem.DataContext,
                            point);
            RaiseEvent(args);


            //Use anchor and drag position to determine where to draw the line.
            _dragConnectionLine.X1 = _currentDragSourceConnector.Hotspot.X;
            _dragConnectionLine.Y1 = _currentDragSourceConnector.Hotspot.Y;

            _dragConnectionLine.X2 = point.X;
            _dragConnectionLine.Y2 = point.Y;
        }

        private void DropNodeConnection(Point point)
        {
            //End Connection Drag
            HideConnectionDragVisuals();

            //Find Drop Target
            var itemAtPoint = _nodeItemsControl.FindNodeItemAtPosition(point);
            if (itemAtPoint == null)
                return;

            //Raise drop event
            RaiseEvent(new NodeConnectionCreatedEventArgs(NodeConnectionCreatedEvent, this,
                _currentDragSourceItem.DataContext, itemAtPoint.DataContext));
        }


        private void ShowDragConnectionVisuals()
        {
                    _dragConnectionLine.Visibility = Visibility.Visible;
                    _currentDragSourceConnector.Visibility = System.Windows.Visibility.Visible;

                    this.IsDragging = true;
                    this.IsNotDragging = false;
                    this.IsDraggingConnection = true;
                    this.IsNotDraggingConnection = false;
        }

        private void HideConnectionDragVisuals()
        {
            isDraggingNodeConnection = false;
            _dragConnectionLine.Visibility = Visibility.Hidden;
            _currentDragSourceConnector.Visibility = System.Windows.Visibility.Hidden;

            this.IsDragging = false;
            this.IsNotDragging = true;

            this.IsDraggingConnection = false;
            this.IsNotDraggingConnection = true;
        }
   }
}
