using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using BimExperts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BimExperts
{
    /// <summary>
    /// Use this for specific events
    /// </summary>
    class ExternalDBApp : IExternalDBApplication

    {
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            //application.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(DocumentOpenedEventHandler);
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            try
            {
                //Register event
                return ExternalDBApplicationResult.Succeeded;

            }
            catch (Exception)
            {
                 return ExternalDBApplicationResult.Failed;
            }
        }

        //public void DocumentOpenedEventHandler(object sender, DocumentOpenedEventArgs args)
        //{
        //    Document doc = args.Document;

        //    TimeStampsModel model = new TimeStampsModel(doc);
        //    model.CreateExtensibleStorage();
        
        //}
    }
}
