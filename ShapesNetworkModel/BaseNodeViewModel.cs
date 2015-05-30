using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Utils;
using System.Windows;

namespace ShapesNetworkModel
{
    /// <summary>
    /// Defines a node in the view-model.
    /// Nodes are connected to other nodes through attached connectors (aka connection points).
    /// </summary>
    public abstract class BaseNodeViewModel : AbstractModelBase, IResizableModel
    {
        /// <summary>
        /// The name of the node.
        /// </summary>
        private string _name = string.Empty;

        /// <summary>
        /// The X coordinate for the position of the node.
        /// </summary>
        private double _x = 0;

        /// <summary>
        /// The Y coordinate for the position of the node.
        /// </summary>
        private double _y = 0;


        /// <summary>
        /// Width of the shape control.
        /// </summary>
        private double _width = 50;

        /// <summary>
        /// Height of the shape control.
        /// </summary>
        private double _height = 50;


        /// <summary>
        /// Minimum Size of the shape
        /// </summary>
        private double _minWidth = 50;

        /// <summary>
        /// Maximum size of the shape
        /// </summary>
        private double _maxWidth = 800;


        /// <summary>
        /// Minimum height of the shape.
        /// </summary>
        private double _minHeight = 50;

        /// <summary>
        /// Maximum height of the shape
        /// </summary>
        private double _maxHeight = 800;


        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        private bool _isSelected = false;

        /// <summary>
        /// List of input connectors (connections points) attached to the node.
        /// </summary>
        private ImpObservableCollection<ConnectorViewModel> _connectors = null;

        /// <summary>
        /// Connector for use with new Connections origionating from the node.
        /// </summary>
        private ConnectorViewModel _linkCreationConnector;


        public BaseNodeViewModel()
        {
            Initialize();
        }

        public BaseNodeViewModel(string name)
        {
            this._name = name;

            Initialize();
        }


        private void Initialize()
        {
            _linkCreationConnector = new ConnectorViewModel();
            _linkCreationConnector.ParentNode = this;
        }


        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;

