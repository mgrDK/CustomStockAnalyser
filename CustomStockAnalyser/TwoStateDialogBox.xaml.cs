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
using System.Windows.Shapes;

namespace CustomStockAnalyser
{
    /// <summary>
    /// Interaction logic for TwoStateDialogBox.xaml
    /// </summary>
    public partial class TwoStateDialogBox : Window
    {
        public TwoStateDialogBox(string question, string btn1, string btn2)
        {
            InitializeComponent();

            QuestionTextBlock.Text = question;
            FirstBtn.Content = btn1;
            SecondBtn.Content = btn2;
        }

        private void DecisionBtn_Click(object sender, EventArgs eArgs)
        {
            Button btn = (Button)sender;
            
            if (btn.Name.Equals("FirstBtn"))
                this.DialogResult = true;
            else
                this.DialogResult = false;

            this.Close();
        }
    }
}
