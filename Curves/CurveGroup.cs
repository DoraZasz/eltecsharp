using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Curves
{
    using Curve = Tuple<UserPointController, CurveController>;
    class CurveGroup
    {
        List<Curve> curves;
        Curve activeCurve;
        Canvas canvas;
        int n = -1;

        public CurveGroup(Canvas canvas)
        {
            this.canvas = canvas;
            curves = new List<Curve>();
        }

        public void AddNewCurve(Point startPointPos)
        {
            n++;
            UserPointController userPointController = new UserPointController(0, canvas, n, UserPoint_MouseMove);
            CurveController curveController = new CurveController(userPointController, canvas, n);
            Curve newCurve = new Curve(userPointController, curveController);
            activeCurve = newCurve;
            curves.Add(newCurve);

            userPointController.AddPoint(startPointPos);
            userPointController.AddPoint(startPointPos);
            curveController.AddPath();
            curveController.DrawCurves();

        }

        public void AddNewPointToActive(Point point)
        {
            activeCurve.Item1.AddPoint(point);
            activeCurve.Item2.AddPath();
            activeCurve.Item2.DrawCurves();
        }

        public void UpdateActive()
        {
            activeCurve.Item2.DrawCurves();
        }

        public void ActiveDone()
        {
            //We dont want to edit the end-points on this curve in drawing mode anymore
            activeCurve.Item1.MakeNotMovable(0);
            activeCurve.Item1.MakeNotMovable(activeCurve.Item1.GetCount() - 1);
        }

        public void TurnModifyModeOn()
        {
            for (int i = 0; i < curves.Count(); i++)
            {
                for (int j = 0; j < curves.ElementAt(i).Item1.GetCount(); j++)
                {
                    curves.ElementAt(i).Item1.MakeMovable(j);
                    curves.ElementAt(i).Item1.AddCurveDrawingEvent(j);
                }
            }
        }

        public void TurnModifyModeOff()
        {
            for (int i = 0; i < curves.Count(); i++)
            {
                for (int j = 0; j < curves.ElementAt(i).Item1.GetCount(); j++)
                {
                    curves.ElementAt(i).Item1.MakeNotMovable(j);
                    curves.ElementAt(i).Item1.RemoveCurveDrawingEvent(j);
                }
            }
        }

        public void UserPoint_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Ellipse currentPoint = (sender as Ellipse);

                int canvasId = getCanvasId(currentPoint);
                curves.ElementAt(canvasId).Item2.DrawCurves();
            }
        }

        public void Reset()
        {
            curves.Clear();
            canvas.Children.Clear();
            n = -1;
        }

        int getCanvasId(Ellipse point)
        {
            string ids = point.Name.Substring("userPoint_".Count());
            return Convert.ToInt32(point.Name.Substring("userPoint_".Count()).Remove(ids.IndexOf('_')));
        }
    }
}
