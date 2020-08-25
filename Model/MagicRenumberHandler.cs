using Autodesk.Revit.UI;

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
            logicData.selectionControler(mode, app);
        }

        public string GetName()
        {
            return "External Event test";
        }
    }
}