using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CaptchaSolverServer.Models.CaptchaSolverServers
{
    class StatisticService : INotifyPropertyChanged
    {
        #region Lockers
        readonly object _lockerGoods = new object();
        readonly object _lockerErrors = new object();
        readonly object _lockerCreateTaskRequests = new object();
        #endregion

        #region Private Fields

        private readonly Statistic _statistic = new Statistic();

        #endregion

        #region Properties

        public int CreateTaskRequests
        {
            get => _statistic.CreateTaskRequests;
            private set
            {
                _statistic.CreateTaskRequests = value;
                OnPropertyChanged();
            }
        }

        public int ImageRecognizeGoods
        {
            get => _statistic.ImageRecognizeGoods;
            private set
            {
                _statistic.ImageRecognizeGoods = value;
                OnPropertyChanged();
            }
        }

        public int ImageRecognizeErrors
        {
            get => _statistic.ImageRecognizeErrors;
            private set
            {
                _statistic.ImageRecognizeErrors = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Public Methods

        public void AddCreateTaskRequest()
        {
            lock (_lockerCreateTaskRequests)
            {
                CreateTaskRequests++;
            }
        }

        public void AddImageRecognizeGood()
        {
            lock (_lockerGoods)
            {
                ImageRecognizeGoods++;
            }
        }

        public void AddImageRecognizeErrors()
        {
            lock (_lockerErrors)
            {
                ImageRecognizeErrors++;
            }
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