                OnPropertyChanged("Name");
            }
        }


        /// <summary>
        /// The X coordinate for the position of the node.
        /// </summary>
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                if (_x == value)
                {
                    return;
                }

                _x = value;

                OnPropertyChanged("X");
                OnNodePositionChanged();
            }
        }

        /// <summary>
        /// The Y coordinate for the position of the node.
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (_y == value)
                {
                    return;
                }

                _y = value;

                OnPropertyChanged("Y");
                OnNodePositionChanged();
            }
        }


        /// <summary>
        /// The width of the node
        /// </summary>
        public double Width
        {
            get { return _width; }
            set
            {
                if (value < MinWidth)
                    value = MinWidth;

                if (value > MaxWidth)
                    value = MaxWidth;

                if (_width == value)
                    return;

                _width = value;
                OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Minimum width of the shape
        /// </summary>
        public double MinWidth
        {
            get { return _minWidth; }
            set
            {
                if (_minWidth == value)
                    return;

                _minWidth = value;
                OnPropertyChanged("MinWidth");
            }
        }

        /// <summary>
        /// Maximum width of the shape
        /// </summary>
        public double MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                if (_maxWidth == value)
                    return;

                _maxWidth = value;
                OnPropertyChanged("MaxWidth");
            }
        }


        /// <summary>
        /// The height of the node
        /// </summary>
        public double Height
        {
            get { return _height; }
            set
            {
                if (value < MinHeight)
                    value = MinHeight;

                if (value > MaxHeight)
                    value = MaxHeight;

                if (_height == value)
                    return;

                _height = value;
                OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Minimum width of the shape
        /// </summary>
        public double MinHeight
        {
            get { return _minHeight; }
            set
            {
                if (_minHeight == value)
                    return;

                _minHeight = value;
                OnPropertyChanged("MinHeight");
            }
        }

        /// <summary>
        /// Maximum width of the shape
        /// </summary>
        public double MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                if (_maxHeight == value)
                    return;

                _maxHeight = value;
                OnPropertyChanged("MaxHeight");
            }
        }


        /// <summary>
        /// Permenant ViewModel for use when creating new links with this node as a source.
        /// </summary>
        public ConnectorViewModel LinkCreationConnection
        {
            get { return _linkCreationConnector; }
        }


        /// <summary>
        /// List of connectors (connection anchor points) attached to the node.
        /// </summary>
        public ImpObservableCollection<ConnectorViewModel> Connectors
        {
            get
            {
                if (_connectors == null)
                {
                    _connectors = new ImpObservableCollection<ConnectorViewModel>();
                    _connectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(connectors_ItemsAdded);
                    _connectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(connectors_ItemsRemoved);
                }

                return _connectors;
            }
        }

        /// <summary>
        /// A helper property that retrieves a list (a new list each time) of all connections attached to the node. 
        /// </summary>
        public ICollection<ConnectionViewModel> AttachedConnections
        {
            get
            {
                List<ConnectionViewModel> attachedConnections = new List<ConnectionViewModel>();

                foreach (var connector in this.Connectors)
                {
                    if (connector.AttachedConnection != null)
                        attachedConnections.Add(connector.AttachedConnection);
                }

                return attachedConnections;
            }
        }


        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value)
                {
                    return;
                }

                _isSelected = value;

                OnPropertyChanged("IsSelected");
            }
        }


        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void connectors_ItemsAdded(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = this;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void connectors_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = null;
            }
        }


        /// <summary>
        /// Call when node position is changed.
        /// </summary>
        private void OnNodePositionChanged()
        {
            if (NodePositionChanged != null)
                NodePositionChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when the node position is changed.
        /// </summary>
        public event EventHandler NodePositionChanged;


        /// <summary>
        /// Update all connectors attached to this node.
        /// </summary>
        public void UpdateConnectorPositions()
        {
            foreach (var sourceConn in this.Connectors)
            {
                //Get target connection point
                var connection = sourceConn.AttachedConnection;
                var targetConn = (ConnectorViewModel)null;

                if (connection.SourceConnector == sourceConn)
                    targetConn = connection.DestConnector;
                else
                    targetConn = connection.SourceConnector;

                //Adust connection between source and target
                AdjustConnectorPositions(sourceConn, targetConn);
            }
        }

        /// <summary>
        /// Update the position of the LinkCreationConnector relative to the target point.
        /// </summary>
        /// <param name="dragPoint"></param>
        public void UpdateConnectorPositionForNewLink(Point dragPoint)
        {
            //Get relative centre and boundry distance
            var sourceCentrePoint = GetRelativeCentre();
            var sourceBoundryDistance = GetBoundryDistance(dragPoint);

            //Get the length of the line between the two points
            var lineLength = Geometry.GetLengthOfLine(sourceCentrePoint.X, sourceCentrePoint.Y,
                                                dragPoint.X, dragPoint.Y);

            //Get distance to the item boundry
            var borderDistance = -lineLength + sourceBoundryDistance;

            //Get the position of the revised source
            var revisedSource = Geometry.GetPointAtDistanceOnLine(
                                    sourceCentrePoint.X,
                                    sourceCentrePoint.Y,
                                    dragPoint.X,
                                    dragPoint.Y,
                                    borderDistance);

            //Set the source connection position
            var conn = this.LinkCreationConnection;
            conn.XConnectionPoint = revisedSource.X;
            conn.YConnectionPoint = revisedSource.Y;
        }


        /// <summary>
        /// Method used for calculating how far from the centre point the line from the target intersects with the shape boundry.
        /// </summary>
        /// <param name="target">Target to calculate the boundry distance from the centre.</param>
        /// <returns>The distance from the boundry.</returns>
        protected abstract double GetBoundryDistance(Point target);

        /// <summary>
        /// Called when the shapes size is modified.
        /// </summary>
        protected abstract void OnSizeChanged();


        /// <summary>
        /// Override property changed and handle Width and Height change so we can update the connector positions.
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "Width" || propertyName == "Height")
                OnSizeChanged();
        }


        /// <summary>
        /// Get the relative centre of the shape.
        /// </summary>
        /// <returns></returns>
        protected Point GetRelativeCentre()
        {
            return Geometry.GetRelativeCentre(this.X, this.Y, this.Width, this.Height);
        }


        /// <summary>
        /// Static utility method to update a pair of connectors relative to each others.
        /// </summary>
        /// <param name="sourceConn"></param>
        /// <param name="targetConn"></param>
        public static void AdjustConnectorPositions(ConnectorViewModel sourceConn, ConnectorViewModel targetConn)
        {
            //Get Nodes
            var sourceNode = sourceConn.ParentNode;
            var targetNode = targetConn.ParentNode;

            //Get Centre of nodes
            var sourceCentrePoint = sourceNode.GetRelativeCentre();
            var targetCentrePoint = targetNode.GetRelativeCentre();

            //Get Boundry distance for node
            var sourceBoundryDistance = sourceNode.GetBoundryDistance(targetCentrePoint);
            var targetBoundryDistance = targetNode.GetBoundryDistance(sourceCentrePoint);

            //Set Source Connector
            var sourcePoint = Geometry.GetPointAtDistanceOnLine(
                                    sourceCentrePoint.X, sourceCentrePoint.Y,
                                    targetCentrePoint.X, targetCentrePoint.Y,
                                    -targetBoundryDistance);
            sourceConn.XConnectionPoint = sourcePoint.X;
            sourceConn.YConnectionPoint = sourcePoint.Y;

            //Set Target Connector
            var targetPoint = Geometry.GetPointAtDistanceOnLine(
                                    targetCentrePoint.X, targetCentrePoint.Y,
                                    sourceCentrePoint.X, sourceCentrePoint.Y,
                                    -sourceBoundryDistance);
            targetConn.XConnectionPoint = targetPoint.X;
            targetConn.YConnectionPoint = targetPoint.Y;
        }
    }
}
