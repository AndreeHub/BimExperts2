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
        #region Vars
        //private UIDocument uidoc;
        private Autodesk.Revit.DB.Document doc;



        private string schemaName = "Time_stamps_data";
        private string schemaDocumentation = "Storage for time data";
        private string schemaGuid = "8107fd14-c2cb-48e3-b079-dddbbea1caef";

        public  string pnCreatedTime = "Created_time";
        public  string pnCreatedBy = "Created_by";
        public  string pnChangedTime = "Changed_time";
        public  string pnChangedBy = "Changed_by";
        public  string pnEditedByUser = "Edited_by_user";       
        public  string spTstGrpName = "Timestamps_TEST_GROUP";
        public  string spTstParName = "Timestams_Data";

        
        public Schema sch;
        public static int elements_affected;

        // this set stores all the elements that the updater has edited to be used in the button nexecution context
        public HashSet<ElementId> eleIdsForTransfer = new HashSet<ElementId>();
        private HashSet<Category> eleIdsCats = new HashSet<Category>(); 
        #endregion
        public TimeStampsModel(Document doc1)
        {
            this.doc = doc1;
        }
        // get the list of all unedited elements, reads the time infrom from entity and places in in the shared parameter folder
        public  void SetElementInformation(Document doc)
        {   // each transaction trigers a dynamic model updater call, so we need to be carefull when editing
            using (Transaction trans = new Transaction(doc, "Trans1")) 
            {
            
                trans.Start();
                elements_affected = eleIdsForTransfer.Count;
                //loop through the list of elements
                foreach (ElementId id in eleIdsForTransfer)
                {
                    Element ele = doc.GetElement(id);
                    String StringBuilder = "";
                    String createdTime = ele.GetEntity(sch).Get<String>(pnCreatedTime);
                    String changedTime = ele.GetEntity(sch).Get<String>(pnChangedTime);
                    StringBuilder = "Created: " + createdTime + " Changed: " + changedTime;


                    Parameter para = ele.LookupParameter(spTstParName);
                    para.Set(StringBuilder);

                    
                    
                }
                eleIdsForTransfer.Clear();
                trans.Commit();
                
            
            }

        }
        internal void CreateExtensibleStorage()
        {
            sch = createSchema(schemaGuid);
        }
        private  Schema createSchema(string str)
        {
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Schema creation");
                Schema schema = SchemaCreationLogic(str);
                
                _ = transaction.Commit();
                return schema;

            }

        }
        private Schema SchemaCreationLogic(string str)
        {
            Guid schemaGuid = new Guid(str);

            SchemaBuilder schemaBuilder = new SchemaBuilder(schemaGuid);

            //set read access
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            //set write Level
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Vendor);
            schemaBuilder.SetVendorId("BimExperts");

            //set Schema Name
            schemaBuilder.SetSchemaName(schemaName);

            //set documentation
            schemaBuilder.SetDocumentation(schemaDocumentation);

            //create Fields
            FieldBuilder fieldBuilder0 = schemaBuilder.AddSimpleField(pnCreatedTime, typeof(string));
            FieldBuilder fieldBuilder1 = schemaBuilder.AddSimpleField(pnCreatedBy, typeof(string));
            FieldBuilder fieldBuilder2 = schemaBuilder.AddSimpleField(pnChangedTime, typeof(string));
            FieldBuilder fieldBuilder3 = schemaBuilder.AddSimpleField(pnChangedBy, typeof(string));
            FieldBuilder fieldBuilder4 = schemaBuilder.AddSimpleField(pnEditedByUser, typeof(bool));

            //register the Schema
            Schema schema = schemaBuilder.Finish();
            return schema;
        }
        internal  void getCategories(Document document)
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
        internal void SetUpUpdater()
        {
            // This part is for the updater
            Guid guid = new Guid("e077f599-5c89-43cd-b550-986237c50b85");
            TimeStampsDynamicUpdater tsdynUpdt = new TimeStampsDynamicUpdater(guid, this);
            // registers the updater
            UpdaterRegistry.RegisterUpdater(tsdynUpdt, true);

            List<BuiltInCategory> builtInCategories = new List<BuiltInCategory>();

            // i think these are all relevant
            // could not find another way
            //builtInCategories.Add(BuiltInCategory.OST_Ceilings);
            //builtInCategories.Add(BuiltInCategory.OST_Conduit);
            //builtInCategories.Add(BuiltInCategory.OST_CurtainWallMullions);
            builtInCategories.Add(BuiltInCategory.OST_Walls);
            //builtInCategories.Add(BuiltInCategory.OST_Floors);
            //builtInCategories.Add(BuiltInCategory.OST_DuctCurves);
            //builtInCategories.Add(BuiltInCategory.OST_PipeCurves);
            //builtInCategories.Add(BuiltInCategory.OST_Railings);
            //builtInCategories.Add(BuiltInCategory.OST_Roofs);
            //builtInCategories.Add(BuiltInCategory.OST_Stairs);
            //builtInCategories.Add(BuiltInCategory.OST_CableTray);

            //Craete multicategory filter
            ElementMulticategoryFilter mCatFilter = new ElementMulticategoryFilter(builtInCategories);
            ElementFilter ifilter = new ElementClassFilter(typeof(FamilyInstance));

            LogicalAndFilter filter = new LogicalAndFilter(mCatFilter, ifilter);

            ElementFilter filtTest = new ElementCategoryFilter(BuiltInCategory.OST_Walls);

            ChangeType change = ChangeType.ConcatenateChangeTypes(Element.GetChangeTypeAny(), Element.GetChangeTypeElementAddition());

            UpdaterRegistry.AddTrigger(tsdynUpdt.GetUpdaterId(), filtTest, change);
        }
        private  void createNewGroupAndParameter(UIApplication application, DefinitionFile sharedParameterFile)
        {
            // transaction
            using (Transaction t = new Transaction(application.ActiveUIDocument.Document))
            {
                t.Start("Create Group and parameter");
                DefinitionGroup group = sharedParameterFile.Groups.Create(spTstGrpName);
                CreateParameter(group);
                t.Commit();
                return;
            }
        }
        private  DefinitionGroup CreateGroup(DefinitionFile sharedParameterFile)
        {
           return sharedParameterFile.Groups.Create(spTstGrpName);
        }
        private  CategorySet createCategorySet(UIApplication application)
        {
            CategorySet catSet = application.Application.Create.NewCategorySet();
            foreach (Category cat in eleIdsCats)
            {
                catSet.Insert(cat);
            }

            return catSet;
        }
        private  void AddParam(DefinitionGroup group, UIApplication application, CategorySet catSet)
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
        private  void CreateParameter(DefinitionGroup group)
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
        internal  void SetUpProjectParams(UIApplication application)
        {
            DefinitionFile sharedParameterFile = application.Application.OpenSharedParameterFile();
            // create parametars on the elements that can store the time data
            bool groupFound = false;
            foreach (DefinitionGroup group in sharedParameterFile.Groups)
            {
                //find the group we are using, when you find it check for parameters
                if (group.Name == spTstGrpName)
                {
                    groupFound = true;

                    //create category set for parameter to bind to
                    CategorySet catSet = createCategorySet(application);

                    //need to check for existance of parameters, if they dont exist add 
                    AddParam(group, application, catSet);

                    break;
                }
            }
            if (!groupFound)
            {
                createNewGroupAndParameter(application, sharedParameterFile);
                return;

            }

        }


       
    }
}
