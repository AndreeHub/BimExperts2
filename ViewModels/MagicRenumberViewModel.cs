using Autodesk.Revit.UI;
using BimExperts.Model;
using BimExperts.ViewModels.ViewCommands;
using System.Collections.ObjectModel;
using System.Windows;
using static BimExperts.Model.MagicRenumberHandler;

namespace BimExperts.ViewModels
{
    //Implementing controls for the MacigRenumber Ui
    public class MagicRenumberViewModel
    {
        #region varDefs
        
        private MagicRenumberHandler handler;
        private ExternalEvent exEvent;
        private MagicRenumberLogic logic;

        public ReleyCommand uiLoadSystemElementsCommand { get; private set; }
        public ReleyCommand uiLoadSingleElementCommand { get; private set; }
        public ReleyCommand uiRunCommand { get; private set; }

        private ObservableCollection<string> _parameterNames = new ObservableCollection<string>();
        public ObservableCollection<string> ParameterNames
        {
            get { return _parameterNames; }
            set { _parameterNames = value;}
        }

        private string _selectedParamName;
        public string SelectedParamName
        {
            get { return _selectedParamName; }
            set { _selectedParamName = value; }
        }

        #endregion

        public MagicRenumberViewModel()
        {
            // hooking up binding to the commands themselves
            // reley is jsut a generic for passing arguments along
            //uiLoadSystemElementsCommand = new ReleyCommand(DisplayMessageBox, MessageBoxCanUse);
            //uiLoadSingleElementCommand  = new ReleyCommand(DisplayMessageBox, MessageBoxCanUse);
            //uiRunCommand                = new ReleyCommand(DisplayMessageBox, MessageBoxCanUse);
        }

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
            this.handler                = handler;
            this.exEvent                = exEvent;
            this.logic                  = logic;
        }

        #region buttonCommands

        private void runRenumbering(object obj)
        {
            handler.mode = selectionMode.Run;
            exEvent.Raise();
        }

        private void loadOrigin(object obj)
        {
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

        public void DisplayMessageBox(object message)
        {
            MessageBox.Show("HARRO", "trla baba lan");
            exEvent.Raise();
        }

        public bool MessageBoxCanUse(object message)
        {
            return true;
        }
    }
}