using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using DevExpress.Mvvm;

namespace CaptchaSolverServer.ViewModels
{
    class AboutPageViewModel : BindableBase
    {
        public ICommand OpenUrl => new DelegateCommand<string>((url) =>
        {
            try
            {
                Process.Start("cmd", "/C start" + " " + url);
            }
            catch { }
            
        });
    }
}
