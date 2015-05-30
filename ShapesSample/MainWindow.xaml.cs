using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetworkUI;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using Utils;
using System.Collections;
using ShapesNetworkModel;

namespace ShapesSample
{
    /// <summary>
    /// This is a Window that uses NetworkView to display a flow-chart.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public MainWindowViewModel ViewModel
        {
            get
            {
                return (MainWindowViewModel)this.DataContext;
            }
        }


        /// <summary>
        /// Event raised when the Window has loaded.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Display help text for the sample app.
            HelpTextWindow helpTextWindow = new HelpTextWindow();
            helpTextWindow.Left = this.Left + this.Width + 5;
            helpTextWindow.Top = this.Top;
            helpTextWindow.Owner = this;
            helpTextWindow.Show();
        }


        /// <summary>
        /// Event raised to delete the selected node.
        /// </summary>
        private void DeleteSelectedNodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.DeleteSelectedNodes();
        }


        /// <summary>
        /// Event raised to create a new node.
        /// </summary>
        private void CreateNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Point newNodeLocation = Mouse.GetPosition(networkControl);
            ShapeType newNodeShape = (ShapeType)e.Parameter;
            this.ViewModel.Network.CreateNode("New Node!", newNodeLocation, 50, 50, newNodeShape);
        }

        /// <summary>
        /// Event raised to delete a node.
        /// </summary>
        private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var node = (BaseNodeViewModel)e.Parameter;
            this.ViewModel.DeleteNode(node);
        }

        /// <summary>
        /// Event raised to delete a connection.
        /// </summary>
        private void DeleteConnection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var connection = (ConnectionViewModel)e.Parameter;
            this.ViewModel.DeleteConnection(connection);
        }


        private void beginConnectionDragButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            var data = element.DataContext as BaseNodeViewModel;
            if (data == null)
                return;

            networkControl.BeginNodeConnectionDrag(data);
        }


        private void networkControl_NodeConnectionCreated(object sender, NodeConnectionCreatedEventArgs e)
        {
            ViewModel.Network.CreateConnection((BaseNodeViewModel)e.From, (BaseNodeViewModel)e.To);
        }

        private void networkControl_NodeConnectionDragging(object sender, NodeConnectionDraggingEventArgs e)
        {
            var node = (BaseNodeViewModel)e.FromNodeItem;
            node.UpdateConnectorPositionForNewLink(e.ToPoint);
        }
    }
}
