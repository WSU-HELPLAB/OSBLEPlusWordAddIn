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
    /// Interaction logic for CriterionUserControl.xaml
    /// </summary>
    public partial class CriterionUserControl : UserControl
    {
        public CriterionUserControl(Criterion criterion)
        {
            InitializeComponent();

            Header.Content = criterion.Header;

            foreach (Level level in criterion.Levels)
                AddLevel(level);
        }

        /// <summary>
        /// Insert a level radio button within the levels stack panel
        /// </summary>
        /// <param name="header">header of level</param>
        /// <param name="details">details of level</param>
        public void AddLevel(Level level)
        {
            LevelUserControl levelController = new LevelUserControl(level.Header, level.Details, Header.Content.ToString());

            //insert seperator between pairs of levels
            if(Levels.Children.Count > 0)
            {
                Separator sep = new Separator();
                sep.Height = 5;
                Levels.Children.Add(sep);
            }

            Levels.Children.Add(levelController);
        }

        private void Header_Checked(object sender, RoutedEventArgs e)
        {
            foreach(UIElement element in Levels.Children)
            {
                if(element is LevelUserControl)
                {
                    LevelUserControl level = element as LevelUserControl;

                    level.Header.IsEnabled = true;
                }
            }
        }

        private void Header_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in Levels.Children)
            {
                if (element is LevelUserControl)
                {
                    LevelUserControl level = element as LevelUserControl;

                    level.Header.IsEnabled = false;
                    level.Header.IsChecked = false;
                }
            }
        }
    }
}
