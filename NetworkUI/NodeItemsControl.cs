using System;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Input;
using Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;

namespace NetworkUI
{
    /// <summary>
    /// Implements an ListBox for displaying nodes in the NetworkView UI.
    /// </summary>
    internal class NodeItemsControl : ListBox
    {
        public NodeItemsControl()
        {
            //
            // By default, we don't want this UI element to be focusable.
            //
            Focusable = false;
        }

        #region Private Methods

        /// <summary>
        /// Find the NodeItem UI element that has the specified data context.
        /// Return null if no such NodeItem exists.
        /// </summary>
        internal NodeItem FindAssociatedNodeItem(object nodeDataContext)
        {
            return (NodeItem) this.ItemContainerGenerator.ContainerFromItem(nodeDataContext);
        }

        internal NodeItem FindNodeItemAtPosition(Point position)
        {
            var element = this.InputHitTest(position) as UIElement;

            if (element == null)
                return null;

            var data = DependencyProperty.UnsetValue;
            while (data == DependencyProperty.UnsetValue)
            {
                data = this.ItemContainerGenerator.ItemFromContainer(element);

                if (data == DependencyProperty.UnsetValue)
                    element = VisualTreeHelper.GetParent(element) as UIElement;

                if (element == this)
                    return null;
            }

            return data != DependencyProperty.UnsetValue ? element as NodeItem : null;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item. 
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NodeItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container. 
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is NodeItem;
        }

        #endregion Private Methods
    }
}
