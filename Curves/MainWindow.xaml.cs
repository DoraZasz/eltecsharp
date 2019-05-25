using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Curves
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        CurveGroup curveGroup;
        Point startPos;
        double distanceX, distanceY;
        bool modifyMode = false;

        public MainWindow()
        {
            InitializeComponent();
            curveGroup = new CurveGroup(canvas);
        }

        private void BtnChangeMode_Click(object sender, RoutedEventArgs e)
        {
            modifyMode = !modifyMode;

            if (modifyMode)
            {
                BtnChangeMode.Content = "Turn change mode off";
                curveGroup.TurnModifyModeOn();

            }
            if (!modifyMode)
            {
                BtnChangeMode.Content = "turn change mode on";
                curveGroup.TurnModifyModeOff();
            }
        }

        private void BtnHide_Click(object sender, RoutedEventArgs e)
        {
            UIElementCollection siblings = (canvas as Panel).Children;
            var points = siblings.OfType<Ellipse>();

            foreach (Ellipse point in points)
            {
                point.Visibility = point.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        double distance(double a, double b)
        {
            return System.Math.Abs(a - b);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!modifyMode)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    Point mousePos = e.GetPosition(canvas);
                    curveGroup.UpdateActive();

                    double prevDistX, prevDistY;
                    prevDistX = distanceX;
                    prevDistY = distanceY;
                    distanceX = distance(mousePos.X, startPos.X);
                    distanceY = distance(mousePos.Y, startPos.Y);

                    if ((prevDistX > distanceX || prevDistY > distanceY)
                        && distanceX != 0 && distanceY != 0) //Prevent to get two points at the same place
                    {
                        curveGroup.AddNewPointToActive(mousePos);

                        TbCoords.Text += "new point \n";
                        TbCoords.Text += mousePos.ToString() + '\n';
                        startPos = mousePos;
                    }
                }
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TbCoords.Text += "mouseDown \n";
            if (!modifyMode)
            {
                curveGroup.AddNewCurve(e.GetPosition(canvas));
                startPos = e.GetPosition(canvas);
                distanceX = distanceY = 0;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            curveGroup.Reset();
            BtnChangeMode.Content = "turn change mode on";
            modifyMode = false;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TbCoords.Text += "mouseUp \n";
            if (!modifyMode)
            {
                curveGroup.ActiveDone();
            }
        }
    }
}
