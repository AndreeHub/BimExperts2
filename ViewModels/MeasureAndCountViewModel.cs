using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BimExperts;
using BimExperts.Commands;
using BimExperts.ViewModels.ViewCommands;
using BimExperts.Views;

namespace BimExperts.ViewModels
{
    class MeasureAndCountViewModel: ViewModelBase
    {
        #region Vars

        public ReleyCommand uiCloseCommand { get; private set; }
        public MeasureAndCount window;

        private ObservableCollection<string> _mesuredElementList;
        public ObservableCollection<string> MesuredElementList 
        {
            get { return _mesuredElementList; }
            set { SetProperty(ref _mesuredElementList, value); OnPropertyChanged("MesuredElementList"); }
        }
        #endregion
        public MeasureAndCountViewModel(MeasureAndCount win)
        {
            uiCloseCommand = new ReleyCommand(closeCommand, canCloseCommand);
            window = win;
        }

        private void closeCommand(object obj)
        {
            window.Close();
        }
        private bool canCloseCommand(object obj)
        {
            return true;
        }


    }
}
