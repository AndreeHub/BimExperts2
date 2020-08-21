using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
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
        public ICollection<ElementId> unorderedElementIds = null;
        public ICollection<ElementId> renumberingOrigin = null;

        public ICollection<Element> elementsForRenumbering = null;

        public HashSet<string> commonParameters = null;

        //Set Revit top level data
        public void setRevitApp(UIApplication app)
        {
            UiApp = app;
            App = UiApp.Application;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
        }

        //get and assign selection from Revit based on Mode
        public void selectionControler(selectionMode sel, UIApplication app)
        {
            if (sel == selectionMode.One)
                getCurrentSelection(renumberingOrigin);
            else if (sel == selectionMode.All)
            {
                getCurrentSelection(unorderedElementIds);
                foreach (var item in unorderedElementIds)
                {
                    elementsForRenumbering.Add(Doc.GetElement(item));
                }

                getCommonParameters();
            }
            else if (sel == selectionMode.Run)
                Run();
        }


        //gets the current selection
        private void getCurrentSelection(ICollection<ElementId> idCollection)
        {
            idCollection = UiDoc.Selection.GetElementIds();
        }
        //run the renumbering logic
        private void Run()
        {
            throw new NotImplementedException();
        }

        internal void Run(UIApplication app)
        {
        }

        //find all the common parameters in the elements set, used for combobox form
        private void getCommonParameters()
        {
            getElementsFromIds();
            populateListWithParameterNames();

        }

        private void populateListWithParameterNames()
        {
            foreach (var ele in elementsForRenumbering)
            {
                foreach (var para in ele.ParametersMap)
                {
                    //TODO add a check to see if the current parameters are contained\
                    Parameter Param = (para as Parameter);
                    if (Param.StorageType == StorageType.String)
                    {
                        var paraName = Param.Definition.Name;
                        commonParameters.Add(paraName);

                    }
                }
            }
        }

        private void getElementsFromIds()
        {
            foreach (var eleId in unorderedElementIds)
            {
                elementsForRenumbering.Add(Doc.GetElement(eleId));
            }
        }

    }
}