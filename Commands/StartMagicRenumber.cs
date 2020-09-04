using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System.Collections.Generic;
namespace BimExperts
{
    [Transaction(TransactionMode.Manual)]
    internal class StartMagicRenumber : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ExternalApp.thisApp.ShowWindowMagicrenumber(commandData.Application);

            return Result.Succeeded;
        }
    }
}
