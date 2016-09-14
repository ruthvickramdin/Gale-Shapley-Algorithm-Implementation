using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace GaleShapleyAlgo
{
    /// <summary>
    /// Interaction logic for Result.xaml
    /// </summary>
    public partial class Result : Window
    {
        public static Dictionary<string, string> result { get; set; }
        public static string stars { get; set; }

        public Result(Dictionary<string, string> res)
        {
            InitializeComponent();
            result = res;
            stars = "*********************************************************";
            PrintResult();
        }

        /// <summary>
        /// Function to print the result on the Result window
        /// </summary>
        private void PrintResult()
        {
            foreach (var item in result)
            {
                txtMenStableMatch.AppendText(string.Format("{0}{1}{2}{3}", item.Key, "\t->\t", item.Value, Environment.NewLine));
            }
            txtMenStableMatch.AppendText(string.Format("{0}{1}{2}", Environment.NewLine, stars, Environment.NewLine));
        }

        /// <summary>
        /// Button to save the output satble match result to a Text file on the local system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveAsTxt_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.Title = "Save the Stable Match result";
            sfd.ShowDialog();

            string text = txtMenStableMatch.Text;

            if (!string.IsNullOrEmpty(sfd.FileName))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(sfd.FileName))
                {
                    file.Write(text);
                }

                MessageBox.Show("File saved successfully..", "Success", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }
    }
}
