using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
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

        public TimeStampsDynamicUpdater(Guid guid)
        {
            uId = new UpdaterId(new AddInId(new Guid("e15bc4c9-ed32-41fd-8991-599ede4e3d3d")), guid);
        }

        public void Execute(UpdaterData data)
        {
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

        }

        public string GetAdditionalInformation()
        {
            return "Updated that collects changed element and updates their times";
        }

        public ChangePriority GetChangePriority()
        {

            return ChangePriority.Annotations;
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
            Schema schemy = TimeStampsModel.sch                           ;
            Entity entsy  = new Entity(schemy)                            ;
            Field fieldsy = schemy.GetField(TimeStampsModel.pnChangedTime);
            entsy.Set(fieldsy, time)                                      ;

            return entsy;
        }

        private Entity createNewElEntity(string time)
        {
            //create extensible storage
            Schema schemy = TimeStampsModel.sch                           ;
            Entity entsy  = new Entity(schemy)                            ;
            Field fieldsy = schemy.GetField(TimeStampsModel.pnCreatedTime);

            entsy.Set(fieldsy, time);

            return entsy;
        }

        private void appendEntityToElements(Document doc,ICollection<ElementId> eleIds, Entity ent)
        {
            foreach (ElementId id in eleIds)
            {
                doc.GetElement(id).SetEntity(ent);
                TimeStampsModel.eleIdsForTransfer.Add(id);
            }
        }



    }
}
