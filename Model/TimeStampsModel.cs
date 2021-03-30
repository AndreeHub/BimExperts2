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
    class TimeStampsModel
    {
        //private UIDocument uidoc;
        private Autodesk.Revit.DB.Document doc;


        private static string schemaName          = "Time stamps data";
        private static string schemaDocumentation = "Storage for time data";
        private static string schemaGuid          = "8107fd14-c2cb-48e3-b079-dddbbea1caef";


        public static string pnCreatedTime        = "Created time";
        public static string pnCreatedBy          = "Created by";
        public static string pnChangedTime        = "Changed time";
        public static string pnChangedBy          = "Changed by";

        public static Schema sch;
        

        public TimeStampsModel(Document doc)
        {
            this.doc   = doc  ;
            
        } 

        internal void CreateExtensibleStorage()
        {
            sch=createSchema(schemaGuid);
        }

        private static Schema createSchema(string str)
        {
            Guid schemaGuid = new Guid(str);

            SchemaBuilder schemaBuilder = new SchemaBuilder(schemaGuid);

            //set read access
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            //set write Level
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);

            //set Schema Name
            schemaBuilder.SetSchemaName(schemaName);

            //set documentation
            schemaBuilder.SetDocumentation(schemaDocumentation);

            //create Fields
            FieldBuilder fieldBuilder0 = schemaBuilder.AddSimpleField(pnCreatedTime, typeof(string));
            FieldBuilder fieldBuilder1 = schemaBuilder.AddSimpleField(pnCreatedBy, typeof(string));
            FieldBuilder fieldBuilder2 = schemaBuilder.AddSimpleField(pnChangedTime, typeof(string));
            FieldBuilder fieldBuilder3 = schemaBuilder.AddSimpleField(pnChangedBy, typeof(string));

            //register the Schema
            Schema schema = schemaBuilder.Finish();

            return schema;
        }

        public static void createUIUpdater()
        {

        }



        
    }
}
