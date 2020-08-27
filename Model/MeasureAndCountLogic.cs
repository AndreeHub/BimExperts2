using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BimExperts.Model
{
    public class MeasureAndCountLogic
    {
        private Selection selection;
        private ICollection<ElementId> selectedIds;
        private string FamilyAndInstanceName;
        private double Lenght;

        public Dictionary<string, List<Element>> dict;
        public List<string> textForListView;

        public UIDocument uidoc;
        public Document doc;

        public MeasureAndCountLogic(UIDocument uidocument,Document document)
        {
            uidoc = uidocument;
            doc = document;
            selection = uidoc.Selection;
            selectedIds = selection.GetElementIds();

            Lenght = 0;

            dict = new Dictionary<string, List<Element>>();

            textForListView = new List<string>();
        }

        internal void MeasureAndCount()
        {
            populateDict();
            populateOutputList();
        }

        private void populateOutputList()
        {
            textForListView.Add("\nLength of elments in selection is : " + Lenght.ToString());
            foreach (var items in dict.Keys)
            {
                textForListView.Add("\n" + items + "  count:  " + dict[items].Count.ToString());
            }

        }

        internal List<string> getInputList()
        {
            return textForListView;
        }

        private void populateDict()
        {
            foreach(ElementId id in selectedIds)
            {
                Element ele = doc.GetElement(id);
                ElementId eTypeId = ele.GetTypeId(); //get the id of the elemnt type    
                ElementType eType = doc.GetElement(eTypeId) as ElementType;

                //Add up all the parameters that have the length property
                if (ele.LookupParameter("Length") != null)
                {
                    Parameter paraLength = ele.LookupParameter("Length");
                    Lenght += Math.Round(paraLength.AsDouble() * 304.8, 2);
                    continue;
                }

                //single out fittings that have the same family and type anad instance name but diferent parameter
                //in this case a pipe bend of 45 and 90 degree have a same name but not the same angle parameter
                if (ele.LookupParameter("CAx_Winkel") != null)
                {
                    double angle = ele.LookupParameter("CAx_Winkel").AsDouble() * 180 / Math.PI;
                    string FittingAngle = Math.Round(angle, 2).ToString();
                    FamilyAndInstanceName = eType.FamilyName.ToString() + ", " + ele.Name.ToString() + ", CAx_Winkel:  " + FittingAngle;
                }
                else FamilyAndInstanceName = eType.FamilyName.ToString() + ", " + ele.Name.ToString();
                //new comment


                //Populate the dictionary with the elements that dont containt he length property
                if (!dict.ContainsKey(FamilyAndInstanceName))
                {
                    dict.Add(FamilyAndInstanceName, new List<Element>());
                }
                dict[FamilyAndInstanceName].Add(ele);
            }
        }
    }
}