using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OSBLEPlusWordAddin.Rubric
{
    /// <summary>
    /// Interaction logic for LevelUserControl.xaml
    /// </summary>
    public partial class LevelUserControl : UserControl
    {
        public LevelUserControl(string header, string details, string groupname)
        {
            InitializeComponent();

            Header.Content = header;
            Details.Text = details;

            Header.GroupName = groupname;
        }
    }
}
