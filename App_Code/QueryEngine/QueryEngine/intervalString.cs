using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlTypes;
using System;

namespace GBE.QueryEngine
{
    public class intervalString
    {
        public intervalString()
        {
        }
        //find the int interval for partial matching keywords
        public static Dictionary<string, List<int>> intervalStringGeneration(Dictionary<int, string> queryIDWithKeyword, DataTable IDlist)
        {
            Dictionary<string, List<int>> intervalForString = new Dictionary<string, List<int>>();
            foreach (KeyValuePair<int, string> pair in queryIDWithKeyword)
            {
                string queryKeyword = pair.Value;
                if (!intervalForString.ContainsKey(queryKeyword))
                {

                    //use to handle special characters
                    if (queryKeyword.Contains("'"))
                    {
                        queryKeyword = queryKeyword.Replace("'", "''");
                    }
                    if (queryKeyword.Contains("%"))
                    {
                        queryKeyword = queryKeyword.Replace("%", "[%]");
                    }
                    //for VLDB use to save look up time
                    int lowerbound = 0;
                    int upperbound = 0;
                    int maxID = IDlist.Rows.Count;
                    bool lowerBoundFound = false;
                    string keyowrdWithoutStar = queryKeyword.Substring(0, queryKeyword.Length - 1);
                    foreach (DataRow i in IDlist.Rows)
                    {
                        if (!lowerBoundFound)
                        {
                            int i1 = String.CompareOrdinal((string)i["Node"], keyowrdWithoutStar);
                            if (i1 >= 0)
                            {
                                lowerBoundFound = true;
                                lowerbound = (int)i["px"];
                                keyowrdWithoutStar = keyowrdWithoutStar + "zzzzzzz";
                            }
                        }
                        if (lowerBoundFound)
                        {
                            int i2 = String.CompareOrdinal((string)i["Node"], keyowrdWithoutStar);
                            if (i2 >= 0)
                            {
                                if (upperbound != lowerbound)
                                {
                                    upperbound = (int)i["px"] - 1;
                                }
                                break;
                            }
                        }
                    }
                    List<int> interval = new List<int>();
                    interval.Add(upperbound);
                    interval.Add(lowerbound);
                    intervalForString.Add(queryKeyword, interval);

                }
            }
            return intervalForString;
        }

        public static List<Dictionary<int, int>> matchSingleLabel(int queryID, string queryKeyword, DataTable IDlist)
        {
            List<Dictionary<int, int>> singleNodeMatchResults = new List<Dictionary<int, int>>();
            int lowerbound = 0;
            int upperbound = 0;
            int maxID = IDlist.Rows.Count;
            bool lowerBoundFound = false;
            string keyowrdWithoutStar = queryKeyword.Substring(0, queryKeyword.Length - 1);
            foreach (DataRow i in IDlist.Rows)
            {
                if (!lowerBoundFound)
                {
                    int i1 = String.CompareOrdinal((string)i["Node"], keyowrdWithoutStar);
                    if (i1 >= 0)
                    {
                        lowerBoundFound = true;
                        lowerbound = (int)i["px"];
                        keyowrdWithoutStar = keyowrdWithoutStar + "zzzzzzz";
                    }
                }
                if (lowerBoundFound)
                {
                    int i1 = String.CompareOrdinal((string)i["Node"], keyowrdWithoutStar);
                    if (i1 >= 0)
                    {
                        upperbound = (int)i["px"] - 1;
                        break;
                    }
                }
            }
            for (int i = lowerbound; i < upperbound + 1; i++)
            {
                Dictionary<int, int> oneMatch = new Dictionary<int, int>();
                oneMatch.Add(queryID, i);
                singleNodeMatchResults.Add(oneMatch);
            }
            return singleNodeMatchResults;
        }
    }
}