using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Utils;
using System.Windows;
using System.Windows.Shapes;

namespace ShapesNetworkModel
{
    /// <summary>
    /// Defines a network of nodes and connections between the nodes.
    /// </summary>
    public sealed class NetworkViewModel
    {
        /// <summary>
        /// The collection of nodes in the network.
        /// </summary>
        private ImpObservableCollection<BaseNodeViewModel> _nodes = null;

        /// <summary>
        /// The collection of connections in the network.
        /// </summary>
        private ImpObservableCollection<ConnectionViewModel> _connections = null;


        /// <summary>
        /// The collection of nodes in the network.
        /// </summary>
        public ImpObservableCollection<BaseNodeViewModel> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new ImpObservableCollection<BaseNodeViewModel>();
                }

                return _nodes;
            }
        }

        /// <summary>
        /// The collection of connections in the network.
        /// </summary>
        public ImpObservableCollection<ConnectionViewModel> Connections
        {
            get
            {
                if (_connections == null)
                {
                    _connections = new ImpObservableCollection<ConnectionViewModel>();
                    _connections.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(connections_ItemsRemoved);
                }

                return _connections;
            }
        }

        public BaseNodeViewModel CreateNode(string name, Point nodeLocation, int width, int height, ShapeType shape)
        {
            //Create node
            BaseNodeViewModel node = CreateShapeNode(name, shape);
            node.Width = width;
            node.Height = height;
            node.X = nodeLocation.X;
            node.Y = nodeLocation.Y;

            // Add the new node to the view-model.
            this.Nodes.Add(node);

            //Wire up event
            node.NodePositionChanged += new EventHandler(node_NodePositionChanged);

            return node;
        }

        private BaseNodeViewModel CreateShapeNode(string name, ShapeType shape)
        {
            switch (shape)
            {
                case ShapeType.Ellipse:
                    return new EllipseNodeViewModel(name);

                case ShapeType.Square:
                    return new SquareShapeViewModel(name);

                case ShapeType.Triangle:
                    return new TriangleShapeViewModel(name);

                case ShapeType.Diamond:
                    return new DiamondShapeViewModel(name);

                case ShapeType.Hexagon:
                    return new HexagonShapeViewModel(name);

                case ShapeType.Star:
                    return new StarShapeViewModel(name);

                default:
                    throw new ApplicationException("Unhandled Shape: " + shape);
            }
        }

        /// <summary>
        /// Delete the node from the view-model.
        /// Also deletes any connections to or from the node.
        /// </summary>
        public void DeleteNode(BaseNodeViewModel node)
        {
            foreach (var conn in node.AttachedConnections)
            {
                var targetConnector = (ConnectorViewModel)null;

                if (conn.SourceConnector.ParentNode != node)
                    targetConnector = conn.SourceConnector;
                else
                    targetConnector = conn.DestConnector;

                //Remove from target node
                var targetNode = targetConnector.ParentNode;
                targetNode.Connectors.Remove(targetConnector);
            }

            // Remove all connections attached to the node.
            this.Connections.RemoveRange(node.AttachedConnections);


            // Remove the node from the network.
            this.Nodes.Remove(node);

            node.NodePositionChanged -= new EventHandler(node_NodePositionChanged);
        }


        /// <summary>
        /// Create a connection between two nodes.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public ConnectionViewModel CreateConnection(BaseNodeViewModel from, BaseNodeViewModel to)
        {
            //From Connector
            var fromConnector = new ConnectorViewModel();
            from.Connectors.Add(fromConnector);

            //To Connector
            var toConnector = new ConnectorViewModel();
            to.Connectors.Add(toConnector);

            //Create and Add Connection
            var connection = new ConnectionViewModel();
            connection.SourceConnector = fromConnector;
            connection.DestConnector = toConnector;
            this.Connections.Add(connection);

            BaseNodeViewModel.AdjustConnectorPositions(fromConnector, toConnector);

            return connection;
        }

        /// <summary>
        /// Delete a connection
        /// </summary>
        /// <param name="connection"></param>
        public void DeleteConnection(ConnectionViewModel connection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event raised then Connections have been removed.
        /// </summary>
        private void connections_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectionViewModel connection in e.Items)
            {
                connection.SourceConnector = null;
                connection.DestConnector = null;
            }
        }


        private void node_NodePositionChanged(object sender, EventArgs e)
        {
            UpdateConnectorPositionsForNode((BaseNodeViewModel)sender);
        }


        private void UpdateConnectorPositionsForNode(BaseNodeViewModel model)
        {
            model.UpdateConnectorPositions();
        }
    }
}
