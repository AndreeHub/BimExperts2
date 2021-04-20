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
        static UpdaterId uId;
        private TimeStampsModel modeli;

        public TimeStampsDynamicUpdater(Guid guid, TimeStampsModel model)
        {
            modeli = model;
            uId = new UpdaterId(new AddInId(new Guid("e15bc4c9-ed32-41fd-8991-599ede4e3d3d")), guid);
        }

        public void Execute(UpdaterData data)
        {
            TaskDialog.Show("Hellothere", "We are in the updater");
            Document doc = data.GetDocument();
            // take all new elements
            ICollection<ElementId> elesAdded = data.GetAddedElementIds();
            ICollection<ElementId> elesChanged = data.GetModifiedElementIds();
            string time = DateTime.Now.ToString();
            
            Entity changeEntity = createChangedEntity(time);

            Entity createdEntity = createNewElEntity(time);
            //assign  Entities to elements
            appendEntityToElements(doc, elesAdded, createdEntity);
            appendEntityToElements(doc, elesChanged, changeEntity);

            // add the new elements to global Element array

            addElementsToGlobalCollector(elesAdded, elesChanged, modeli);

        }

        private static void addElementsToGlobalCollector(ICollection<ElementId> elesAdded, ICollection<ElementId> elesChanged,TimeStampsModel model)
        {
            foreach (ElementId elementId in elesAdded)
            {
                model.eleIdsForTransfer.Add(elementId);
            }
            foreach (ElementId elementId in elesChanged)
            {
                model.eleIdsForTransfer.Add(elementId);
            }
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
        /// <summary>
        /// Craete entity, get time and set the entity paraemter 
        /// </summary>
        /// <returns></returns>
        private Entity createChangedEntity(string time)
        {
            //create extensible storage
            Schema schemy = modeli.sch                           ;
            Entity entsy  = new Entity(schemy)                            ;
            Field fieldsy = schemy.GetField(modeli.pnChangedTime);
            entsy.Set(fieldsy, time)                                      ;
            return entsy;
        }

        private Entity createNewElEntity(string time)
        {
            //create extensible storage
            Schema schemy = modeli.sch                           ;
            Entity entsy  = new Entity(schemy)                            ;
            Field fieldsy = schemy.GetField(modeli.pnCreatedTime);

            entsy.Set(fieldsy, time);

            return entsy;
        }

        private void appendEntityToElements(Document doc,ICollection<ElementId> eleIds, Entity ent)
        {
            foreach (ElementId id in eleIds)
            {
                //set entities
                doc.GetElement(id).SetEntity(ent);
                //add to a unique list of elements
                modeli.eleIdsForTransfer.Add(id);
            }
        }



    }
}
