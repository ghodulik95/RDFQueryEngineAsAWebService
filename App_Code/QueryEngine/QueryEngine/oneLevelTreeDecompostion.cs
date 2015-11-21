using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace GBE.QueryEngine
{
    public class oneLevelTreeDecompostion
    {
        public oneLevelTreeDecompostion()
        {
        }
        //int, Dictionary<int, List<int> is a MLT tree. The first int is the root node and second int is the tree level(from 1 ,2 ,3 forwading levels and from -1, -2 , -3 backwarding levels)
        public static Dictionary<int, Dictionary<int, List<int>>> decomposeOneLevelMLT(DataTable queryGraph, Dictionary<int, int> allCandidateSize)
        {
            Dictionary<int, Dictionary<int, List<int>>> MLTs = new Dictionary<int, Dictionary<int, List<int>>>();

            //set entry of desired decomposition
            //allCandidateSize[2] = 0;

            Dictionary<int, int> orderedCandidateSize = allCandidateSize.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (KeyValuePair<int, int> pair in orderedCandidateSize)
            {
                if (queryGraph.Rows.Count == 0)
                {
                    break;
                }
                //maxIndex is the query node ID with smallest candidate set
                int maxIndex = pair.Key;
                DataRow[] neighborhoodForward = queryGraph.Select("ID1=" + maxIndex);
                int rows = neighborhoodForward.Count();
                DataRow[] neighborhoodBackward = queryGraph.Select("ID2=" + maxIndex);
                int rows2 = neighborhoodBackward.Count();
                if (rows + rows2 == 0)
                {
                    continue;
                }
                List<int> firstLevelForwardNeighbor = new List<int>();
                if (!(rows == 0))
                {

                    for (int i = 0; i < rows; i++)
                    {
                        firstLevelForwardNeighbor.Add(int.Parse(neighborhoodForward[i][2].ToString()));
                        queryGraph.Rows.Remove(neighborhoodForward[i]);
                    }
                }

                Dictionary<int, List<int>> multiLevel = new Dictionary<int, List<int>>();
                if (firstLevelForwardNeighbor.Count != 0)
                {
                    multiLevel.Add(1, firstLevelForwardNeighbor);
                }


                DataRow[] neighborhoodBackward2 = queryGraph.Select("ID2=" + maxIndex);
                int rows2R = neighborhoodBackward2.Count();

                // begin backward level first
                List<int> firstLevelBackwardNeighbor = new List<int>();
                if (!(rows2R == 0))
                {

                    for (int i = 0; i < rows2R; i++)
                    {
                        firstLevelBackwardNeighbor.Add(int.Parse(neighborhoodBackward2[i][0].ToString()));
                        queryGraph.Rows.Remove(neighborhoodBackward2[i]);
                    }
                }
                if (firstLevelBackwardNeighbor.Count != 0)
                {
                    multiLevel.Add(-1, firstLevelBackwardNeighbor);
                }

                //first level indexing finish
                //begin the second level
                if (multiLevel.Count() != 0)
                {
                    MLTs.Add(maxIndex, multiLevel);
                }

            }
            return MLTs;
        }
    }
}