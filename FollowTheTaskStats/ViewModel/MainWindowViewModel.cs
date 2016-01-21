using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FollowTheTaskStats.ServiceReference;
using FollowTheTaskStats.Model;

namespace FollowTheTaskStats.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Visibility _managersButtonVisibility, _workersButtonVisibility, _managersProgressBarVisibility, _workersProgressBarVisibility;

        #region constructors

        public MainWindowViewModel()
        {
            ManagersStats = new ObservableCollection<UserStats>();
            WorkersStats = new ObservableCollection<UserStats>();
            ManagersButtonVisibility = Visibility.Visible;
            ManagersProgressBarVisibility = Visibility.Collapsed;
            WorkersButtonVisibility = Visibility.Visible;
            WorkersProgressBarVisibility = Visibility.Collapsed;
        }

        #endregion

        #region properties

        public ObservableCollection<UserStats> ManagersStats { get; set; }
        public ObservableCollection<UserStats> WorkersStats { get; set; }

        public Visibility ManagersButtonVisibility
        {
            get { return _managersButtonVisibility; }
            set
            {
                _managersButtonVisibility = value;
                OnPropertyChanged("ManagersButtonVisibility");
            }
        }

        public Visibility WorkersButtonVisibility
        {
            get { return _workersButtonVisibility; }
            set
            {
                _workersButtonVisibility = value; 
                OnPropertyChanged("WorkersButtonVisibility");
            }
        }

        public Visibility ManagersProgressBarVisibility
        {
            get { return _managersProgressBarVisibility; }
            set
            {
                _managersProgressBarVisibility = value;
                OnPropertyChanged("ManagersProgressBarVisibility");
            }
        }

        public Visibility WorkersProgressBarVisibility
        {
            get { return _workersProgressBarVisibility; }
            set
            {
                _workersProgressBarVisibility = value;
                OnPropertyChanged("WorkersProgressBarVisibility");
            }
        }

        #endregion

        #region commands

        public ICommand LoadManagersStats
        {
            get { return new RelayCommand(LoadManagersStatsExecute); }
        }

        public ICommand LoadWorkersStats
        {
            get { return new RelayCommand(LoadWorkersStatsExecute); }
        }

        #endregion

        #region methods

        public async void LoadManagersStatsExecute()
        {
            ManagersButtonVisibility = Visibility.Collapsed;
            ManagersProgressBarVisibility = Visibility.Visible;
            FollowTheTaskServiceClient serviceClient = new FollowTheTaskServiceClient();
            ManagersStats.Clear();
            try
            {
                var managers = await serviceClient.GetManagerModelsAsync();
                foreach (var manager in managers)
                {
                    var managerStats = new UserStats
                    {
                        UserName = manager.User.UserName,
                        Role = "Менеджер",
                        Total = manager.TrackedTasks.Length,
                        TotalCompleted = manager.TrackedTasks.Count(t => t.IsFinished),
                        InTime = manager.TrackedTasks.Count(t => t.IsFinished && t.CompletionDate <= t.DeadLine)
                    };
                    var x = managerStats.TotalCompleted > 0 ? managerStats.InTime*1.0F/managerStats.TotalCompleted : 0.0F;
                    managerStats.BorderBrush = new SolidColorBrush(managerStats.TotalCompleted == 0
                        ? Colors.DarkGray
                        : Color.FromScRgb(1.0F, 1.0F - x, 0.0F + x, 0.0F));
                    ManagersStats.Add(managerStats);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            serviceClient.Close();
            ManagersButtonVisibility = Visibility.Visible;
            ManagersProgressBarVisibility = Visibility.Collapsed;
        }

        public async void LoadWorkersStatsExecute()
        {
            WorkersButtonVisibility = Visibility.Collapsed;
            WorkersProgressBarVisibility = Visibility.Visible;
            FollowTheTaskServiceClient serviceClient = new FollowTheTaskServiceClient();
            WorkersStats.Clear();
            try
            {
                var workers = await serviceClient.GetWorkerModelsAsync();
                foreach (var worker in workers)
                {
                    var workerStats = new UserStats
                    {
                        UserName = worker.User.UserName,
                        Role = "Исполнитель",
                        Total = worker.Quests.Length,
                        TotalCompleted = worker.Quests.Count(q => q.IsFinished),
                        InTime = worker.Quests.Count(q => q.IsFinished && q.CompletionDate <= q.DeadLine)
                    };
                    var x = workerStats.TotalCompleted > 0 ? workerStats.InTime * 1.0F / workerStats.TotalCompleted : 0.0F;
                    workerStats.BorderBrush = new SolidColorBrush(workerStats.TotalCompleted == 0
                        ? Colors.DarkGray
                        : Color.FromScRgb(1.0F, 1.0F - x, 0.0F + x, 0.0F));
                    WorkersStats.Add(workerStats);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            serviceClient.Close();
            WorkersButtonVisibility = Visibility.Visible;
            WorkersProgressBarVisibility = Visibility.Collapsed;
        }

        #endregion

    }
}
