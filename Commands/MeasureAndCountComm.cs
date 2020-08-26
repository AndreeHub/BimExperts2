using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace BimExperts.Commands
{
    [Transaction(TransactionMode.Manual)]
    class MeasureAndCountComm:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ExternalApp.thisApp.ShowMeasureAndCount(commandData.Application);
            return Result.Succeeded;
        }
    }
}
