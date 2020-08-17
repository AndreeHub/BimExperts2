using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Windows;

namespace BimExperts.Model
{
    public class MagicRenumberHandler : IExternalEventHandler
    {
        private UIApplication UiApp = null;
        private Autodesk.Revit.ApplicationServices.Application App = null;
        private UIDocument UIDoc = null;
        private Document Doc = null;

        MagicRenumberLogic logicData = null;


        //enum for selection
        public enum selectionMode
        {
            One = 1,
            All = 2
        }
        selectionMode mode;


        public MagicRenumberHandler()
        {

        }

        public void Execute(UIApplication app)
        {
            // at this point the application in inside revit again

            UiApp = app;
            App = UiApp.Application;
            UIDoc = UiApp.ActiveUIDocument;
            Doc = UIDoc.Document;

            selectionControler(mode);
           


        }
        /// <summary>
        /// This function places the current selection in the appropriate variable in the logic class based on th eneum
        /// </summary>
        /// <param name="sel"></param>
        public void selectionControler(selectionMode sel)
        {
            if (sel == selectionMode.One)
                getCurrentSelection(UIDoc,logicData.renumberingOrigin);
            else if (sel == selectionMode.All)
                getCurrentSelection(UIDoc, logicData.elementsForRenumbering);
        }


        //gets the current selection
        public void getCurrentSelection(UIDocument uiDoc, ICollection<ElementId> idCollection)
        {
            idCollection = uiDoc.Selection.GetElementIds();
        }
     
        public string GetName()
        {
            return "External Event test";
        }
    }
}
