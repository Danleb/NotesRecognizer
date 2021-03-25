using Microsoft.Extensions.Logging;
using Prism.Ioc;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VoiceChangerApp.Utils;
using Point = System.Windows.Point;

namespace VoiceChangerApp.Views.SoundViews
{
    public class PlotNavigator : IEnable
    {
        private readonly ILogger _logger;
        private readonly DispatcherTimer _keyTimer;
        private Point _dragStartMousePosition;
        private float _dragStartViewportCenterPosition;

        public PlotNavigator(Control targetInputControl, OrthographicViewportMatrix viewport, BoundSquare boundSquare, IRenderable renderable)
        {
            _logger = (ILogger)ContainerLocator.Container.Resolve(typeof(ILogger<SignalgramView>));
            _keyTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.0f / 30)
            };
            _keyTimer.Tick += KeyboardTimerTick;
            _keyTimer.Start();

            Control = targetInputControl;
            Viewport = viewport;
            BoundSquare = boundSquare;
            Renderable = renderable;
            Control.MouseDown += Control_MouseDown;
            Control.MouseMove += Control_MouseMove;
            Control.MouseWheel += Control_MouseWheel;
        }

        public bool IsEnabled { get; set; } = true;
        public Control Control { get; }
        public OrthographicViewportMatrix Viewport { get; set; }
        public BoundSquare BoundSquare { get; set; }
        public IRenderable Renderable { get; }
        public float MouseWheelScaleSpeed { get; set; } = 1.0f / 800.0f;
        public float KeyboardNavigationSpeed { get; set; } = 1.0f / 800.0f;
        public bool ScaleAxesSeparately { get; set; } = true;

        private float XUnitsPerPixel => Viewport.ScaledWidth / (float)Control.ActualWidth;

        private void Control_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var diff = -e.Delta * MouseWheelScaleSpeed;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                //Viewport.ScaleY += diff;
                Viewport.DoScaleY(1.0f + diff);
            }
            else
            {
                //Viewport.ScaleX += diff;
                Viewport.DoScaleX(1.0f + diff);
            }

            ClampViewport();
            Viewport.UpdateMatrix();
            Renderable.RequestRedraw();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                return;
            }

            var delta = _dragStartMousePosition - e.GetPosition(Control);
            var mouseDeltaX = (float)delta.X;
            var unitsDeltaX = mouseDeltaX * XUnitsPerPixel;
            _logger.LogDebug($"PlotNavigator MouseMove DeltaX={mouseDeltaX} ControlWidth={Control.ActualWidth} XUnitsPerPixel={XUnitsPerPixel} XUnitsDelta={unitsDeltaX}");
            Viewport.CenterX = _dragStartViewportCenterPosition + unitsDeltaX;

            ClampViewport();
            Viewport.UpdateMatrix();
            Renderable.RequestRedraw();
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartMousePosition = e.GetPosition(Control);
            _dragStartViewportCenterPosition = Viewport.CenterX;
        }

        private void KeyboardTimerTick(object sender, EventArgs e)
        {
            Vector direction = new(0, 0);
            bool pressed = false;
            if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
            {
                pressed = true;
                direction.Y = -1;
            }
            if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up))
            {
                pressed = true;
                direction.Y = 1;
            }
            if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
            {
                pressed = true;
                direction.X = -1;
            }
            if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
            {
                pressed = true;
                direction.X = 1;
            }

            if (!pressed)
            {
                return;
            }

            var timeDelta = _keyTimer.Interval.TotalMilliseconds;
            direction.Normalize();
            var offset = direction * timeDelta * KeyboardNavigationSpeed;
            Viewport.CenterX += (float)offset.X;

            ClampViewport();
            Viewport.UpdateMatrix();
            Renderable.RequestRedraw();
        }

        private void ClampViewport()
        {
            Viewport.Left = MathF.Max(BoundSquare.Left, Viewport.Left);
            Viewport.Right = MathF.Min(BoundSquare.Right, Viewport.Right);
            Viewport.Bottom = MathF.Max(BoundSquare.Bottom, Viewport.Bottom);
            Viewport.Top = MathF.Min(BoundSquare.Top, Viewport.Top);
        }
    }
}
