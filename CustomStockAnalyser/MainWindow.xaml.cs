using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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


namespace CustomStockAnalyser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<FileInfo> stockFiles = new ObservableCollection<FileInfo>();

        public MainWindow()
        {
            InitializeComponent();
                                              
            LoadListOfFIles();
            filesDataGrid.ItemsSource = stockFiles;


            //Przykładowe użycie ChartWindow
            List<KeyValuePair<string,double>> chartData = new List<KeyValuePair<string, double>>()
            {
               new KeyValuePair<string, double>(new DateTime(2016, 07,01).ToString("yyyy-MM-dd"), 4),
               new KeyValuePair<string, double>(new DateTime(2016, 07,02).ToString("yyyy-MM-dd"), 4.2),
               new KeyValuePair<string, double>(new DateTime(2016, 07,03).ToString("yyyy-MM-dd"), 4.5)
            };

            ChartWindow chartWindow = new ChartWindow("Getin notowanie od 20.07.2015 do 13.09.2016", "Getin",  chartData);
            chartWindow.Show();

            
        }

        private void LoadListOfFIles()
        {
            string [] filePaths = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Pliki z danymi");

            for (int i = 0; i < filePaths.Length; i++ )
                stockFiles.Add(new FileInfo(filePaths[i]));

        }

        private void AddOrSelectMarketModeBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            
            if (btn.Content.ToString().Equals("Tryb dodawania"))
            {
                btn.Content = "Tryb selekcji";
                btn.ToolTip = "Przełącz na tryb selekcji.";
                MarketTextBox.Visibility = Visibility.Visible;
                MarketComboBox.Visibility = Visibility.Collapsed;
                AddOrDeleteMarketBtn.IsEnabled = true;
                AddOrDeleteMarketBtn.Content = "Dodaj";
                AddOrDeleteMarketBtn.ToolTip = "Dodaj giełdę.";

                if (MarketTextBox.Text.Length == 0)
                    AddOrDeleteMarketBtn.IsEnabled = false;
                else
                    AddOrDeleteMarketBtn.IsEnabled = true;
            }
            else if (btn.Content.ToString().Equals("Tryb selekcji"))
            {
                btn.Content = "Tryb dodawania";
                btn.ToolTip = "Przełącz na tryb dodawania nowej giełdy";
                MarketTextBox.Visibility = Visibility.Collapsed;
                MarketComboBox.Visibility = Visibility.Visible;
                AddOrDeleteMarketBtn.Content = "Usuń";
                AddOrDeleteMarketBtn.ToolTip = "Usuń giełdę.";

                if (MarketComboBox.SelectedItem != null)
                    AddOrDeleteMarketBtn.IsEnabled = true;
                else
                    AddOrDeleteMarketBtn.IsEnabled = false;
                
            }
        }

        private void AddOrDeleteMarketBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Content.ToString().Equals("Usuń"))
            {
                ComboBoxItem cbi = (ComboBoxItem) MarketComboBox.SelectedItem;
                   
                TwoStateDialogBox decisionDialog = new TwoStateDialogBox("Czy na pewno chcesz usunąć rynek o nazwie:  " + cbi.Content + " ?", "tak", "nie");
                bool result = (bool)decisionDialog.ShowDialog();
            }
            else if (btn.Content.ToString().Equals("Dodaj"))
            {
                //Sprawdź czy rynek o podanej nazwie już istnieje !!!!!!!!!!
                List<string> messageParts = new List<string>() { "Rynek o nazwie ", MarketTextBox.Text, " został dodany." };
                List<bool> isBold = new List<bool>() { false, true, false };

                OneStateDialogBox oneStateDialogBox = new OneStateDialogBox("Rynek dodany", messageParts, isBold);
                oneStateDialogBox.ShowDialog();
            }
        }

        private void MarketComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ComboBoxItem cbi = (ComboBoxItem)cb.SelectedItem;

            if (cbi != null)
                AddOrDeleteMarketBtn.IsEnabled = true;
            else
                AddOrDeleteMarketBtn.IsEnabled = false;
        }

       
        private void AddOrSelectStockModeBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Content.ToString().Equals("Tryb dodawania"))
            {
                btn.Content = "Tryb selekcji";
                btn.ToolTip = "Przełącz na tryb selekcji.";
                StockTextBox.Visibility = Visibility.Visible;
                StockComboBox.Visibility = Visibility.Collapsed;
                AddOrDeleteStockBtn.Content = "Dodaj";
                AddOrDeleteStockBtn.ToolTip = "Dodaj spółkę. (instrument)";
                AddOrDeleteStockBtn.IsEnabled = true;

                if (StockTextBox.Text.Length == 0)
                    AddOrDeleteStockBtn.IsEnabled = false;
                else
                    AddOrDeleteStockBtn.IsEnabled = true;
            }
            else if (btn.Content.ToString().Equals("Tryb selekcji"))
            {
                btn.Content = "Tryb dodawania";
                btn.ToolTip = "Przełącz na tryb dodawania nowej spółki. (instrumentu)";
                StockTextBox.Visibility = Visibility.Collapsed;
                StockComboBox.Visibility = Visibility.Visible;
                AddOrDeleteStockBtn.Content = "Usuń";
                AddOrDeleteStockBtn.ToolTip = "Usuń spółkę. (instrument)";

                if (StockComboBox.SelectedItem != null)
                    AddOrDeleteStockBtn.IsEnabled = true;
                else
                    AddOrDeleteStockBtn.IsEnabled = false;
            }
        }

        private void AddOrDeleteStockBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Content.ToString().Equals("Usuń"))
            {
                ComboBoxItem cbi = (ComboBoxItem)StockComboBox.SelectedItem;

                TwoStateDialogBox decisionDialog = new TwoStateDialogBox("Czy na pewno chcesz usunąć spółkę o nazwie:  " + cbi.Content + " ?", "tak", "nie");
                bool result = (bool)decisionDialog.ShowDialog();
            }
            else if (btn.Content.ToString().Equals("Dodaj"))
            {
                //Sprawdź czy spółka o podanej nazwie już istnieje !!!!!!!!!!
                List<string> messageParts = new List<string>() { "Spółka o nazwie ", StockTextBox.Text, " została dodana." };
                List<bool> isBold = new List<bool>() { false, true, false };

                OneStateDialogBox oneStateDialogBox = new OneStateDialogBox("Spółka dodana", messageParts, isBold);
                oneStateDialogBox.ShowDialog();
            }
        }

        private void StockComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ComboBoxItem cbi = (ComboBoxItem)cb.SelectedItem;

            if (cbi != null)
                AddOrDeleteStockBtn.IsEnabled = true;
            else
                AddOrDeleteStockBtn.IsEnabled = false;
        }

        private void MarketTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MarketTextBox.Text.Length == 0)
                AddOrDeleteMarketBtn.IsEnabled = false;
            else
                AddOrDeleteMarketBtn.IsEnabled = true;
        }

        private void StockTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StockTextBox.Text.Length == 0)
                AddOrDeleteStockBtn.IsEnabled = false;
            else
                AddOrDeleteStockBtn.IsEnabled = true;
        }

        private void DeleteFileBtn_Click(object sender, RoutedEventArgs e)
        {
            ThreeStateDialogBox tD = new ThreeStateDialogBox("Czy chcesz usunąć plik z dysku?", "Usuń z dysku", "Usuń tylko z listy", "Anuluj");
            bool? result = tD.ShowDialog();

            //Anuluj usuwanie
            if (result == null)
                return;
            
            string currentPath = Directory.GetCurrentDirectory();
            FileInfo fileInfo = (FileInfo) filesDataGrid.SelectedItem;

            //Usuń z listy
            stockFiles.Remove(fileInfo);

            //Usuń plik z dysku
            if (result == true)
            {
                if (Directory.Exists(currentPath + "\\Pliki z danymi") == false)
                   Directory.CreateDirectory(currentPath + "\\Pliki z danymi");
                                
                File.Delete(fileInfo.FullName);
            }
            //Przenieś plik do innego folderu
            else if (result == false)
            {
                if (Directory.Exists(currentPath + "\\Usunięte pliki") == false)
                    Directory.CreateDirectory(currentPath + "\\Usunięte pliki");

                File.Move(fileInfo.FullName, currentPath + "\\Usunięte pliki\\" + fileInfo.Name);
            }
            
        }

        private void ImportSheetBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter += "Arkusz xls (*.xls)|*.xls";
            openFileDialog.ShowDialog();

            string fileName = openFileDialog.FileName;

            List<FileInfo> foundFiles = (List<FileInfo>) stockFiles.Where(x => x.FullName == fileName).ToList();
           
            //Sprawdź najpierw czy dany plik już tam nie istnieje
            if (foundFiles.Count > 0)
            {
                OneStateDialogBox oD = new OneStateDialogBox("", new List<string>() { "Plik o nazwie", foundFiles[0].Name, "już jest na liście" }, new List<bool>() { false, true, false });
                oD.ShowDialog();
                return;
            }
            

            FileInfo fileInfo = new FileInfo(fileName);
            string newFullPath =  Directory.GetCurrentDirectory() + "\\Pliki z danymi\\" + fileInfo.Name;

           //skopiuj plik do właściwego folderu
            File.Copy(fileName, newFullPath);
            //fileInfo z nową ściężką FullPath
            fileInfo = new FileInfo(newFullPath);

            stockFiles.Add(fileInfo);

            //Dopasowanie szerokości kolumn datagrid (ponieważ szerokość * nie zmniejsza się przy zmianie zawartości kolumn o szerokości auto)
            filesDataGrid.Columns[0].Width = 0;
            filesDataGrid.UpdateLayout();
            filesDataGrid.Columns[0].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
           

            
        }

        

       
    }
}
