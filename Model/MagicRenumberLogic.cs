using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using static BimExperts.Model.MagicRenumberHandler;

namespace BimExperts.Model
{
    public class MagicRenumberLogic
    {
        //revit intilizers
        private UIApplication UiApp = null;
        private Autodesk.Revit.ApplicationServices.Application App = null;
        private UIDocument UiDoc = null;
        private Document Doc = null;


        //data storage
        public ICollection<ElementId> elementsForRenumbering = null;
        public ICollection<ElementId> renumberingOrigin = null;

        public ICollection<Element> elementsForRe = null
        public List<string> parameterNames = null;

        //Set Revit top level data
        public void setRevitApp(UIApplication app)
        {
            UiApp = app;
            App = UiApp.Application;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
        }

        //Chose w
        public void selectionControler(selectionMode sel, UIApplication app)
        {
            if (sel == selectionMode.One)
                getCurrentSelection(UiDoc,renumberingOrigin);
            else if (sel == selectionMode.All)
            {
                getCurrentSelection(UiDoc,elementsForRenumbering);
                foreach (var item in elementsForRenumbering)
                {
                    elementsForRe.Add(Doc.GetElement(item));
                }

                getParameters();
            }
            else if (sel == selectionMode.Run)
                Run();
        }

        internal void Run(UIApplication app)
        {
        }

        private void getParameters()
        {
            foreach (var item in elementsForRenumbering)
            {
                elementsForRe.Add(Doc.GetElement(item));
            }

        }

        //gets the current selection
        public void getCurrentSelection(UIDocument uiDoc, ICollection<ElementId> idCollection)
        {
            idCollection = uiDoc.Selection.GetElementIds();
        }
    }
}