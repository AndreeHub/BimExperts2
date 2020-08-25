using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static BimExperts.Model.MagicRenumberHandler;

namespace BimExperts.Model
{
    public class MagicRenumberLogic
    {
        #region Fields

        //revit intilizers
        private UIApplication UiApp = null;

        private Autodesk.Revit.ApplicationServices.Application App = null;
        private UIDocument UiDoc = null;
        private Document Doc = null;

        //input vars
        public ICollection<ElementId> unorderedElementIds = null;

        public ICollection<ElementId> renumberingOrigin = null;
        private string selectedPara;
        private string startNumber;

        //work vars
        private Element Origin = null;

        public List<Element> elementsForRenumbering = new List<Element>();
        public List<int> elementsForRenumberingCheckList = new List<int>();
        private List<Element> orderedElements = new List<Element>();
        public HashSet<string> commonParameters = new HashSet<string>();

        #endregion Fields

        #region Constructor and Controler

        //Set Revit top level data needed for api acess
        public void setRevitApp(UIApplication app)
        {
            UiApp = app;
            App = UiApp.Application;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
        }

        //decide which function to run inside revit ( this is called only in the execute method of the handler)
        public void selectionControler(selectionMode sel, UIApplication app)
        {
            if (sel == selectionMode.One)
                renumberingOrigin = getCurrentSelection();
            else if (sel == selectionMode.All)
            {
                unorderedElementIds = getCurrentSelection();
                getCommonParameters();
            }
            else if (sel == selectionMode.Run)
            {
                Run();
            }
        }

        #endregion Constructor and Controler

        #region renumberingLogic

        //run the renumbering logic
        private void Run()
        {
            //get first element from ID
            getOriginElement(renumberingOrigin);
            //pop his ass from the list for easier orientation
            elementsForRenumberingCheckList.Remove(Origin.Id.IntegerValue);
            //get the connector pointing to the selection
            ConnectorSet originConnectors = GetConnectors(Origin);
            traverseSystem(Origin);
            setParamters();
        }

        #endregion renumberingLogic

        #region RunHelpers

        private void traverseSystem(Element ele)
        {
            if (isFinalElement())
            {
                orderedElements.Add(ele);
                return;
            }
            elementsForRenumberingCheckList.Remove(ele.Id.IntegerValue); // remove from check list
            orderedElements.Add(ele);
            List<Element> nextEles = null;
            //get the connecting elements
            nextEles = getConnectingElements(ele);
            //check if the element is in the list, if yes that means hes next one to be visited
            foreach (Element el in nextEles)
            {
                if (isElementInSelection(el))
                {
                    traverseSystem(el);
                }
            }

        }


        private void setParamters()
        {
            //1.get the starting number from input string
            int num = int.Parse(startNumber);
            string val;
            Parameter pa;
            using (Transaction trans = new Transaction(Doc, "Set params"))
            {
                trans.Start();

                foreach (Element ele in orderedElements)
                {
                    val = num.ToString();
                    pa = ele.LookupParameter(selectedPara);
                    pa.Set(val);
                    num++;
                }

                trans.Commit();

            }
                
        }

        private bool isFinalElement()
        {
            if (elementsForRenumberingCheckList.Count == 1) return true; return false;
        }

        private bool isElementInSelection(Element el)
        {
            if (elementsForRenumberingCheckList.Contains(el.Id.IntegerValue)) return true; return false;
        }

        private List<Element> getConnectingElements(Element ele)
        {
            List<Element> eles   = new List<Element>();
            ConnectorSet cons    = GetConnectors(ele);
            foreach (Connector con in cons)
            {
                Element neigbour = getNeighbour(con);
                if (neigbour != null)
                    eles.Add(neigbour);
            }
            return eles;
        }

        private Element getNeighbour(Connector con)
        {
            // this should return the neigbour
            ConnectorSet allConnections = con.AllRefs;
            foreach (Connector co in allConnections)
            {
                if (!doesConnectorHaveTheSameOwnerAsCaller(con, co))
                {
                    return co.Owner;
                }
            }
            return null;
        }

        private bool doesConnectorHaveTheSameOwnerAsCaller(Connector con, Connector item)
        {
            //TODO create extension methods of Element class that returns id integer value
            if (con.Owner.Id.IntegerValue == item.Owner.Id.IntegerValue) return true; return false;
        }

        private bool isConnectorOwnerInCheckList(Connector con)
        {
            if (elementsForRenumberingCheckList.Contains(con.Owner.Id.IntegerValue))
                return true;
            else return false;
        }

        private void getOriginElement(ICollection<ElementId> renumberingOrigin)
        {
            foreach (var item in renumberingOrigin)
            {
                if (renumberingOrigin.Count == 1) Origin = Doc.GetElement(item);
            }
        }

        private static ConnectorSet GetConnectors(Element e)
        {
            ConnectorSet connectors = null;

            if (e is FamilyInstance)
            {
                MEPModel m     = ((FamilyInstance)e).MEPModel;

                if (null != m
                  && null != m.ConnectorManager)
                {
                    connectors = m.ConnectorManager.Connectors;
                }
            }
            else if (e is Wire)
            {
                connectors = ((Wire)e)
                  .ConnectorManager.Connectors;
            }
            else
            {
                Debug.Assert(
                  e.GetType().IsSubclassOf(typeof(MEPCurve)),
                  "expected all candidate connector provider "
                  + "elements to be either family instances or "
                  + "derived from MEPCurve");

                if (e is MEPCurve)
                {
                    connectors = ((MEPCurve)e)
                      .ConnectorManager.Connectors;
                }
            }
            return connectors;
        }

        internal void setStartinParameterNumber(string str)
        {
            startNumber = str;
        }

        internal void setStartingParameterName(string str)
        {
            selectedPara = str;
        }

        #endregion RunHelpers

        #region RevitsLittleHelpers

        private void getElementsFromIds()
        {
            foreach (var eleId in unorderedElementIds)
            {
                elementsForRenumbering.Add(Doc.GetElement(eleId));
                elementsForRenumberingCheckList.Add(eleId.IntegerValue); // this one is just for quicker checking
            }
        }

        public List<String> returnParamList()
        {
            return commonParameters.ToList();
        }

        public void setStartingParameter(string name)
        {
            selectedPara = name;
        }

        private ICollection<ElementId> getCurrentSelection()
        {
            return UiDoc.Selection.GetElementIds();
        }

        //find all the common parameters in the elements set, used for combobox form
        private void getCommonParameters()
        {
            getElementsFromIds();
            populateListWithParameterNames();
        }

        //geting common parameters from all the elements in the selection
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

        #endregion RevitsLittleHelpers
    }
}