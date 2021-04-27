using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using BimExperts.Model;
using BimExperts.ViewModels;
using BimExperts.Views;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
namespace BimExperts
{
    public class ExternalApp : IExternalApplication

    {
        #region Vars

        internal static ExternalApp thisApp = null;
        
        TimeStampsModel model = null;
         
       
        //Magic Renumber
        private MagicRenumberViewModel mrVmod   ;
        private MeasureAndCountViewModel macVmod;

        //Measure and count
        private MagicRenumber windowMagicRenumber    ;
        private MeasureAndCount windowMeasureAndCount;
        private BitmapImage transferImage            ;

        // Timestamps
        private ControlledApplication conApp;


        #endregion Vars

        #region InterfaceMethods

        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(DocumentOpenedEventHandler);
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //preping window vairables
            windowMagicRenumber   = null;
            windowMeasureAndCount = null;
            thisApp               = this;
           
            System.Diagnostics.Debugger.Launch();
            conApp = application.ControlledApplication;
            #region Image and buttons

            //add images
            Uri InfoImagePath            = new Uri("pack://application:,,,/BimExperts;component/Resources/bim32x32.png");
            Uri MeasureAndCountImagePath = new Uri("pack://application:,,,/BimExperts;component/Resources/Measure and count.png");
            Uri TransitionImagePath      = new Uri("pack://application:,,,/BimExperts;component/Resources/Magic Transition.png");
            Uri MagicTransition          = new Uri("pack://application:,,,/BimExperts;component/Resources/Magic Renumber.png");
            Uri BimExpertsLogo           = new Uri("pack://application:,,,/BimExperts;component/Resources/bimexperts.png");
            Uri TimeStampsImagePath      = new Uri("pack://application:,,,/BimExperts;component/Resources/TimeStamps.png");
            //Create Bitmap image
            BitmapImage InfoImage              = new BitmapImage(InfoImagePath           );
            BitmapImage MeasureAndCountImage   = new BitmapImage(MeasureAndCountImagePath);
            BitmapImage ChangeHostedLevelImage = new BitmapImage(MagicTransition         );
            BitmapImage TransitionImage        = new BitmapImage(TransitionImagePath     );
            BitmapImage TimeStampsImage        = new BitmapImage(TimeStampsImagePath     );
            transferImage                      = new BitmapImage(BimExpertsLogo          );
            //Create ribbon element

            //create ribbon
            string AssemblyPath = Assembly.GetExecutingAssembly().Location;

            // Create button data
            PushButtonData InfoData             = new PushButtonData("Info", "Hello", AssemblyPath, "BimExperts.TestCommand");
            PushButtonData MeasureAndCountData  = new PushButtonData("Measure and Count", "Measure \n and Count", AssemblyPath, "BimExperts.Commands.MeasureAndCountComm");
            PushButtonData TransitionData       = new PushButtonData("Magic Transition", "Magic \n Transition", AssemblyPath, "BimExperts.CreateTransition");
            PushButtonData ChangeHosteLevelData = new PushButtonData("Change Hosted Level", "Magic \n Renumber", AssemblyPath, "BimExperts.StartMagicRenumber");
            PushButtonData TimeStampsData       = new PushButtonData("Time Stamps", "Place \n TimeStamps", AssemblyPath, "BimExperts.TimeStamps");

            // Set the button data
            InfoData.LargeImage             = InfoImage             ;
            MeasureAndCountData.LargeImage  = MeasureAndCountImage  ;
            TransitionData.LargeImage       = TransitionImage       ;
            ChangeHosteLevelData.LargeImage = ChangeHostedLevelImage;
            TimeStampsData.LargeImage       = TimeStampsImage       ;

            #endregion Image and buttons

            //Add buttons to ribbon
            application.CreateRibbonTab("BimExperts");
            RibbonPanel panel = application.CreateRibbonPanel("BimExperts", "Tool Box");

            panel.AddItem(InfoData            );
            panel.AddItem(MeasureAndCountData );
            panel.AddItem(TransitionData      );
            panel.AddItem(ChangeHosteLevelData);
            panel.AddItem(TimeStampsData      );
            
            //Register Event for document opening
            application.ControlledApplication.DocumentOpened += new EventHandler<DocumentOpenedEventArgs>(DocumentOpenedEventHandler);

            
            
            
            
            
            return Result.Succeeded;

  


        }

        #endregion InterfaceMethods

        #region MagicRenumberIni

        public void ShowWindowMagicrenumber(UIApplication uiapp)
        {
            //if we do not have a dialoge yet, create it
            if (windowMagicRenumber             == null)
            {
                //A new handeler to handle request posting by the dialog
                MagicRenumberHandler handler    = new MagicRenumberHandler();

                //External event for the dialog to use (to post requests)
                ExternalEvent exEvent           = ExternalEvent.Create(handler);

                //create LogicClass for 
                MagicRenumberLogic logic        = new MagicRenumberLogic();

                // We give the objects to the new dialog;
                // The dialog becomes the owner responsible fore disposing them, eventual
                mrVmod                          = new MagicRenumberViewModel(exEvent, handler, logic);
                windowMagicRenumber             = new MagicRenumber();
                windowMagicRenumber.DataContext = mrVmod;

                windowMagicRenumber.Show();
            }
        }

        #endregion MagicRenumberIni

        #region MeasureAndCountINI

        public void ShowMeasureAndCount(UIApplication uiapp, UIDocument uiDOC)
        {
            //if (windowMeasureAndCount == null)
            // {
            windowMeasureAndCount = new MeasureAndCount();
            windowMeasureAndCount.uiLogoImg.Source = transferImage;
            macVmod = new MeasureAndCountViewModel(windowMeasureAndCount, uiDOC);

            windowMeasureAndCount.DataContext = macVmod;
            windowMeasureAndCount.ShowDialog();
            // }
        }

        #endregion MeasureAndCountINI

        #region TimestampsINI

        public void DocumentOpenedEventHandler(object sender, DocumentOpenedEventArgs args)
        {
            Document doc = args.Document;
            //TaskDialog.Show("Hellothere", "We are in the event handler");
            model = new TimeStampsModel(doc);
            //TaskDialog.Show("Hellothere", "Class was created");
            model.CreateExtensibleStorage();
            //TaskDialog.Show("Hellothere", "Extensible storage was created");
            model.SetUpUpdater();
           // TaskDialog.Show("hello again","The setup was executed");

        }
        public void RunCommand(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            model.getCategories(doc);

            model.SetUpProjectParams(commandData.Application);

            model.SetElementInformation(commandData.Application.ActiveUIDocument.Document);
        }
        #endregion



    }
}