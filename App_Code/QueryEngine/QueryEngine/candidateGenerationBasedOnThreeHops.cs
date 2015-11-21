using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace GBE.QueryEngine
{
    public class candidateGenerationBasedOnThreeHops
    {
        public candidateGenerationBasedOnThreeHops()
        {
        }
        public static List<int> generateCandidate(DataTable queryGraph, int queryID, string queryKeyword, Dictionary<int, List<NBEntry>> NBindex, Dictionary<string, List<int>> intervalForString)
        {
            List<int> candidateSetforQueryID = new List<int>();
            Dictionary<string, Dictionary<int, int>> neighborhoodOfQueryID = generateNeighbor.returnNeighborhood(queryGraph, queryID);
            //string expression = "Node like '" + queryKeyword + "'";
            //DataRow[] result = IDlist.Select(expression);
            //get the keyword integer range
            List<int> interval = intervalForString[queryKeyword];
            int upperbound = interval[0];
            int lowerbound = interval[1];
            int labelMatches = upperbound - lowerbound + 1;
            if (labelMatches < 10000 && labelMatches > 1)
            {
                for (int i = lowerbound; i < upperbound + 1; i++)
                {

                    if (NHContainment.containsNeighbor(NBindex, intervalForString, i, neighborhoodOfQueryID))
                    {
                        candidateSetforQueryID.Add(i);
                    }
                }
            }
            else
            {
                for (int i = lowerbound; i < upperbound + 1; i++)
                {
                    candidateSetforQueryID.Add(i);
                }

            }
            return candidateSetforQueryID;
        }
    }
}