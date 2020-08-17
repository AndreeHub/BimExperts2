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

        public MagicRenumberLogic logicData = null;

        //enum for selection
        public enum selectionMode
        {
            One = 1,
            All = 2,
            Run = 3
        }
        public selectionMode mode;


        public void Execute(UIApplication app)
        {
            // at this point the application in inside revit again

            logicData.setRevitApp(app);
            logicData.selectionControler(mode,app);


        }
        /// <summary>
        /// This function places the current selection in the appropriate variable in the logic class based on th eneum
        /// </summary>
        /// <param name="sel"></param>
    

       
     
        public string GetName()
        {
            return "External Event test";
        }
    }
}
