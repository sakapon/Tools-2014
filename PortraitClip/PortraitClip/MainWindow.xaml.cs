using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PortraitClip
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += (o, e) =>
            {
                ((IDisposable)KinectContext.Current).Dispose();
            };

            var isMouseDown = false;
            var point = default(Point);
            MouseLeftButtonDown += (o, e) =>
            {
                isMouseDown = true;
                point = ScreenManager.GetCursorPosition();
            };
            MouseMove += (o, e) =>
            {
                if (!isMouseDown) return;

                var point_old = point;
                point = ScreenManager.GetCursorPosition();

                var v = point - point_old;
                Left += v.X;
                Top += v.Y;
            };
            MouseLeftButtonUp += (o, e) =>
            {
                isMouseDown = false;
                point = default(Point);
            };
        }
    }
}
