using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace GBE.QueryEngine
{
    public class labelCandidates
    {
        public labelCandidates()
        {
        }
        public static List<int> generateLabelCandidates(int queryID, string queryKeyword, Dictionary<string, List<int>> intervalForString)
        {
            List<int> candidateSetforQueryID = new List<int>();
            List<int> interval = intervalForString[queryKeyword];
            int upperbound = interval[0];
            int lowerbound = interval[1];
            for (int i = lowerbound; i < upperbound + 1; i++)
            {
                candidateSetforQueryID.Add(i);

            }
            return candidateSetforQueryID;
        }
    }
}