using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Curves
{
    //JS source: https://www.particleincell.com/2012/bezier-splines/
    class ComputeCurve
    {
        UserPointController userPoints;
        Canvas canvas;
        int curveId;
        int n;

        public ComputeCurve(UserPointController userPoints, Canvas canvas, int curveId)
        {
            this.userPoints = userPoints;
            n = userPoints.GetCount();
            this.canvas = canvas;
            this.curveId = curveId;
        }

        /*computes spline control points*/
        public string[] UpdateSplines()
        {
            n = userPoints.GetCount();

            /*grab (x,y) coordinates of the control points*/
            double[] x = new double[n];
            double[] y = new double[n];

            UIElementCollection childs = (canvas as Panel).Children;
            var allEllipses = childs.OfType<Ellipse>().ToList();
            var ellipses = allEllipses.FindAll(lambda => lambda.Name.StartsWith("userPoint_" + curveId));

            for (int i = 0; i < n; i++)
            {
                x[i] = Canvas.GetLeft(ellipses.ElementAt(i));
                y[i] = Canvas.GetTop(ellipses.ElementAt(i));
            }

            /*computes control points p1 and p2 for x and y direction*/
            pair px = computeControlPoints(x);
            pair py = computeControlPoints(y);

            string[] res = new string[n-1];
            for (int i = 0; i < n-1; i++)
            {
                res[i] = path(x[i], y[i], px.p1[i], py.p1[i], px.p2[i], py.p2[i], x[i + 1], y[i + 1]).Replace(',', '.');
            }
            return res;
        }

        /*creates formated path string for SVG cubic path element*/
        string path(double x1, double y1, double px1, double py1, double px2, double py2, double x2, double y2)
        {
            return "M " + radius(x1) + " " + radius(y1)  + " C " + radius(px1) + " " + radius(py1) + " " + radius(px2) + " " + radius(py2) + " " + radius(x2) + " " + radius(y2);
        }

        double radius(double x)
        {
            return x + UserPointController.Radius;
        }

        /*computes control points given knots K, this is the brain of the operation*/
        pair computeControlPoints(double[] K)
        {
            int n = K.Count() - 1;
            double[] p1 = new double[n]; 
            double[] p2 = new double[n]; 

            /*rhs vector*/
            double[] a = new double[n]; 
            double[] b = new double[n]; 
            double[] c = new double[n]; 
            double[] r = new double[n];

            /*left most segment*/
            a[0] = 0;
            b[0] = 2;
            c[0] = 1;
            r[0] = K[0] + 2 * K[1];

            /*internal segments*/
            for (int i = 1; i < n - 1; i++)
            {
                a[i] = 1;
                b[i] = 4;
                c[i] = 1;
                r[i] = 4 * K[i] + 2 * K[i + 1];
            }

            /*right segment*/
            a[n - 1] = 2;
            b[n - 1] = 7;
            c[n - 1] = 0;
            r[n - 1] = 8 * K[n - 1] + K[n];

            /*solves Ax=b with the Thomas algorithm (from Wikipedia)*/
            for (int i = 1; i < n; i++)
            {
                var m = a[i] / b[i - 1];
                b[i] = b[i] - m * c[i - 1];
                r[i] = r[i] - m * r[i - 1];
            }

            p1[n - 1] = r[n - 1] / b[n - 1];
            for (int i = n - 2; i >= 0; --i)
                p1[i] = (r[i] - c[i] * p1[i + 1]) / b[i];

            /*we have p1, now compute p2*/
            for (int i = 0; i < n - 1; i++)
                p2[i] = 2 * K[i + 1] - p1[i + 1];

            p2[n - 1] = 0.5 * (K[n] + p1[n - 1]);

            pair p;
            p.p1 = p1;
            p.p2 = p2;
            return p;
        }
        struct pair
        {
            public double[] p1;
            public double[] p2;
        }

    }
}
