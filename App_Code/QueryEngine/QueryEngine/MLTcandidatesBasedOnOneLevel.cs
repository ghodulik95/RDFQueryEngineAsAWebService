using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GBE.QueryEngine
{
    public class MLTcandidatesBasedOnOneLevel
    {
        public MLTcandidatesBasedOnOneLevel()
        {
        }
        //generate MLT candidate for a specific MLT tree rooted at tree root
        public static List<Dictionary<int, int>> generateMLTcandidatesBasedOnOneLevel(Dictionary<int, List<int>> MLTtree, int treeRoot, Dictionary<int, List<int>> allCandidateSets, Dictionary<int, List<NBEntry>> NBindex)
        {
            List<int> candidateRoot = allCandidateSets[treeRoot];
            List<Dictionary<int, int>> MLTcandidates = new List<Dictionary<int, int>>();
            foreach (int rootID in candidateRoot)
            {
                List<Dictionary<int, int>> MLTcandidatesRootedAtrootID = new List<Dictionary<int, int>>();
                Dictionary<int, int> piece1Match = new Dictionary<int, int>();
                piece1Match.Add(treeRoot, rootID);
                List<Dictionary<int, int>> piece1 = new List<Dictionary<int, int>>();
                piece1.Add(piece1Match);
                MLTcandidatesRootedAtrootID.AddRange(piece1);
                List<NBEntry> resultIndexingNeighborhoodForID = NBindex[rootID];
                List<NBEntry> result1 = new List<NBEntry>();
                result1 = (from s in resultIndexingNeighborhoodForID
                           where s.Distance == 1
                           select s).ToList<NBEntry>();
                List<NBEntry> resultB1 = new List<NBEntry>();
                resultB1 = (from s in resultIndexingNeighborhoodForID
                            where s.Distance == -1
                            select s).ToList<NBEntry>();
                int count1 = result1.Count();
                int countB1 = resultB1.Count();
                List<int> firstLevelF = new List<int>();
                List<int> firstLevelB = new List<int>();
                if (count1 != 0)
                {
                    for (int i = 0; i < count1; i++)
                    {

                        firstLevelF.AddRange(IndexInMemory.byteIDstoInt(result1[i].Sigbyte, result1[i].IDs));
                    }
                }
                if (countB1 != 0)
                {
                    for (int i = 0; i < countB1; i++)
                    {
                        firstLevelB.AddRange(IndexInMemory.byteIDstoInt(resultB1[i].Sigbyte, resultB1[i].IDs));
                    }
                }
                foreach (KeyValuePair<int, List<int>> pair in MLTtree)
                {
                    // join forward 1 tree edge
                    if (pair.Key == 1)
                    {
                        foreach (int queryID in pair.Value)
                        {
                            List<int> candidatesForQueryID = allCandidateSets[queryID];
                            List<int> candidatesForQueryIDInMLT = candidatesForQueryID.Intersect(firstLevelF).ToList();
                            List<Dictionary<int, int>> piece2 = new List<Dictionary<int, int>>();
                            if (candidatesForQueryIDInMLT.Count() != 0)
                            {
                                foreach (int matchID in candidatesForQueryIDInMLT)
                                {
                                    Dictionary<int, int> piece2Match = new Dictionary<int, int>();
                                    piece2Match.Add(queryID, matchID);
                                    piece2.Add(piece2Match);

                                }
                            }
                            MLTcandidatesRootedAtrootID = joinCandidateMatch.joinPieces(MLTcandidatesRootedAtrootID, piece2);
                        }
                    }
                    if (pair.Key == -1)
                    {
                        foreach (int queryID in pair.Value)
                        {
                            List<int> candidatesForQueryID = allCandidateSets[queryID];
                            List<int> possibleMatchingNeighborhood = new List<int>();
                            possibleMatchingNeighborhood.AddRange(firstLevelB);
                            List<int> candidatesForQueryIDInMLT = candidatesForQueryID.Intersect(possibleMatchingNeighborhood).ToList();
                            List<Dictionary<int, int>> piece2 = new List<Dictionary<int, int>>();
                            if (candidatesForQueryIDInMLT.Count() != 0)
                            {
                                foreach (int matchID in candidatesForQueryIDInMLT)
                                {
                                    Dictionary<int, int> piece2Match = new Dictionary<int, int>();
                                    piece2Match.Add(queryID, matchID);
                                    piece2.Add(piece2Match);
                                }
                            }
                            MLTcandidatesRootedAtrootID = joinCandidateMatch.joinPieces(MLTcandidatesRootedAtrootID, piece2);
                        }
                    }
                }
                MLTcandidates.AddRange(MLTcandidatesRootedAtrootID);
            }
            return MLTcandidates;
        }
    }
}