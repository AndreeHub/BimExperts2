using Autodesk.Revit.UI;
using BimExperts.Model;
using BimExperts.ViewModels;
using BimExperts.Views;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace BimExperts
{
    public class ExternalApp : IExternalApplication

    {
        #region Vars

        internal static ExternalApp thisApp = null;

        //Magic Renumber
        private MagicRenumberViewModel mrVmod;
        private MeasureAndCountViewModel macVmod;

        //Measure and count
        private MagicRenumber windowMagicRenumber;
        private MeasureAndCount windowMeasureAndCount;

        #endregion Vars

        #region InterfaceMethods  
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //preping window vairables
            windowMagicRenumber   = null;
            windowMeasureAndCount = null;

            thisApp               = this;

            System.Diagnostics.Debugger.Launch();

            #region Image and buttons

            //add images
            Uri InfoImagePath = new Uri("pack://application:,,,/BimExperts;component/Resources/bim32x32.png");
            Uri MeasureAndCountImagePath = new Uri("pack://application:,,,/BimExperts;component/Resources/Measure and count.png");
            Uri TransitionImagePath = new Uri("pack://application:,,,/BimExperts;component/Resources/Magic Transition.png");
            Uri MagicTransition = new Uri("pack://application:,,,/BimExperts;component/Resources/Magic Renumber.png");
            //Create Bitmap image
            BitmapImage InfoImage = new BitmapImage(InfoImagePath);
            BitmapImage MeasureAndCountImage = new BitmapImage(MeasureAndCountImagePath);
            BitmapImage ChangeHostedLevelImage = new BitmapImage(MagicTransition);
            BitmapImage TransitionImage = new BitmapImage(TransitionImagePath);
            //Create ribbon element

            //create ribbon
            string AssemblyPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData InfoData = new PushButtonData("Info", "Hello", AssemblyPath, "BimExperts.TestCommand");
            PushButtonData MeasureAndCountData = new PushButtonData("Measure and Count", "Measure \n and Count", AssemblyPath, "BimExperts.TestCommand");
            PushButtonData TransitionData = new PushButtonData("Magic Transition", "Magic \n Transition", AssemblyPath, "BimExperts.CreateTransition");
            PushButtonData ChangeHosteLevelData = new PushButtonData("Change Hosted Level", "Magic \n Renumber", AssemblyPath, "BimExperts.StartMagicRenumber");

            InfoData.LargeImage = InfoImage;
            MeasureAndCountData.LargeImage = MeasureAndCountImage;
            TransitionData.LargeImage = TransitionImage;
            ChangeHosteLevelData.LargeImage = ChangeHostedLevelImage;

            #endregion Image and buttons

            //Add buttons to ribbon
            application.CreateRibbonTab("BimExperts");
            RibbonPanel panel = application.CreateRibbonPanel("BimExperts", "Tool Box");

            panel.AddItem(InfoData);
            panel.AddItem(MeasureAndCountData);
            panel.AddItem(TransitionData);
            panel.AddItem(ChangeHosteLevelData);

            return Result.Succeeded;
        }
        #endregion

        #region MagicRenumberIni

        public void ShowWindowMagicrenumber(UIApplication uiapp)
        {
            //if we do not have a dialoge yet, create it
            if (windowMagicRenumber == null)
            {
                //A new handeler to handle request posting by the dialog
                MagicRenumberHandler handler    = new MagicRenumberHandler();

                //External event for the dialog to use (to post requests)
                ExternalEvent exEvent           = ExternalEvent.Create(handler);

                //create LogicClass for MagicRenumber

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

        public void ShowMeasureAndCount(UIApplication uiapp)
        {
            if (windowMeasureAndCount == null)
            {
                windowMeasureAndCount             = new MeasureAndCount();
                macVmod                           = new MeasureAndCountViewModel(windowMeasureAndCount);
                windowMeasureAndCount.DataContext = macVmod;

                windowMeasureAndCount.Show();
            }

            if(windowMeasureAndCount != null)
            {
                windowMeasureAndCount.Close();
            }
        }

        #endregion MeasureAndCountINI
    }
}