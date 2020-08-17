
using Autodesk.Revit.UI;
using BimExperts.Model;
using BimExperts.ViewModels.ViewCommands;
using System.Windows;

namespace BimExperts.ViewModels
{
    //Implementing controls for the MacigRenumber Ui
    public class MagicRenumberViewModel
    { 
        private MagicRenumberHandler handler;
        private ExternalEvent exEvent;

        public ReleyCommand uiLoadSystemElementsCommand { get; private set; }
        public ReleyCommand uiLoadSingleElementCommand { get; private set; }
        public ReleyCommand uiRunCommand { get; private set; }


        public MagicRenumberViewModel()
        {
            // hooking up binding to the commands themselves
            // reley is jsut a generic for passing arguments along
            uiLoadSystemElementsCommand = new ReleyCommand(DisplayMessageBox, MessageBoxCanUse);
            uiLoadSingleElementCommand = new ReleyCommand(DisplayMessageBox, MessageBoxCanUse);
            uiRunCommand = new ReleyCommand(DisplayMessageBox, MessageBoxCanUse);

        }
        /// <summary>
        /// constructor for event
        /// </summary>
        /// <param name="exEvent"></param>
        /// <param name="handler"></param>
        public MagicRenumberViewModel(ExternalEvent exEvent, MagicRenumberHandler handler)
        {

            //hooking up commands
            uiLoadSystemElementsCommand = new ReleyCommand(DisplayMessageBox, MessageBoxCanUse);
            uiLoadSingleElementCommand  = new ReleyCommand(DisplayMessageBox, MessageBoxCanUse);
            uiRunCommand                = new ReleyCommand(DisplayMessageBox, MessageBoxCanUse);

            //konsturktor za handler
            this.handler = handler;
            this.exEvent = exEvent;

            
        }
        
        public void DisplayMessageBox(object message)
        {
            MessageBox.Show("HARRO","trla baba lan");
            exEvent.Raise();
        }


        public bool MessageBoxCanUse(object message)
        {
            return true;
        }

      
    }
}
