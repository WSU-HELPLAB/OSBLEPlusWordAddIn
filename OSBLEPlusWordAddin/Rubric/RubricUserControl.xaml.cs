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
    /// Interaction logic for RubricUserControl.xaml
    /// </summary>
    public partial class RubricUserControl : UserControl
    {
        public RubricUserControl()
        {
            InitializeComponent();

            List<Level> levels = new List<Level>();
            levels.Add(new Rubric.Level("F-D", "Bad"));
            levels.Add(new Rubric.Level("C-B", "Good"));
            levels.Add(new Rubric.Level("A", "Excellent"));

            List<Criterion> criterions = new List<Criterion>();
            criterions.Add(new Criterion("Login", levels));
            criterions.Add(new Criterion("Database", levels));
            criterions.Add(new Criterion("Submission", levels));
            criterions.Add(new Criterion("Data", levels));
            criterions.Add(new Criterion("Rubric", levels));

            foreach (Criterion criterion in criterions)
                AddCriterion(criterion);
        }

        public void AddCriterion(Criterion criterion)
        {
            CriterionUserControl criterionController = new CriterionUserControl(criterion);

            Criterions.Children.Add(criterionController);
        }
    }
}
