using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;

namespace CaptchaSolverServer.Behaviors
{
    public class DragMoveBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Application.Current.MainWindow.DragMove();
            }
        }
    }

}
