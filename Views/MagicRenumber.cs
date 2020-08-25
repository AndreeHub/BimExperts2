using System;
using System.Windows;

namespace BimExperts.Views
{
    /// <summary>
    /// Interaction logic for MagicRenumber.xaml
    /// </summary>
    public partial class MagicRenumber : Window, IDisposable
    {
        public MagicRenumber()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}