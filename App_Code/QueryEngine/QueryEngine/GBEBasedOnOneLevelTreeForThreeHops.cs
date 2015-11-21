using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GBE.QueryEngine
{
    public class GBEBasedOnOneLevelTreeForThreeHops
    {
        public GBEBasedOnOneLevelTreeForThreeHops()
        {
        }
        //return matching results based on one level tree
        public static List<Dictionary<int, int>> componentMatchingResult(DataTable queryGraph, Dictionary<int, string> queryIDWithKeyword, DataTable IDlist, Dictionary<int, List<NBEntry>> NBindex, Dictionary<string, List<int>> intervalForString)
        {
            //generate all candidate sets based on neighborhood check
            Dictionary<int, List<int>> allCandidateSet = new Dictionary<int, List<int>>();
            allCandidateSet = allCandidateSetsBasedOnThreeHops.generateAllCandidate(queryGraph, queryIDWithKeyword, NBindex, intervalForString);
            Dictionary<int, int> allCandidateSize = allCandidateSetsBasedOnThreeHops.generateCandidateSize(allCandidateSet);
            //second idea is to decompose to one level tree
            Dictionary<int, Dictionary<int, List<int>>> MLTs = oneLevelTreeDecompostion.decomposeOneLevelMLT(queryGraph, allCandidateSize);
            List<Dictionary<int, int>> finalPiece = finalPieceBasedOnOneLevel.generateFinalPieceBasedOnOneLevel(MLTs, allCandidateSet, NBindex);
            //check redundant result
            List<Dictionary<int, int>> finalMatch = checkRedundant.verifyRedundant(finalPiece);
            return finalMatch;
        }

        public static TimeSpan componentMatchingTime(DataTable queryGraph, Dictionary<int, string> queryIDWithKeyword, DataTable IDlist, Dictionary<int, List<NBEntry>> NBindex, Dictionary<string, List<int>> intervalForString)
        {
            //generate all candidate sets based on neighborhood check
            DateTime time1 = DateTime.Now;
            Dictionary<int, List<int>> allCandidateSet = new Dictionary<int, List<int>>();
            allCandidateSet = allCandidateSetsBasedOnThreeHops.generateAllCandidate(queryGraph, queryIDWithKeyword, NBindex, intervalForString);
            Dictionary<int, int> allCandidateSize = allCandidateSetsBasedOnThreeHops.generateCandidateSize(allCandidateSet);
            //second idea is to decompose to one level tree
            Dictionary<int, Dictionary<int, List<int>>> MLTs = oneLevelTreeDecompostion.decomposeOneLevelMLT(queryGraph, allCandidateSize);
            List<Dictionary<int, int>> finalPiece = finalPieceBasedOnOneLevel.generateFinalPieceBasedOnOneLevel(MLTs, allCandidateSet, NBindex);
            //check redundant result
            List<Dictionary<int, int>> finalMatch = checkRedundant.verifyRedundant(finalPiece);
            DateTime time2 = DateTime.Now;
            TimeSpan diff = time2 - time1;
            return diff;
        }
    }
}