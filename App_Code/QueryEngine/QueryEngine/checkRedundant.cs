using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GBE.QueryEngine
{
    public class checkRedundant
    {
        public checkRedundant()
        {
        }
        public static List<Dictionary<int, int>> verifyRedundant(List<Dictionary<int, int>> result)
        {
            List<Dictionary<int, int>> verifiedResult = new List<Dictionary<int, int>>();
            foreach (Dictionary<int, int> candidateMatch in result)
            {
                var lookup = candidateMatch.ToLookup(x => x.Value, x => x.Key).Where(x => x.Count() > 1);
                if (lookup.Count() == 0)
                {
                    verifiedResult.Add(candidateMatch);
                }
            }
            return verifiedResult;
        }
    }
}