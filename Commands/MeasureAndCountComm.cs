using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using BimExperts.Model;

namespace BimExperts.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class MeasureAndCountComm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        { 
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ExternalApp.thisApp.ShowMeasureAndCount(commandData.Application,uidoc);

            return Result.Succeeded;
        }
    }
}
