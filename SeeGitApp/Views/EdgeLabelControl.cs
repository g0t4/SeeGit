namespace SeeGit.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using GraphShape.Controls;

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

            double labelLength;
            var routePoints = edgeControl.RoutePoints;
            if (routePoints == null)
                // the edge is a single segment (p1,p2)
                labelLength = GetDistanceBetweenPoints(p1, p2) * 0.5; // half way
            else
            {
                // the edge has one or more segments
                // compute the total length of all the segments
                double totalEdgesLength = 0;
                for (var i = 0; i <= routePoints.Length; ++i)
                    // layout: p1 => ...routePoints => p2
                    if (i == 0)
                        totalEdgesLength += GetDistanceBetweenPoints(p1, routePoints[0]);
                    else if (i == routePoints.Length)
                        totalEdgesLength += GetDistanceBetweenPoints(routePoints[routePoints.Length - 1], p2);
                    else
                        totalEdgesLength += GetDistanceBetweenPoints(routePoints[i - 1], routePoints[i]);

                // find the line segment where the half distance is located
                labelLength = totalEdgesLength * 0.5; // half way
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
                    if (lengthOfSegment >= labelLength)
                        // means label position is w/in this segment (newp1/newp2 are captured by this break)
                        break;
                    // subtract out each segment length until we find the segment within which the half way point resides
                    labelLength -= lengthOfSegment;
                }
                // redefine our edge points
                p1 = newp1;
                p2 = newp2;
            }

            // align the point so that it  passes through the center of the label content
            // hard to see solo / (below every commit) so offset it slightly from the line
            // enable Commit Content to see this
            var shift = this.Content?.ToString() == "/" ? 7 : 0;
            var desiredSize = DesiredSize;
            p1.Offset(-desiredSize.Width / 2 - shift, -desiredSize.Height / 2);

            // todo ensure rotated label isn't past the arrow...
            // make label perpendicular to arrow it aligns with?
            //var rotationAngle = 0; // Specify the rotation angle in degrees
            //var rotateTransform = new RotateTransform(rotationAngle);
            //RenderTransform = rotateTransform;

            // TODO hover file => `git show a3cb` its content if its plaintext?

            // move it "edgLength" on the segment
            var angleBetweenPoints = GetAngleBetweenPoints(p1, p2);
            p1.Offset(labelLength * Math.Cos(angleBetweenPoints), -labelLength * Math.Sin(angleBetweenPoints));
            Arrange(new Rect(p1, desiredSize));
        }
    }
}