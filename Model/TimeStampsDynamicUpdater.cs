using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BimExperts.Model
{
    class TimeStampsDynamicUpdater : IUpdater
    {
        #region Vars
        static UpdaterId uId;
        private TimeStampsModel modeli; 
        #endregion

        #region Updater Mandated functions
        public TimeStampsDynamicUpdater(Guid guid, TimeStampsModel model)
        {
            modeli = model;
            uId = new UpdaterId(new AddInId(new Guid("e15bc4c9-ed32-41fd-8991-599ede4e3d3d")), guid);
        }
        public void Execute(UpdaterData data)
        {
            //TaskDialog.Show("Hellothere", "We are in the updater");
            Document doc = data.GetDocument();
            // take all new and change elements elements
            ICollection<ElementId> elesAdded = data.GetAddedElementIds();
            ICollection<ElementId> elesChanged = data.GetModifiedElementIds();

            // get date and time
            string time = DateTime.Now.ToString();
            //edit changed elements
            editChangedElements(doc, elesChanged, time);
            //same for new elements
            elesAdded.ToList().ForEach(e => editNewElements(e,time,doc));

            addElementsToGlobalCollector(elesAdded, elesChanged, modeli);

        }

        private void editNewElements(ElementId e,string time,Document doc)
        {
            Schema schemy = modeli.sch;
            
            Entity entsy = new Entity(schemy);
            entsy.Set(modeli.pnEditedByUser, false);
            entsy.Set(modeli.pnChangedTime, "");
            entsy.Set(modeli.pnCreatedTime, time);
            doc.GetElement(e).SetEntity(entsy);
        }

        public string GetAdditionalInformation()
        {
            return "Updated that collects changed element and updates their times";
        }
        public ChangePriority GetChangePriority()
        {

            return ChangePriority.FreeStandingComponents;
        }
        public UpdaterId GetUpdaterId()
        {
            return uId;
        }
        public string GetUpdaterName()
        {
            return "Time to Parameter Updater";
        } 
        #endregion



        private void editChangedElements(Document doc, ICollection<ElementId> elesChanged, string time)
        {
            //for all modified elements there are 3 possible options
            //1. Does not have a schema
            //2. Has a schema but the updater was triggerd by user pressing hte button to move data to shared params. we should leave out this time stamp out of record
            //3. Has a schema and was changed regulary, in this case we need to copy the Created Time. If there was no created time it will just be empty in the ened
            Schema schemy = modeli.sch;
            foreach (ElementId elementId in elesChanged)
            {
                //figure out how to see if schema exits already on the elemnt
                Element ele = doc.GetElement(elementId);
                if (schemaExistsOnElement(schemy, ele))
                {
                    //check if the element was edited by user
                    if (ele.GetEntity(schemy).Get<bool>(modeli.pnEditedByUser))
                    {
                        //create a new entity and set its bool to false
                        //true will only be set up from the button execution context


                        Entity entsy = new Entity(schemy);
                        entsy.Set(modeli.pnEditedByUser, false);
                        //copy changed time and created time
                        entsy.Set(modeli.pnChangedTime, ele.GetEntity(schemy).Get<string>(modeli.pnChangedTime));
                        entsy.Set(modeli.pnCreatedTime, ele.GetEntity(schemy).Get<string>(modeli.pnCreatedTime));
                        ele.SetEntity(entsy);

                    }
                    else
                    {
                        Entity entsy = new Entity(schemy);
                        entsy.Set(modeli.pnEditedByUser, false);
                        entsy.Set(modeli.pnChangedTime, time);
                        entsy.Set(modeli.pnCreatedTime, ele.GetEntity(schemy).Get<string>(modeli.pnCreatedTime));
                        ele.SetEntity(entsy);

                    
                    
                    
                    
                    
                    
                    }

                }
                else
                {
                    Entity entsy = new Entity(schemy);
                    entsy.Set(modeli.pnEditedByUser, false);
                    entsy.Set(modeli.pnChangedTime, time);
                    entsy.Set(modeli.pnCreatedTime, "");
                    ele.SetEntity(entsy);
                }
            }
        }
        private static bool schemaExistsOnElement(Schema schemy, Element ele)
        {
            return ele.GetEntitySchemaGuids().Contains(schemy.GUID);
        }
        private static void addElementsToGlobalCollector(ICollection<ElementId> elesAdded, ICollection<ElementId> elesChanged,TimeStampsModel model)
        {

            model.eleIdsForTransfer.UnionWith(elesAdded.Concat(elesChanged));
           
        }
      



    }
}
