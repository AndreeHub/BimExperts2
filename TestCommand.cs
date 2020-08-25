using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BimExperts.Views;

namespace BimExperts
{
    [Transaction(TransactionMode.Manual)]
    internal class TestCommand : IExternalCommand
    {
        //private MagicRenumber win = new MagicRenumber();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("Test Command Window", "Greeting from the other side");
            //win.Show();
            

            return Result.Succeeded;
        }
    }
}