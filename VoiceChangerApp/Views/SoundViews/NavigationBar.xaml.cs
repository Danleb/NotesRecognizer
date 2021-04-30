using System;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace VoiceChangerApp.Views.SoundViews
{
    public partial class NavigationBar : UserControl
    {
        private readonly DispatcherTimer _dispatcherTimer = new();
        //private bool _isMouseLeftButtonPresed;
        private readonly Timer timer = new();
        private double _mouseScreenPositionMoveStart;
        private double progressBarX;

        public NavigationBar()
        {
            InitializeComponent();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1.0f / 60);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;

            timer.Interval = 1000f / 60f;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var currentScreenPos = System.Windows.Forms.Cursor.Position.X;
            var delta = _mouseScreenPositionMoveStart - currentScreenPos;
            var newPos = progressBarX + delta;

            Console.WriteLine($"Delta = {delta}");
            Console.WriteLine($"NewPos = {newPos}");

            var ratio = newPos / ProgressBar.ActualWidth;
            //var ratio = Mouse.GetPosition(ProgressBar).X / ProgressBar.ActualWidth;
            ProgressBar.Value = ratio;

            //Console.WriteLine("!!!");
            //var position = Mouse.GetPosition(ProgressBar);
            //var ratio = position.X / ProgressBar.ActualWidth;
            //ProgressBar.Value = ratio;
        }

        private void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseScreenPositionMoveStart = System.Windows.Forms.Cursor.Position.X;
            progressBarX = e.GetPosition(ProgressBar).X;
            //Console
            //_dispatcherTimer.Start();
            timer.Start();
            //_isMouseLeftButtonPresed = true;
        }

        private void ProgressBar_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void ProgressBar_MouseMove(object sender, MouseEventArgs e)
        {
            //if (Mouse.LeftButton == MouseButtonState.Released)
            //{
            //    return;
            //}

            //var ratio = e.GetPosition(ProgressBar).X / ProgressBar.ActualWidth;
            //ProgressBar.Value = ratio;
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                Console.WriteLine("RELEASED");
                _dispatcherTimer.Stop();
                return;
            }

            
        }
    }
}
