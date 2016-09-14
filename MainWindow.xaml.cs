using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Linq;

namespace GaleShapleyAlgo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        public static string emptyErrorMsg { get; set; }
        public static string greaterValueErrorMsg { get; set; }
        public static string numberOfPairsAreZero { get; set; }
        public static string stars { get; set; }
        public static int nOP { get; set; }
        public static int nOT { get; set; }

        //Create the input matrix
        public string[,] inputMenArray { get; set; }
        public string[,] inputWomenArray { get; set; }

        //Create the mens & womens lists
        public List<string> menNames { get; set; }
        public List<string> womenNames { get; set; }

        public Dictionary<string, List<string>> mensPreference { get; set; }
        public Dictionary<string, List<string>> womensPreference { get; set; }

        public static string filePath { get; set; }
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
        }

        static MainWindow()
        {
            emptyErrorMsg = "* Please enter the number of pairs....!!";
            greaterValueErrorMsg = "* Please enter the number of pairs less than 100....!!";
            numberOfPairsAreZero = "* Please enter number of pairs greater than 0";
            stars = "*********************************************************************";
            nOP = 0;
            nOT = 0;
        }
        #endregion

        #region Events
        /// <summary>
        /// Event to browse the input file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            InitializeControls();
            GetInputLists();

        }

        /// <summary>
        /// Event to check/uncheck the generate input option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBoxGenerateInput_Click(object sender, RoutedEventArgs e)
        {
            if (chkBoxGenerateInput.IsChecked.Value)
            {
                ChangeControl(true);
                chkBoxInputFile.IsChecked = false;
                chkBoxInputFile.IsEnabled = false;
            }
            else
            {
                ChangeControl(false);
                chkBoxInputFile.IsEnabled = true;
                chkBoxGenerateInput.IsEnabled = true;
                txtNumberOfPairs.Text = string.Empty;
            }
            lblErrorMsg.Content = string.Empty;
            txtBoxMensList.Text = string.Empty;
            txtBoxWomensList.Text = string.Empty;
        }

        /// <summary>
        /// Event to check/uncheck the input file option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBoxInputFile_Click(object sender, RoutedEventArgs e)
        {
            if (chkBoxInputFile.IsChecked.Value)
            {
                btnBrowse.IsEnabled = true;
                chkBoxGenerateInput.IsChecked = false;
                chkBoxGenerateInput.IsEnabled = false;
            }
            else
            {
                btnBrowse.IsEnabled = false;
                chkBoxGenerateInput.IsEnabled = true;
            }
            lblErrorMsg.Content = string.Empty;
            txtBoxMensList.Text = string.Empty;
            txtBoxWomensList.Text = string.Empty;
        }

        /// <summary>
        /// Textbox event to check the textbox entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNumberOfPairs_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNumberOfPairs.Text))
            {
                    CheckForNumberOfPairs();
            }
        }

        /// <summary>
        /// Button to generate the preference list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGeneratePreferenceList_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtNumberOfPairs.Text) != 0)
            {
                if (nOP > 0)
                {
                    InitializeControls();

                    if (chkBoxGenerateInput.IsChecked.Value)
                    {
                        System.Diagnostics.Stopwatch stp = new System.Diagnostics.Stopwatch();
                        stp.Start();
                        CreatePreferenceList();
                        DisplayPreferenceList();

                        DisplayStableMatch(Match.CalculateStableMatch(nOP, nOT, mensPreference, womensPreference));

                        stp.Stop();

                        lblErrorMsg.Visibility = Visibility.Visible;
                        lblErrorMsg.Content = string.Format("{0}{1}", "Execution Time (Miliseconds): ", stp.Elapsed.Milliseconds);
                    }
                }
                else
                {
                    lblErrorMsg.Content = numberOfPairsAreZero;
                    lblErrorMsg.Visibility = Visibility.Visible;
                    btnGeneratePreferenceList.IsEnabled = false;
                }
            }
            else
            {
                lblErrorMsg.Content = numberOfPairsAreZero;
                lblErrorMsg.Visibility = Visibility.Visible;
                btnGeneratePreferenceList.IsEnabled = false;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes all the controls on the form & the variables used
        /// </summary>
        private void InitializeControls()
        {
            inputMenArray = null;
            inputWomenArray = null;

            menNames = null;
            womenNames = null;

            lblErrorMsg.Visibility = Visibility.Hidden;
            lblMenPrefTitle.Visibility = Visibility.Hidden;
            lblWomenPrefTitle.Visibility = Visibility.Hidden;

            txtBoxMensList.Text = string.Empty;
            txtBoxMensList.Visibility = Visibility.Hidden;
            txtBoxWomensList.Text = string.Empty;
            txtBoxWomensList.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Reads the input file given as an input and parses the file to generate the preference list which can be given an an input
        /// </summary>
        private void GetInputLists()
        {
            //Show the Open File Dialog Box
            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.Title = "Open File";
            ofd.ShowDialog();

            #region Read data from input file
            if (File.Exists(ofd.FileName))
            {
                System.Diagnostics.Stopwatch stp = new System.Diagnostics.Stopwatch();
                stp.Start();
                string[] lines = System.IO.File.ReadAllLines(ofd.FileName);

                if (lines.Any())
                {
                    string[] chars = { " ", "", ":", "," };

                    menNames = new List<string>();
                    womenNames = new List<string>();
                    mensPreference = new Dictionary<string, List<string>>();
                    womensPreference = new Dictionary<string, List<string>>();

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] res = lines[i].Split(chars, StringSplitOptions.RemoveEmptyEntries);

                        //fill the lists
                        if (res.Any())
                        {
                            if (i == 0)
                            {
                                menNames.AddRange(res.ToList());
                                menNames.ForEach(x => mensPreference.Add(x, new List<string>()));
                            }
                            if (i == 1)
                            {
                                womenNames.AddRange(res.ToList());
                                womenNames.ForEach(x => womensPreference.Add(x, new List<string>()));
                            }
                            if (mensPreference.Keys.Contains(res.First()))
                            {
                                mensPreference[res.First()] = new List<string>();
                                mensPreference[res.First()] = res.Where(x => x != res.First()).ToList();
                            }
                            if (womensPreference.Keys.Contains(res.First()))
                            {
                                womensPreference[res.First()] = new List<string>();
                                womensPreference[res.First()] = res.Where(x => x != res.First()).ToList();
                            }
                        }
                    }
                }

                //Check if the dictionary has items
                if (mensPreference.Any() && womensPreference.Any())
                {
                    nOP = mensPreference.Count;
                    nOT = 1;

                    DisplayPreferenceList();
                }

                DisplayStableMatch(Match.CalculateStableMatch(nOP, nOT, mensPreference, womensPreference));

                stp.Stop();
 
                lblErrorMsg.Visibility = Visibility.Visible;
                lblErrorMsg.Content = string.Format("{0}{1}", "Execution Time (Miliseconds): ", stp.Elapsed.Milliseconds);
            } 
            #endregion
        }

        /// <summary>
        /// Check for number of pairs given as an input are valid or invalid
        /// </summary>
        private void CheckForNumberOfPairs()
        {
            if (string.IsNullOrEmpty(txtNumberOfPairs.Text))
            {
                lblErrorMsg.Visibility = Visibility.Hidden;
                btnGeneratePreferenceList.IsEnabled = false;
                InitializeControls();
            }
            else if (string.IsNullOrWhiteSpace(txtNumberOfPairs.Text))
            {
                lblErrorMsg.Content = emptyErrorMsg;
                lblErrorMsg.Visibility = Visibility.Visible;
                btnGeneratePreferenceList.IsEnabled = false;
            }
            else if (Convert.ToInt32(txtNumberOfPairs.Text) <= 100)
            {
                lblErrorMsg.Visibility = Visibility.Hidden;
                btnGeneratePreferenceList.IsEnabled = true;
                nOP = Convert.ToInt32(txtNumberOfPairs.Text);
            }
            else
            {
                lblErrorMsg.Content = greaterValueErrorMsg;
                lblErrorMsg.Visibility = Visibility.Visible;
                btnGeneratePreferenceList.IsEnabled = false;
            }
        }

        /// <summary>
        /// Change the textbox and lable state
        /// </summary>
        /// <param name="value"></param>
        private void ChangeControl(bool value)
        {
            lblNumOfPairs.IsEnabled = value;
            txtNumberOfPairs.IsEnabled = value;
        }

        /// <summary>
        /// Laod the XML file which has names for Men & Women
        /// Generate the preference lists for both Men & Women 
        /// </summary>
        public void CreatePreferenceList()
        {
            #region Load XML and get data

            //loads the xml document
            var doc = XDocument.Load("MenWomen.xml");

            inputMenArray = new String[nOP, nOP];
            inputWomenArray = new String[nOP, nOP];

            menNames = new List<string>();
            womenNames = new List<string>();

            //Get the men names from XML Document
            (doc.Root.Elements().First().Elements().Take(nOP).ToList()).ForEach(x => { menNames.Add(x.Value); });

            //Get the women names from XML Document
            (doc.Root.Elements().Last().Elements().Take(nOP).ToList()).ForEach(x => { womenNames.Add(x.Value); });

            #endregion

            #region Create preference lists for Men and Women

            List<string> tempMenList = new List<string>(menNames);
            List<string> tempWomenList = new List<string>(womenNames);

            mensPreference = new Dictionary<string, List<string>>();
            womensPreference = new Dictionary<string, List<string>>();

            for (int j = 0; j < nOP; j++)
            {
                tempWomenList = Preference.TakeInput(tempWomenList);
                mensPreference.Add(menNames.ToArray()[j], tempWomenList);

                tempMenList = Preference.TakeInput(tempMenList);
                womensPreference.Add(womenNames.ToArray()[j], tempMenList);
            }
            #endregion
        }

        /// <summary>
        /// Displays the preferece lists for both Men & Women
        /// </summary>
        private void DisplayPreferenceList()
        {
            #region Print the Preference List Matrix

            lblMenPrefTitle.Visibility = Visibility.Visible;
            txtBoxMensList.Visibility = Visibility.Visible;
            foreach (var item in mensPreference)
            {
                txtBoxMensList.AppendText(string.Format("{0}{1}", item.Key + "\t", ":"));
                for (int i = 0; i < nOP; i++)
                {
                    txtBoxMensList.AppendText(string.Format("{0}{1}", "\t", item.Value.ToArray()[i]));
                }
                txtBoxMensList.AppendText(string.Format("{0}", Environment.NewLine));
            }

            lblWomenPrefTitle.Visibility = Visibility.Visible;
            txtBoxWomensList.Visibility = Visibility.Visible;
            foreach (var item in womensPreference)
            {
                txtBoxWomensList.AppendText(string.Format("{0}{1}", item.Key + "\t", ":"));
                for (int i = 0; i < nOP; i++)
                {
                    txtBoxWomensList.AppendText(string.Format("{0}{1}", "\t", item.Value.ToArray()[i]));
                }
                txtBoxWomensList.AppendText(string.Format("{0}", Environment.NewLine));
            }
            #endregion
        }

        /// <summary>
        /// Passes the stable match result to the Result window where result is displayed
        /// </summary>
        /// <param name="matchResult"></param>
        private void DisplayStableMatch(Dictionary<string, string> matchResult)
        {
            if (matchResult.Any())
            {
                Result res = new Result(matchResult);
                res.Show();
            }
        }
        #endregion
    }
}
