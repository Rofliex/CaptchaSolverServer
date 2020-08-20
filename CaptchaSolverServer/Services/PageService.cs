using System;
using System.Windows.Controls;

namespace CaptchaSolverServer.Services
{
    class PageService
    {
        public event Action<Page> OnPageChanged;

        public void Navigate(Page page)
        {
            OnPageChanged?.Invoke(page);
        }
    }
}