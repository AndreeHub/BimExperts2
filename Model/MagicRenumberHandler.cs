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
using System.Windows;

namespace BimExperts.Model
{
    public class MagicRenumberHandler : IExternalEventHandler
    {
        private UIApplication UiApp = null;
        private Autodesk.Revit.ApplicationServices.Application App = null;
        private UIDocument UIDoc = null;
        private Document Doc = null;

        public void Execute(UIApplication app)
        {
            // at this point the application in inside revit again

            //UiApp = UiApp;
            //App = UiApp.Application;
            //UIDoc = UiApp.ActiveUIDocument;
            //Doc = UIDoc.Document;
            //Test task dialogue
            
            TaskDialog.Show("HARRO", "I WAS CALLED FROM OTUSIDE REVIT");
            MessageBox.Show("hello there", "Little ONe"); 

        }

        public string GetName()
        {
            return "External Event test";
        }
    }
}
