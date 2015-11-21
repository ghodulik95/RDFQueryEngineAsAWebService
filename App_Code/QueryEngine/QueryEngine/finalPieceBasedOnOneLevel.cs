using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GBE.QueryEngine
{
    public class finalPieceBasedOnOneLevel
    {
        public finalPieceBasedOnOneLevel()
        {
        }
        //generate final results by joining all MLTs
        public static List<Dictionary<int, int>> generateFinalPieceBasedOnOneLevel(Dictionary<int, Dictionary<int, List<int>>> allMLTs, Dictionary<int, List<int>> allCandidateSets, Dictionary<int, List<NBEntry>> NBindex)
        {
            bool firstTree = true;
            bool isFoundArootNode = true;
            List<Dictionary<int, int>> finalPiece = new List<Dictionary<int, int>>();
            Dictionary<int, List<Dictionary<int, int>>> treeCandidates = new Dictionary<int, List<Dictionary<int, int>>>();
            Dictionary<int, int> treeCandidateSize = new Dictionary<int, int>();
            foreach (KeyValuePair<int, Dictionary<int, List<int>>> pair in allMLTs)
            {
                List<Dictionary<int, int>> MLTcandidate = new List<Dictionary<int, int>>();
                MLTcandidate = MLTcandidatesBasedOnOneLevel.generateMLTcandidatesBasedOnOneLevel(pair.Value, pair.Key, allCandidateSets, NBindex);
                treeCandidates.Add(pair.Key, MLTcandidate);
                treeCandidateSize.Add(pair.Key, MLTcandidate.Count());
            }
            List<int> treeRootOrderByCandidateSize = new List<int>();
            foreach (KeyValuePair<int, int> item in treeCandidateSize.OrderBy(key => key.Value))
            {
                treeRootOrderByCandidateSize.Add(item.Key);
            }
            while (treeRootOrderByCandidateSize.Count() != 0)
            {
                int minimalValueKey = treeRootOrderByCandidateSize[0];
                if (firstTree)
                {
                    finalPiece.AddRange(treeCandidates[minimalValueKey]);
                    firstTree = false;
                    treeRootOrderByCandidateSize.Remove(minimalValueKey);
                }
                else
                {
                    if (finalPiece[0].ContainsKey(minimalValueKey))
                    {
                        List<Dictionary<int, int>> jointPieces = new List<Dictionary<int, int>>();
                        jointPieces = joinCandidateMatch.joinPieces(treeCandidates[minimalValueKey], finalPiece);
                        finalPiece.Clear();
                        finalPiece.AddRange(jointPieces);
                        treeRootOrderByCandidateSize.Remove(minimalValueKey);
                    }
                    else
                    {
                        isFoundArootNode = false;
                        for (int i = 1; i < treeRootOrderByCandidateSize.Count(); i++)
                        {
                            minimalValueKey = treeRootOrderByCandidateSize[i];
                            if (finalPiece[0].ContainsKey(minimalValueKey))
                            {
                                List<Dictionary<int, int>> jointPieces = new List<Dictionary<int, int>>();
                                jointPieces = joinCandidateMatch.joinPieces(treeCandidates[minimalValueKey], finalPiece);
                                finalPiece.Clear();
                                finalPiece.AddRange(jointPieces);
                                treeRootOrderByCandidateSize.Remove(minimalValueKey);
                                isFoundArootNode = true;
                                break;
                            }
                        }
                    }
                    if (isFoundArootNode == false)
                    {
                        minimalValueKey = treeRootOrderByCandidateSize[0];
                        List<Dictionary<int, int>> jointPieces = new List<Dictionary<int, int>>();
                        jointPieces = joinCandidateMatch.joinPieces(treeCandidates[minimalValueKey], finalPiece);
                        finalPiece.Clear();
                        finalPiece.AddRange(jointPieces);
                        treeRootOrderByCandidateSize.Remove(minimalValueKey);
                        isFoundArootNode = true;
                    }
                }
            }
            return finalPiece;
        }
    }
}