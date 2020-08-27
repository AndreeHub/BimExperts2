using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BimExperts;
using BimExperts.Commands;
using BimExperts.Model;
using BimExperts.ViewModels.ViewCommands;
using BimExperts.Views;

namespace BimExperts.ViewModels
{
    class MeasureAndCountViewModel: ViewModelBase
    {
        #region Vars



        private ReleyCommand _uiCloseCommand;
        public ReleyCommand CloseCommand
        {
            get { return _uiCloseCommand; }
            set { SetProperty(ref _uiCloseCommand, value); }
        }


        public MeasureAndCount window;

        private UIDocument uidoc;
        private Document doc;

        public MeasureAndCountLogic logic;

        private ObservableCollection<string> _mesuredElementList;
        public ObservableCollection<string> MesuredElementList 
        {
            get { return _mesuredElementList; }
            set { _mesuredElementList = value; OnPropertyChanged("MesuredElementList"); }
        }

        #endregion
        public MeasureAndCountViewModel(MeasureAndCount win,UIDocument UIDOC)
        {
            CloseCommand = new ReleyCommand(closeCommand, canCloseCommand);
            window = win;
            uidoc = UIDOC;
            doc = uidoc.Document;
            logic = new MeasureAndCountLogic(uidoc,doc);
            logic.MeasureAndCount();

            MesuredElementList = new ObservableCollection<string>(logic.getInputList());
        }

        private void closeCommand(object obj)
        {
            window.Dispose();


        }
        private bool canCloseCommand(object obj)
        {
            return true;
        }

    }
}
