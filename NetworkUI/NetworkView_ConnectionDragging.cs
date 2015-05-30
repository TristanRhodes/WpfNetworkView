﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;
using Utils;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;

namespace NetworkUI
{
    /// <summary>
    /// Partial definition of the NetworkView class.
    /// This file only contains private members related to dragging connections.
    /// </summary>
    public partial class NetworkView
    {
        /// <summary>
        /// When dragging a connection, this is set to the ConnectorItem that was initially dragged out.
        /// </summary>
        private ConnectorItem _draggedOutConnectorItem = null;

        /// <summary>
        /// The view-model object for the connector that has been dragged out.
        /// </summary>
        private object _draggedOutConnectorDataContext = null;

        /// <summary>
        /// The view-model object for the node whose connector was dragged out.
        /// </summary>
        private object _draggedOutNodeDataContext = null;

        /// <summary>
        /// The view-model object for the connection that is currently being dragged, or null if none being dragged.
        /// </summary>
        private object _draggingConnectionDataContext = null;

        /// <summary>
        /// A reference to the feedback adorner that is currently in the adorner layer, or null otherwise.
        /// It is used for feedback when dragging a connection over a prospective connector.
        /// </summary>
        private FrameworkElementAdorner _feedbackAdorner = null;


        /// <summary>
        /// Event raised when the user starts to drag a connector.
        /// </summary>
        private void ConnectorItem_DragStarted(object source, ConnectorItemDragStartedEventArgs e)
        {
            this.Focus();

            e.Handled = true;

            this.IsDragging = true;
            this.IsNotDragging = false;
            this.IsDraggingConnection = true;
            this.IsNotDraggingConnection = false;

            this._draggedOutConnectorItem = (ConnectorItem)e.OriginalSource;
            var nodeItem = this._draggedOutConnectorItem.ParentNodeItem;
            this._draggedOutNodeDataContext = nodeItem.DataContext != null ? nodeItem.DataContext : nodeItem;
            this._draggedOutConnectorDataContext = this._draggedOutConnectorItem.DataContext != null ? this._draggedOutConnectorItem.DataContext : this._draggedOutConnectorItem;

            //
            // Raise an event so that application code can create a connection and
            // add it to the view-model.
            //
            ConnectionDragStartedEventArgs eventArgs = new ConnectionDragStartedEventArgs(ConnectionDragStartedEvent, this, this._draggedOutNodeDataContext, this._draggedOutConnectorDataContext);
            RaiseEvent(eventArgs);

            //
            // Retrieve the the view-model object for the connection was created by application code.
            //
            this._draggingConnectionDataContext = eventArgs.Connection;

            if (_draggingConnectionDataContext == null)
            {
                //
                // Application code didn't create any connection.
                //
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Event raised while the user is dragging a connector.
        /// </summary>
        private void ConnectorItem_Dragging(object source, ConnectorItemDraggingEventArgs e)
        {
            e.Handled = true;

            Trace.Assert((ConnectorItem)e.OriginalSource == this._draggedOutConnectorItem);

            Point mousePoint = Mouse.GetPosition(this);

            //
            // Raise an event so that application code can compute intermediate connection points.
            //
            var connectionDraggingEventArgs =
                new ConnectionDraggingEventArgs(
                        ConnectionDraggingEvent, 
                        this, 
                        this._draggedOutNodeDataContext, 
                        this._draggingConnectionDataContext, 
                        this._draggedOutConnectorDataContext);

            RaiseEvent(connectionDraggingEventArgs);

            //
            // Figure out if the connection has been dragged over a connector.
            //
            ConnectorItem connectorDraggedOver = null;
            object connectorDataContextDraggedOver = null;
            bool dragOverSuccess = DetermineConnectorItemDraggedOver(mousePoint, out connectorDraggedOver, out connectorDataContextDraggedOver);
            if (connectorDraggedOver != null)
            {
                // Raise an event so that application code can specify if the connector
                // that was dragged over is valid or not.
                var queryFeedbackEventArgs = 
                    new QueryConnectionFeedbackEventArgs(QueryConnectionFeedbackEvent, this, this._draggedOutNodeDataContext, this._draggingConnectionDataContext, 
                            this._draggedOutConnectorDataContext, connectorDataContextDraggedOver);

                RaiseEvent(queryFeedbackEventArgs);

                if (queryFeedbackEventArgs.FeedbackIndicator != null)
                {
                    // A feedback indicator was specified by the event handler.
                    // This is used to indicate whether the connection is good or bad!
                    AddFeedbackAdorner(connectorDraggedOver, queryFeedbackEventArgs.FeedbackIndicator);
                }
                else
                {
                    // No feedback indicator specified by the event handler.
                    // Clear any existing feedback indicator.
                    ClearFeedbackAdorner();
                }
            }
            else
            {
                // Didn't drag over any valid connector.
                // Clear any existing feedback indicator.
                ClearFeedbackAdorner();
            }
        }

        /// <summary>
        /// Event raised when the user has finished dragging a connector.
        /// </summary>
        private void ConnectorItem_DragCompleted(object source, ConnectorItemDragCompletedEventArgs e)
        {
            e.Handled = true;

            Trace.Assert((ConnectorItem)e.OriginalSource == this._draggedOutConnectorItem);

            Point mousePoint = Mouse.GetPosition(this);

            //
            // Figure out if the end of the connection was dropped on a connector.
            //
            ConnectorItem connectorDraggedOver = null;
            object connectorDataContextDraggedOver = null;
            DetermineConnectorItemDraggedOver(mousePoint, out connectorDraggedOver, out connectorDataContextDraggedOver);

            //
            // Now that connection dragging has completed, don't any feedback adorner.
            //
            ClearFeedbackAdorner();

            //
            // Raise an event to inform application code that connection dragging is complete.
            // The application code can determine if the connection between the two connectors
            // is valid and if so it is free to make the appropriate connection in the view-model.
            //
            RaiseEvent(new ConnectionDragCompletedEventArgs(ConnectionDragCompletedEvent, this, this._draggedOutNodeDataContext, this._draggingConnectionDataContext, this._draggedOutConnectorDataContext, connectorDataContextDraggedOver));

            this.IsDragging = false;
            this.IsNotDragging = true;
            this.IsDraggingConnection = false;
            this.IsNotDraggingConnection = true;
            this._draggedOutConnectorDataContext = null;
            this._draggedOutNodeDataContext = null;
            this._draggedOutConnectorItem = null;
            this._draggingConnectionDataContext = null;
        }


        /// <summary>
        /// This function does a hit test to determine which connector, if any, is under 'hitPoint'.
        /// </summary>
        private bool DetermineConnectorItemDraggedOver(Point hitPoint, out ConnectorItem connectorItemDraggedOver, out object connectorDataContextDraggedOver)
        {
            connectorItemDraggedOver = null;
            connectorDataContextDraggedOver = null;

            //
            // Run a hit test 
            //
            HitTestResult result = null;
            VisualTreeHelper.HitTest(_nodeItemsControl, null, 
                //
                // Result callback delegate.
                // This method is called when we have a result.
                //
                delegate(HitTestResult hitTestResult)
                {
                    result = hitTestResult;

                    return HitTestResultBehavior.Stop;
                },
                new PointHitTestParameters(hitPoint));

            if (result == null || result.VisualHit == null)
            {
                // Hit test failed.
                return false;
            }

            //
            // Actually want a reference to a 'ConnectorItem'.  
            // The hit test may have hit a UI element that is below 'ConnectorItem' so
            // search up the tree.
            //
            var hitItem = result.VisualHit as FrameworkElement;
            if (hitItem == null)
            {
                return false;
            }
            var connectorItem = WpfUtils.FindVisualParentWithType<ConnectorItem>(hitItem);
			if (connectorItem == null)
            {
                return false;
            }

            var networkView = connectorItem.ParentNetworkView;
            if (networkView != this)
            {
                //
                // Ensure that dragging over a connector in another NetworkView doesn't
                // return a positive result.
                //
                return false;
            }

            object connectorDataContext = connectorItem;
            if (connectorItem.DataContext != null)
            {
                //
                // If there is a data-context then grab it.
                // When we are using a view-model then it is the view-model
                // object we are interested in.
                //
                connectorDataContext = connectorItem.DataContext;
            }

            connectorItemDraggedOver = connectorItem;
            connectorDataContextDraggedOver = connectorDataContext;

            return true;
        }
         
        /// <summary>
        /// Add a feedback adorner to a UI element.
        /// This is used to show when a connection can or can't be attached to a particular connector.
        /// 'indicator' will be a view-model object that is transformed into a UI element using a data-template.
        /// </summary>
        private void AddFeedbackAdorner(FrameworkElement adornedElement, object indicator)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

            if (_feedbackAdorner != null)
            {
                if (_feedbackAdorner.AdornedElement == adornedElement)
                {
                    // No change.
                    return;
                }

                adornerLayer.Remove(_feedbackAdorner);
                _feedbackAdorner = null;
            }

            //
            // Create a content control to contain 'indicator'.
            // The view-model object 'indicator' is transformed into a UI element using
            // normal WPF data-template rules.
            //
            ContentControl adornerElement = new ContentControl();
            adornerElement.HorizontalAlignment = HorizontalAlignment.Center;
            adornerElement.VerticalAlignment = VerticalAlignment.Center;
            adornerElement.Content = indicator;

            //
            // Create the adorner and add it to the adorner layer.
            //
            _feedbackAdorner = new FrameworkElementAdorner(adornerElement, adornedElement);
            adornerLayer.Add(_feedbackAdorner);
        }

        /// <summary>
        /// If there is an existing feedback adorner, remove it.
        /// </summary>
        private void ClearFeedbackAdorner()
        {
            if (_feedbackAdorner == null)
            {
                return;
            }

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
            adornerLayer.Remove(_feedbackAdorner);
            _feedbackAdorner = null;
        }

    }
}
