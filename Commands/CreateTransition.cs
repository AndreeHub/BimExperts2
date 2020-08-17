using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace BimExperts
{
    [Transaction(TransactionMode.Manual)]
    internal class CreateTransition : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Fetch the Document
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            //required arrays
            BuiltInCategory biDucts = BuiltInCategory.OST_DuctCurves;
            ElementId idDucts = doc.Settings.Categories.get_Item(biDucts).Id;

            ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();
            List<Element> eles = new List<Element>();
            List<Connector> cons = new List<Connector>();

            //check for element number
            if (selectedIds.Count > 2)
            {
                TaskDialog.Show("Wrong input", "You selected more then 2 elements");
                return Result.Succeeded;
            }
            //chekc element Category and if its ok
            foreach (var eleId in selectedIds)
            {
                Element ele = doc.GetElement(eleId);
                if (ele.Category.Id.Equals(idDucts))
                {
                    eles.Add(ele);
                }
                else
                {
                    TaskDialog.Show("Wrong input", "Wrong elements category");
                    return Result.Succeeded;
                }
            }

            Duct du1 = eles[0] as Duct;
            Duct du2 = eles[1] as Duct;
            ConnectorSet cc1 = du1.ConnectorManager.UnusedConnectors;
            ConnectorSet cc2 = du2.ConnectorManager.UnusedConnectors;

            foreach (Connector c in cc1)
            {
                cons.Add(c);
            }
            foreach (Connector c in cc2)
            {
                cons.Add(c);
            }
            //transaction for creating the transition
            using (Transaction trans = new Transaction(doc, "Trans1"))
            {
                trans.Start();

                doc.Create.NewTransitionFitting(cons[0], cons[1]);

                trans.Commit();
            }

            return Result.Succeeded;
        }
    }
}