using Practica_SchimbValutar.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica_SchimbValutar.MVVM.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand SelectViewModel { get; set; }
        public RelayCommand InsertViewModel { get; set; }
        public RelayCommand UpdateViewModel { get; set; }
        public RelayCommand DeleteViewModel { get; set; }
        public RelayCommand MainViewModels { get; set; }
        public RelayCommand DeleteClientViewModel { get; set; }
        public RelayCommand InsertClientViewModel { get; set; }
        public RelayCommand UpdateClientViewModel { get; set; }

        public SelectViewModel SelectVM{ get; set; }
        public InsertViewModel InsertVM { get; set; }
        public UpdateViewModel UpdateVM { get; set; }
        public DeleteViewModel DeleteVM { get; set; }
        public InsertClientViewModel InsertClientVM { get; set; }
        public UpdateClientViewModel UpdateClientVM { get; set; }
        public DeleteClientViewModel DeleteClientVM { get; set; }


        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            SelectVM = new SelectViewModel();
            InsertVM = new InsertViewModel();
            UpdateVM = new UpdateViewModel();
            DeleteVM = new DeleteViewModel();
            InsertClientVM = new InsertClientViewModel();
            UpdateClientVM = new UpdateClientViewModel();
            DeleteClientVM = new DeleteClientViewModel();

            MainViewModels = new RelayCommand(o =>
            {
                CurrentView = null;
            });

            SelectViewModel = new RelayCommand(o => {
                CurrentView = SelectVM;
            });

            InsertViewModel = new RelayCommand(o => {
                CurrentView = InsertVM;
            });

            UpdateViewModel = new RelayCommand(o => {
                CurrentView = UpdateVM;
            });

            DeleteViewModel = new RelayCommand(o => {
                CurrentView = DeleteVM;
            });

            InsertClientViewModel = new RelayCommand(o => {
                CurrentView = InsertClientVM;
            });

            UpdateClientViewModel = new RelayCommand(o => {
                CurrentView = UpdateClientVM;
            });

            DeleteClientViewModel = new RelayCommand(o => {
                CurrentView = DeleteClientVM;
            });
        }
    }
}
