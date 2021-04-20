using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System.Collections.Generic;


namespace BimExperts
{
    [Transaction(TransactionMode.Manual)]
    class TimeStamps : IExternalCommand
    {
        /// <summary>
        /// this fires when the button is pressed
        /// </summary>
        /// <param name="commandData">Access data through here</param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ExternalApp.thisApp.RunCommand(commandData);

            return Result.Succeeded;
        }
    }

}


