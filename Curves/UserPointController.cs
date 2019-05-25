using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Curves
{
    class UserPointController  
    {
        public const double Radius = 5;
        List<Ellipse> points;
        Canvas canvas;
        int curveId;

        public delegate void NewEvent(object sender, MouseEventArgs e);
        NewEvent curveDrawing;

        TextBox tbTest;


        public UserPointController(int count, Canvas canvas, NewEvent curveDrawing, TextBox tbTest, int curveId)
        {
            this.tbTest = tbTest;

            this.curveId = curveId;
            this.canvas = canvas;
            this.curveDrawing = curveDrawing;

            points = new List<Ellipse>();
            for (int i = 0; i < count; i++)
            {
                double left = 10 + i * (Radius + Radius*2);
                double top = Radius * 2 + Radius*2;
                AddPoint(new Point(left, top));
            }
        }

        public UserPointController(int count, Canvas canvas, int curveId, NewEvent curveDrawing)
        {
            //this.tbTest = tbTest;

            this.curveId = curveId;
            this.canvas = canvas;
            this.curveDrawing = curveDrawing;

            points = new List<Ellipse>();
            for (int i = 0; i < count; i++)
            {
                double left = 10 + i * (Radius + Radius * 2);
                double top = Radius * 2 + Radius * 2;
                AddPoint(new Point(left, top));
            }
        }
        public void AddPoint(Point point)
        {
            int i = points.Count;
            points.Add(new Ellipse());
            Ellipse current = points.ElementAt(i);
            current.Name = "userPoint_" + curveId + "_" + i;
            current.Height = current.Width = Radius * 2;
            current.Fill = new SolidColorBrush(Colors.Blue);
            SetPosition(current, point.X, point.Y);
            Panel.SetZIndex(current, i);

            current.MouseMove += UserPoint_MouseMove;
            //current.MouseMove += new MouseEventHandler(curveDrawing);
            addToMainWindow(current);


            //During drawing we only want to move the last point
            if ( points.Count() > 2)
                points.ElementAt(points.Count() - 2).MouseMove -= UserPoint_MouseMove; 
        }

        public void SetPosition(Ellipse point, double left, double top)
        {
            Canvas.SetLeft(point, left - Radius);
            Canvas.SetTop(point, top - Radius);
        }

        public int GetCount()
        {
            return points.Count;
        }

        void addToMainWindow(Ellipse point)
        {
            canvas.Children.Add(point);
        }

        public void UserPoint_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Ellipse currentPoint = (sender as Ellipse);
                SetPosition(currentPoint, e.GetPosition(canvas).X, e.GetPosition(canvas).Y);
            }
        }

        public void MakeMovable(int index)
        {
            points.ElementAt(index).MouseMove += UserPoint_MouseMove;
        }

        public void AddCurveDrawingEvent(int index)
        {
            points.ElementAt(index).MouseMove += new MouseEventHandler(curveDrawing);
        }

        public void MakeNotMovable(int index)
        {
            points.ElementAt(index).MouseMove -= UserPoint_MouseMove;
        }

        public void RemoveCurveDrawingEvent(int index)
        {
            points.ElementAt(index).MouseMove -= new MouseEventHandler(curveDrawing);
        }
    }
}
