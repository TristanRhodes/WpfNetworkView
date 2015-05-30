using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using System.Windows;
using System.Windows.Input;
using ShapesNetworkModel;

namespace ShapesSample
{
    /// <summary>
    /// The view-model for the main window.
    /// </summary>
    public class MainWindowViewModel : AbstractModelBase
    {
        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel _network = null;


        public MainWindowViewModel()
        {
            // Add some test data to the view-model.
            PopulateWithTestData();
        }

        private void PopulateWithTestData()
        {
            // Create a network, the root of the view-model.
            this.Network = new NetworkViewModel();

            // Create some shape nodes
            var node1 = _network.CreateNode("Tall Ellipse", new Point(50, 50), 40, 80, ShapeType.Ellipse);
            var node2 = _network.CreateNode("Fat Ellipse", new Point(150, 50), 80, 40, ShapeType.Ellipse);
            var node3 = _network.CreateNode("Circle", new Point(250, 50), 40, 40, ShapeType.Ellipse);
            var node4 = _network.CreateNode("Hex", new Point(350, 50), 60, 40, ShapeType.Hexagon);
            var node5 = _network.CreateNode("Square", new Point(450, 50), 50, 50, ShapeType.Square);
            var node6 = _network.CreateNode("Triangle", new Point(550, 50), 60, 50, ShapeType.Triangle);
            var node7 = _network.CreateNode("Diamond", new Point(50, 150), 80, 80, ShapeType.Diamond);
            var node8 = _network.CreateNode("Star", new Point(150, 150), 100, 100, ShapeType.Star);

            //Create some connections
            _network.CreateConnection(node1, node2);
            _network.CreateConnection(node2, node3);
            _network.CreateConnection(node3, node4);
        }


        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel Network
        {
            get
            {
                return _network;
            }
            set
            {
                _network = value;

                OnPropertyChanged("Network");
            }
        }


        /// <summary>
        /// Delete the currently selected nodes from the view-model.
        /// </summary>
        public void DeleteSelectedNodes()
        {
            // Take a copy of the nodes list so we can delete nodes while iterating.
            var nodesCopy = this.Network.Nodes.ToArray();

            foreach (var node in nodesCopy)
            {
                if (node.IsSelected)
                {
                    _network.DeleteNode(node);
                }
            }
        }

        /// <summary>
        /// Delete node
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(BaseNodeViewModel node)
        {
            //Get Connections
            var connections = node.Connectors
                .Where(c => c.AttachedConnection != null)
                .Select(c => c.AttachedConnection)
                .ToArray();

            //Delete Connections
            foreach (var conn in connections)
            {
                DeleteConnection(conn);
            }

            // Remove the node from the network.
            this.Network.Nodes.Remove(node);
        }

        /// <summary>
        /// Utility method to delete a connection from the view-model.
        /// </summary>
        public void DeleteConnection(ConnectionViewModel connection)
        {
            //Connections
            var fromConnection = connection.SourceConnector;
            var toConnection = connection.DestConnector;

            //Remove Connection
            this.Network.Connections.Remove(connection);

            //Remove Connectors
            fromConnection.ParentNode.Connectors.Remove(fromConnection);
            toConnection.ParentNode.Connectors.Remove(toConnection);
        }
    }
}
