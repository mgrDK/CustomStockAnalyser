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
    /// Interaction logic for ThreeStateDialogBox.xaml
    /// </summary>
    public partial class ThreeStateDialogBox : Window
    {
        public ThreeStateDialogBox(string question, string btn1, string btn2, string btn3)
        {
            InitializeComponent();

            QuestionTextBlock.Text = question;
            FirstBtn.Content = btn1;
            SecondBtn.Content = btn2;
            ThirdButton.Content = btn3;
        }

        private void DecisionBtn_Click(object sender, EventArgs eArgs)
        {
            Button btn = (Button)sender;

            if (btn.Name.Equals("FirstBtn"))
                this.DialogResult = true;
            else if (btn.Name.Equals("SecondBtn"))
                this.DialogResult = false;
            else
                this.DialogResult = null;

            this.Close();
        }
    }
}
