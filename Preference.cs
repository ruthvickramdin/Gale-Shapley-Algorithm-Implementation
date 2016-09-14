using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaleShapleyAlgo
{
    public static class Preference
    {
        #region Properties

        public static Random random { get; set; }
        public static Dictionary<Guid, string> dict { get; set; } 

        #endregion

        #region Methods

        /// <summary>
        /// Gets the input list from the XML file and creates the preference list and returns back the preference list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<string> TakeInput(List<string> list)
        {
            list.Reverse();
            dict = new Dictionary<Guid, string>();
            random = new Random();

            for (int i = 0; i < list.Count; i++)
            {
                dict.Add(Guid.NewGuid(), list.ToArray()[i]);
            }

            var lst = from element in dict
                      orderby Guid.NewGuid().ToString()
                      select element;

            List<string> result = new List<string>();

            lst.ToList().ForEach(x => result.Add(x.Value));

            return result;
        } 

        #endregion
    }
}
