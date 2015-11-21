using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace GBE.QueryEngine
{
    public class joinCandidateMatch
    {
        public joinCandidateMatch()
        {
        }
        // join two list of maping candidates. Each list is the mapping candidate set for one MLT tree.
        public static List<Dictionary<int, int>> joinPieces(List<Dictionary<int, int>> piece1, List<Dictionary<int, int>> piece2)
        {
            List<Dictionary<int, int>> jointPieces = new List<Dictionary<int, int>>();
            int jointNumbers = piece1.Count() * piece2.Count();
            if (jointNumbers > 10000000)
            {
                System.Console.WriteLine("Too many joins need to be proecess for this query please use more specified query keywords");
            }

            bool joinable = true;
            foreach (Dictionary<int, int> candidatePiece1 in piece1)
            {

                foreach (Dictionary<int, int> candidatePiece2 in piece2)
                {
                    foreach (KeyValuePair<int, int> pair in candidatePiece2)
                    {
                        if (candidatePiece1.ContainsKey(pair.Key))
                        {
                            if (candidatePiece1[pair.Key] != pair.Value)
                            {
                                joinable = false;
                                break;

                            }
                        }
                    }

                    if (joinable)
                    {
                        jointPieces.Add(candidatePiece1.Concat(candidatePiece2.Where(kvp => !candidatePiece1.ContainsKey(kvp.Key))).ToDictionary(x => x.Key, x => x.Value));
                    }
                    else
                    {
                        joinable = true;
                        continue;
                    }

                }
            }
            return jointPieces;
        }

        // direct join two candidate mapping together. 

        public static Dictionary<int, int> joinPieceCandidates(Dictionary<int, int> piece1, Dictionary<int, int> piece2)
        {
            bool joinable = true;
            Dictionary<int, int> jointPieceCandidate = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> pair in piece2)
            {
                if (piece1.ContainsKey(pair.Key))
                {
                    if (piece1[pair.Key] != pair.Value)
                    {
                        joinable = false;
                        break;

                    }
                }
            }
            if (joinable)
            {
                jointPieceCandidate = piece1.Concat(piece2.Where(kvp => !piece1.ContainsKey(kvp.Key))).ToDictionary(x => x.Key, x => x.Value);
            }
            return jointPieceCandidate;
        }
    }
}