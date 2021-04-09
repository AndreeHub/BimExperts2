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
        public Autodesk.Revit.DB.Document doc;
        

        private static string schemaName          = "Time stamps data";
        private static string schemaDocumentation = "Storage for time data";
        private static string schemaGuid          = "8107fd14-c2cb-48e3-b079-dddbbea1caef";

        public static string pnCreatedTime        = "Created time";
        public static string pnCreatedBy          = "Created by"  ;

        public static string pnChangedTime        = "Changed time";

       
        public static string pnChangedBy          = "Changed by"  ;

        public static string spTstGrpName         = "Timestamps_TEST_GROUP";
        public static string spTstParName         = "Timestams_Data";

        public static Schema sch;

        // this set stores all the elements that the updater has edited to be used in the button nexecution context
        public static HashSet<ElementId> eleIdsForTransfer = new HashSet<ElementId>();
        public static HashSet<Category> eleIdsCats = new HashSet<Category>();
       
        public TimeStampsModel(Document doc)
        {
            this.doc = doc;
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

        internal static void getCategories(Document document)
        {
            foreach (ElementId id in eleIdsForTransfer)
            {
                Category cat = document.GetElement(id).Category;
                eleIdsCats.Add(cat);
            }
           
        }
        //Create shared parameters
        //Group Test
        //Params Timestamp_Test_
        internal static void SetUpProjectParams(UIApplication application)
        {
            application.Application.OpenSharedParameterFile();
            // create parametars on the elements that can store the time data
           
            DefinitionFile defFile = application.Application.OpenSharedParameterFile();
            //create group Name
            DefinitionGroup group  = defFile.Groups.get_Item(spTstGrpName);
            if (group == null)
            {
                defFile.Groups.Create(spTstGrpName);
            }

            //create shared param definition
            Definition def = group.Definitions.get_Item(spTstParName);

            ParameterType _defType = ParameterType.Text;

            if (null == def)
            {
                ExternalDefinitionCreationOptions opt = new ExternalDefinitionCreationOptions(spTstParName, _defType);

                opt.Visible = true;

                def = group.Definitions.Create(opt);

            }
            // Create the category sset
            CategorySet cat = null;
            CategorySet catSet = application.Application.Create.NewCategorySet();
            foreach (Category cats in eleIdsCats)
            {
                catSet.Insert(cats);
            }

            Binding binding = application.Application.Create.NewInstanceBinding(catSet);

            application.ActiveUIDocument.Document.ParameterBindings.Insert(def,binding);

        }
        
    }
}
