using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaleShapleyAlgo
{
    class Match
    {
        #region Properties

        public static int count { get; set; }
        public static int menEngaged { get; set; }

        public static string[] mensArray { get; set; }
        public static string[] womensArray { get; set; }

        public static string[] husband { get; set; }
        public static string[] wife { get; set; }

        public static Dictionary<string, List<string>> manPref { get; set; }
        public static Dictionary<string, List<string>> womanPref { get; set; }
        public static Dictionary<string, bool> freeMan { get; set; }

        public static List<Dictionary<string, string>> outputResult { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Initialize all the local variables used to calculate stable match
        /// </summary>
        /// <param name="n"></param>
        /// <param name="mensPreference"></param>
        /// <param name="womensPreference"></param>
        private static void Initialize(int n, Dictionary<string, List<string>> mensPreference, Dictionary<string, List<string>> womensPreference)
        {
            count = n;
            menEngaged = 0;

            mensArray = mensPreference.Keys.ToArray();
            womensArray = womensPreference.Keys.ToArray();

            husband = new string[count];
            wife = new string[count];

            manPref = new Dictionary<string, List<string>>(mensPreference);
            womanPref = new Dictionary<string, List<string>>(womensPreference);
            freeMan = new Dictionary<string, bool>();

            manPref.Keys.ToList().ForEach(x => freeMan.Add(x, false));
        }

        /// <summary>
        /// Function to actual evaluate the stable match
        /// </summary>
        /// <param name="n"></param>
        /// <param name="nOT"></param>
        /// <param name="menPreference"></param>
        /// <param name="womenPreference"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CalculateStableMatch(int n, int nOT, Dictionary<string, List<string>> menPreference, Dictionary<string, List<string>> womenPreference)
        {
                #region Assignments
                Initialize(n, menPreference, womenPreference);
                #endregion

                #region Evaluation
                while (menEngaged < count)
                {
                    int freeManIndex = -1;

                    foreach (var item in freeMan)
                    {
                        ++freeManIndex;
                        if (!item.Value)
                            break;
                    }

                    for (int i = 0; i < count && !freeMan[mensArray[freeManIndex]]; i++)
                    {
                        int index = Array.IndexOf(womensArray, (manPref[mensArray[freeManIndex]].ToArray())[i]);

                        if (wife[index] == null)
                        {
                            wife[index] = mensArray[freeManIndex];
                            husband[freeManIndex] = womensArray[index];
                            freeMan[mensArray[freeManIndex]] = true;
                            menEngaged++;
                        }
                        else
                        {
                            string currentPartner = wife[index];

                            if (CheckCurrentPartnerPreference(womensArray[index], mensArray[freeManIndex], currentPartner))
                            {
                                wife[index] = mensArray[freeManIndex];
                                husband[freeManIndex] = womensArray[index];
                                freeMan[mensArray[freeManIndex]] = true;
                                freeMan[currentPartner] = false;
                            }
                        }
                    }
                }
                #endregion

                #region Generate Output Dictionary
                Dictionary<string, string> match = new Dictionary<string, string>();

                for (int i = 0; i < n; i++)
                {
                    match.Add(mensArray[i], husband[i]);
                }

                return match;
                #endregion 
        }

        /// <summary>
        /// Function to check women's current partner preference over the new proporsing pratner
        /// </summary>
        /// <param name="women"></param>
        /// <param name="newPartner"></param>
        /// <param name="presentPartner"></param>
        /// <returns></returns>
        private static bool CheckCurrentPartnerPreference(string women, string newPartner, string presentPartner)
        {
            for (int i = 0; i < count; i++)
            {
                if (womanPref[women].ToArray()[i].Equals(newPartner))
                    return true;

                if (womanPref[women].ToArray()[i].Equals(presentPartner))
                    return false;
            }
            return false;
        } 
        
        #endregion
    }
}
