using Autodesk.Revit.DB;
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

            // take all new elements
            ICollection<ElementId> elesAdded = data.GetAddedElementIds();
            ICollection<ElementId> elesChanged = data.GetModifiedElementIds();
            //get time
            string time = DateTime.Now.ToString();

            //
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
    }
}
