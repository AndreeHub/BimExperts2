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
        internal static ExternalApp thisApp = null;

        private MagicRenumberViewModel mrVmod;
        private MagicRenumber window;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            window = null;
            thisApp = this;

            System.Diagnostics.Debugger.Launch();

            #region Image and buttons

            //add images
            Uri InfoImagePath = new Uri("pack://application:,,,/BimExperts;component/Resources/bim32x32.png");
            Uri MeasureAndCountImagePath = new Uri("pack://application:,,,/BimExperts;component/Resources/bim32x32_1.png");
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
            PushButtonData MeasureAndCountData = new PushButtonData("Measure and Count", "Hello", AssemblyPath, "BimExperts.TestCommand");
            PushButtonData TransitionData = new PushButtonData("Magic Transition", "Magic Transition", AssemblyPath, "BimExperts.CreateTransition");
            PushButtonData ChangeHosteLevelData = new PushButtonData("Change Hosted Level", "Hello", AssemblyPath, "BimExperts.TestCommand");

            InfoData.LargeImage = InfoImage;
            MeasureAndCountData.LargeImage = MeasureAndCountImage;
            TransitionData.LargeImage = TransitionImage;
            ChangeHosteLevelData.LargeImage = ChangeHostedLevelImage;

            #endregion Image and buttons

            //Add buttons to ribbon
            application.CreateRibbonTab("BimExperts");
            RibbonPanel panel = application.CreateRibbonPanel("BimExperts", "Commands");

            panel.AddItem(InfoData);
            panel.AddItem(MeasureAndCountData);
            panel.AddItem(TransitionData);
            panel.AddItem(ChangeHosteLevelData);

            return Result.Succeeded;
        }

        //this method creates and shows a modeless dialog, unells it alreay exists
        public void ShowWindow(UIApplication uiapp)
        {
            //if we do not have a dialoge yet, create it
            if (window == null)
            {
                //A new handeler to handle request posting by the dialog
                MagicRenumberHandler handler = new MagicRenumberHandler();

                //External event for the dialog to use (to post requests)
                ExternalEvent exEvent = ExternalEvent.Create(handler);

                //create LogicClass for MagicRenumber

                MagicRenumberLogic logic = new MagicRenumberLogic();

                // We give the objects to the new dialog;
                // The dialog becomes the owner responsible fore disposing them, eventual

                mrVmod = new MagicRenumberViewModel(exEvent, handler, logic);

                window = new MagicRenumber();
                window.DataContext = mrVmod;

                window.Show();
            }
        }
    }
}