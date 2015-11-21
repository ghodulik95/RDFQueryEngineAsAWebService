using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace GBE.QueryEngine
{
    public class allCandidateSetWithoutNH
    {
        public allCandidateSetWithoutNH()
        {
        }
        //generate all candidate sets in the form of query Node ID and list of IDs which can be a candidate matching for it in original graph after neighborhood checking
        public static Dictionary<int, List<int>> generateAllCandidateSetWithoutNH(Dictionary<int, string> queryIDWithKeyword, Dictionary<string, List<int>> intervalForString)
        {
            Dictionary<int, List<int>> allCandidateSets = new Dictionary<int, List<int>>();
            foreach (KeyValuePair<int, string> pair in queryIDWithKeyword)
            {
                int queryID = pair.Key;
                string queryKeyword = pair.Value;
                List<int> candidateSetforQueryID = new List<int>();
                candidateSetforQueryID = labelCandidates.generateLabelCandidates(queryID, queryKeyword, intervalForString);
                allCandidateSets.Add(queryID, candidateSetforQueryID);
            }
            return allCandidateSets;
        }
        //generate query Node ID and candidate sizes

        public static Dictionary<int, int> generateCandidateSize(Dictionary<int, List<int>> allCandidateSets)
        {
            Dictionary<int, int> allCandidateSize = new Dictionary<int, int>();
            foreach (KeyValuePair<int, List<int>> pair in allCandidateSets)
            {
                int queryID = pair.Key;
                int candidateSize = pair.Value.Count();
                allCandidateSize.Add(queryID, candidateSize);
            }
            return allCandidateSize;

        }
    }
}