using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Curves
{
    class CurveController
    {
        Canvas canvas;
        UserPointController userPoints;
        ComputeCurve computeCurve;
        string[] curves;
        int id;

        public CurveController(UserPointController userPoints, Canvas canvas, int id)
        {
            this.canvas = canvas;
            this.userPoints = userPoints;
            this.id = id;
            computeCurve = new ComputeCurve(userPoints, canvas, id);
            if (userPoints.GetCount() >= 2)
            {
                curves = computeCurve.UpdateSplines();
                addCurves();
            }
        }

        public void DrawCurves()
        {
            if (userPoints.GetCount() >= 2)
            {
                var paths = getCurvePaths();
                curves = computeCurve.UpdateSplines();
                int i = 0;
                foreach (Path path in paths)
                {
                    path.Data = Geometry.Parse(curves[i]);
                    i++;
                }
            }
        }

        public Path AddPath()
        {
            List<Path> curvePaths = getCurvePaths();     

            Path path = new Path();
            path.Name = "curve_" + id + "_" + curvePaths.Count();
            path.Stroke = new SolidColorBrush(Colors.Black);
            path.StrokeThickness = 1;
            canvas.Children.Add(path);

            return path;
        }

        void addCurves()
        {
            for (int i = 0; i < curves.Length; i++)
            {
                Path newPath = AddPath();
                newPath.Data = Geometry.Parse(curves[i]);
                canvas.Children.Add(newPath);
            }
        }

        //Get Path elements connected to this curve
        List<Path> getCurvePaths()
        {
            UIElementCollection siblings = (canvas as Panel).Children;
            var allPaths = siblings.OfType<Path>().ToList();
            var curvePaths = allPaths.FindAll(x => x.Name.StartsWith("curve_" + id));
            return curvePaths;
        }




    }
}
