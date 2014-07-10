using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PCRSever.ViewModel;

namespace PCRSever.View
{
    /// <summary>
    /// CameraView.xaml 的交互逻辑
    /// </summary>
    public partial class CameraView : Window
    {
        private CameraViewModel model;
        public CameraView()
        {
            InitializeComponent();
            model = new CameraViewModel();
            this.DataContext = model;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            model.Close();
            base.OnClosing(e);
        }
    }
}
