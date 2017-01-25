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
    /// Interaction logic for OneStateDialogBox.xaml
    /// </summary>
    public partial class OneStateDialogBox : Window
    {
        public OneStateDialogBox(string title, List<string> messageParts, List<bool> isBold, string buttonText = "Ok")
        {
            InitializeComponent();

            this.Title = title;
            onlyBtn.Content = buttonText;

            //DialogTextBlock.Text = dialogText;
            for(int i = 0; i < messageParts.Count; i++)
            {
                if (isBold[i] == true)
                    DialogTextBlock.Inlines.Add(new Run(" " + messageParts[i]) { FontWeight = FontWeights.Bold });
                else
                    DialogTextBlock.Inlines.Add(new Run(" " + messageParts[i]));
            }


            
        }

        private void DecisionBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
