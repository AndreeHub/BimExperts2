using Autodesk.Revit.UI;
using BimExperts.Model;
using BimExperts.ViewModels.ViewCommands;
using System.Collections.ObjectModel;
using System.Windows;
using static BimExperts.Model.MagicRenumberHandler;

namespace BimExperts.ViewModels
{
    //Implementing controls for the MacigRenumber Ui
    public class MagicRenumberViewModel : ViewModelBase
    {
        #region varDefs

        private MagicRenumberHandler handler;
        private ExternalEvent exEvent;
        private MagicRenumberLogic logic;

        //command for button triggers
        public ReleyCommand uiLoadSystemElementsCommand { get; private set; }
        public ReleyCommand uiLoadSingleElementCommand { get; private set; }
        public ReleyCommand uiRunCommand { get; private set; }

        //for the comboBox selection
        private ObservableCollection<string> _parameterNames;

        public ObservableCollection<string> ParameterNames
        {
            get { return _parameterNames; }
            set { _parameterNames = value; OnPropertyChanged("ParameterNames"); }
        }

        private string _selectedParamName;

        public string SelectedParamName
        {
            get { return _selectedParamName; }
            set { _selectedParamName = value; }
        }

        private string _startingStringEntryBase;

        public string StartingStringEntryBase
        {
            get { return _startingStringEntryBase; }
            set { SetProperty(ref _startingStringEntryBase, value); }
        }

        private string _startingStringEntryPrefix;

        public string StartingStringEntryPrefix
        {
            get { return _startingStringEntryPrefix; }
            set { SetProperty(ref _startingStringEntryPrefix, value); }
        }

        private string _startingStringEntrySuffix;

        public string StartingStringEntrySuffix
        {
            get { return _startingStringEntrySuffix; }
            set { SetProperty(ref _startingStringEntrySuffix, value); }
        }

        #endregion varDefs

        #region Constructor
        /// <summary>
        /// constructor for event
        /// </summary>
        /// <param name                 = "exEvent"></param>
        /// <param name                 = "handler"></param>
        public MagicRenumberViewModel(ExternalEvent exEvent, MagicRenumberHandler handler, MagicRenumberLogic logic)
        {
            //hooking up commands
            uiLoadSystemElementsCommand = new ReleyCommand(loadSystemElements, loadSystemCanUse);
            uiLoadSingleElementCommand  = new ReleyCommand(loadOrigin, loadOriginCanUse);
            uiRunCommand                = new ReleyCommand(runRenumbering, renumberingCanUe);

            //konsturktor za handler
            this.handler = handler;
            this.exEvent = exEvent;
            this.logic   = logic;
        }

        #endregion

        #region buttonCommands

        //get the entry from the textbox
        private void runRenumbering(object obj)
        {
            handler.mode = selectionMode.Run;
            logic.setStartingParameterName(_selectedParamName);
            logic.setStartinParameterNumber(_startingStringEntryBase);
            exEvent.Raise();
        }

        private void loadOrigin(object obj)
        {
            ParameterNames = new ObservableCollection<string>(logic.returnParamList());
            handler.mode = selectionMode.One;
            exEvent.Raise();
        }

        private void loadSystemElements(object obj)
        {
            handler.mode = selectionMode.All;
            handler.logicData = logic;
            exEvent.Raise();
        }

        //required bools for commands
        private bool renumberingCanUe(object obj)
        {
            return true;
        }

        private bool loadOriginCanUse(object obj)
        {
            return true;
        }

        private bool loadSystemCanUse(object obj)
        {
            return true;
        }

        #endregion buttonCommands

        #region TestingCode
        public void DisplayMessageBox(object message)
        {
            MessageBox.Show("HARRO", "trla baba lan");
            exEvent.Raise();
        }

        public bool MessageBoxCanUse(object message)
        {
            return true;
        }

        #endregion
    }
}