namespace SeeGit.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using GraphSharp.Controls;

    public class EdgeLabelControl : ContentControl
    {
        public EdgeLabelControl()
        {
            LayoutUpdated += EdgeLabelControl_LayoutUpdated;
        }

        private EdgeControl GetEdgeControl(DependencyObject parent)
        {
            while (parent != null)
                if (parent is EdgeControl)
                    return (EdgeControl)parent;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            return null;
        }

        private static double GetAngleBetweenPoints(Point point1, Point point2)
        {
            return Math.Atan2(point1.Y - point2.Y, point2.X - point1.X);
        }

        private static double GetDistanceBetweenPoints(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }

        private static double GetLabelDistance(double edgeLength)
        {
            return edgeLength / 2; // set the label halfway the length of the edge
        }

        private void EdgeLabelControl_LayoutUpdated(object sender, EventArgs e)
        {
            if (!IsLoaded)
                return;
            var edgeControl = GetEdgeControl(VisualParent);
            if (edgeControl == null)
                return;
            var source = edgeControl.Source;
            var p1 = new Point(GraphCanvas.GetX(source), GraphCanvas.GetY(source));
            var target = edgeControl.Target;
            var p2 = new Point(GraphCanvas.GetX(target), GraphCanvas.GetY(target));

            double edgeLength;
            var routePoints = edgeControl.RoutePoints;
            if (routePoints == null)
                // the edge is a single segment (p1,p2)
                edgeLength = GetLabelDistance(GetDistanceBetweenPoints(p1, p2));
            else
            {
                // the edge has one or more segments
                // compute the total length of all the segments
                edgeLength = 0;
                for (var i = 0; i <= routePoints.Length; ++i)
                    if (i == 0)
                        edgeLength += GetDistanceBetweenPoints(p1, routePoints[0]);
                    else if (i == routePoints.Length)
                        edgeLength += GetDistanceBetweenPoints(routePoints[routePoints.Length - 1], p2);
                    else
                        edgeLength += GetDistanceBetweenPoints(routePoints[i - 1], routePoints[i]);
                // find the line segment where the half distance is located
                edgeLength = GetLabelDistance(edgeLength);
                var newp1 = p1;
                var newp2 = p2;
                for (var i = 0; i <= routePoints.Length; ++i)
                {
                    double lengthOfSegment;
                    if (i == 0)
                        lengthOfSegment = GetDistanceBetweenPoints(newp1 = p1, newp2 = routePoints[0]);
                    else if (i == routePoints.Length)
                        lengthOfSegment = GetDistanceBetweenPoints(newp1 = routePoints[routePoints.Length - 1], newp2 = p2);
                    else
                        lengthOfSegment = GetDistanceBetweenPoints(newp1 = routePoints[i - 1], newp2 = routePoints[i]);
                    if (lengthOfSegment >= edgeLength)
                        break;
                    edgeLength -= lengthOfSegment;
                }
                // redefine our edge points
                p1 = newp1;
                p2 = newp2;
            }
            // align the point so that it  passes through the center of the label content
            var p = p1;
            var desiredSize = DesiredSize;
            if (this.Content?.ToString() == "/")
            {
                // hard to see solo / (below every commit) so offset it slightly from the line
                // enable Commit Content to see this
                p.Offset(-desiredSize.Width / 2 - 7, -desiredSize.Height / 2);
            }
            else
            {
                p.Offset(-desiredSize.Width / 2, -desiredSize.Height / 2);
            }

            // TODO hover file => `git show a3cb` its content if its plaintext?


            // move it "edgLength" on the segment
            var angleBetweenPoints = GetAngleBetweenPoints(p1, p2);
            p.Offset(edgeLength * Math.Cos(angleBetweenPoints), -edgeLength * Math.Sin(angleBetweenPoints));
            Arrange(new Rect(p, desiredSize));
        }
    }
}