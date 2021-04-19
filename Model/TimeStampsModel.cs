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
        public static int elements_affected;

        // this set stores all the elements that the updater has edited to be used in the button nexecution context
        public static HashSet<ElementId> eleIdsForTransfer = new HashSet<ElementId>();
        public static HashSet<Category> eleIdsCats = new HashSet<Category>();
        public TimeStampsModel(Document doc)
        {
            this.doc = doc;
        }
        // get the list of all unedited elements, reads the time infrom from entity and places in in the shared parameter folder
        public static void SetElementInformation(Document doc)
        {
            elements_affected = eleIdsForTransfer.Count;
            //loop through the list of elements
            foreach (ElementId id in eleIdsForTransfer)
            {
                Element ele = doc.GetElement(id);
                String StringBuilder = "";
                String createdTime = ele.GetEntity(sch).Get<String>(pnCreatedTime);
                String changedTime = ele.GetEntity(sch).Get<String>(pnChangedTime);
                StringBuilder = "Created: " + createdTime + " Changed: " + changedTime;

                //place info into elements;
                //In a transaction
                using (Transaction trans = new Transaction(doc, "Trans1"))
                {
                    trans.Start();

                    ele.LookupParameter(spTstParName).Set(StringBuilder);
                    TaskDialog taskDialog = new TaskDialog("Element edite");
                    // display the number of elements in the dialogue
                   
                    trans.Commit();
                }
                eleIdsForTransfer.Remove(id);
                TaskDialog.Show("Elemenets edited", " " + elements_affected.ToString());
            }
            
        }
        internal void CreateExtensibleStorage()
        {
            sch = createSchema(schemaGuid);
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
        //Group Test, 1 shared parameter test
        //Params Timestamp_Test_
        internal static void SetUpProjectParams(UIApplication application)
        {
            DefinitionFile sharedParameterFile = application.Application.OpenSharedParameterFile();
            // create parametars on the elements that can store the time data
            bool groupFound = false;
            foreach (DefinitionGroup group in sharedParameterFile.Groups)
            {
                //find the group we are using, when you find it check for parameters
                if (group.Name == spTstGrpName)
                {
                    groupFound = false;
                    //create category set for parameter to bind to
                    CategorySet catSet = createCategorySet(application);

                    //need to check for existance of parameters, if they dont exist add 
                    AddParam(group, application, catSet);

                    continue;
                }
            }
            if (!groupFound)
            {
                DefinitionGroup group =sharedParameterFile.Groups.Create(spTstGrpName);
                CreateParameter(group);

            }

        }
        private static CategorySet createCategorySet(UIApplication application)
        {
            CategorySet catSet = application.Application.Create.NewCategorySet();
            foreach (Category cat in eleIdsCats)
            {
                catSet.Insert(cat);
            }

            return catSet;
        }
        private static void AddParam(DefinitionGroup group, UIApplication application, CategorySet catSet)
        {
            // check for parameter and group existance existance,add if its not there
            ExternalDefinition extDef = group.Definitions.get_Item(spTstParName) as ExternalDefinition;

            if (extDef != null)
            {
                BindParameter(application, catSet, extDef);
                return;
            }
            else
            {
                CreateParameter(group);
                BindParameter(application, catSet, extDef);
            }


        }

        private static void CreateParameter(DefinitionGroup group)
        {
            ExternalDefinitionCreationOptions opt = new ExternalDefinitionCreationOptions(spTstParName, ParameterType.Text);
            group.Definitions.Create(opt);
            
        }

        private static void BindParameter(UIApplication application, CategorySet catSet, ExternalDefinition extDef)
        {
            using (Transaction t = new Transaction(application.ActiveUIDocument.Document))
            {
                t.Start("Add shared parameter");
                //param binding
                InstanceBinding newIb = application.Application.Create.NewInstanceBinding(catSet);
                application.ActiveUIDocument.Document.ParameterBindings.Insert(extDef, newIb, BuiltInParameterGroup.PG_DATA);
                t.Commit();
                return;
            }
        }
    }
}
