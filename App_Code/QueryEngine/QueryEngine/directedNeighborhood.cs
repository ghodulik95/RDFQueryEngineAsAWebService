using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

namespace GBE.QueryEngine
{
    public class directedNeighborhood
    {

        public directedNeighborhood()
        {

        }
        public static DataTable buildIDMAP()
        {
            string sql = "";
            DataTable insertTable = new DataTable();
            insertTable.Columns.Add("Node", typeof(string));
            insertTable.Columns.Add("px", typeof(byte[]));
            sql = "select Node from NodeDegree2 order by Node collate Latin1_General_BIN";
            DataTable Nodes = DataHelpe.GetDataTable(sql);
            int i = 1;
            foreach (DataRow t in Nodes.Rows)
            {
                byte[] px = BitConverter.GetBytes(i);
                Array.Reverse(px, 0, px.Length);
                string node = (string)t["Node"];
                insertTable.Rows.Add(node, px);
                i++;
            }
            return insertTable;
        }
        public static List<int> forwardFirstLevelNeighbor(int ID, DataTable idGraph)
        {
            string filterExpression = "ID1=" + ID;
            DataRow[] result = idGraph.Select(filterExpression);
            int rows = result.Count();
            DataTable forwardNode = new DataTable();
            List<int> firstLevelNeighbor = new List<int>();
            if (!(rows == 0))
            {
                forwardNode = result.CopyToDataTable();
            }
            if (!(rows == 0))
            {

                for (int i = 0; i < rows; i++)
                {
                    firstLevelNeighbor.Add(int.Parse(forwardNode.Rows[i]["ID2"].ToString()));
                }
            }

            firstLevelNeighbor.Sort();
            return firstLevelNeighbor;
        }
        public static List<int> backwardFirstLevelNeighbor(int ID, DataTable idGraph)
        {
            string filterExpression = "ID2=" + ID;
            DataRow[] result = idGraph.Select(filterExpression);
            int rows = result.Count();
            DataTable backwardNode = new DataTable();
            if (!(rows == 0))
            {
                backwardNode = result.CopyToDataTable();
            }
            List<int> firstLevelNeighbor = new List<int>();
            if (!(rows == 0))
            {
                for (int i = 0; i < rows; i++)
                {
                    firstLevelNeighbor.Add(int.Parse(backwardNode.Rows[i]["ID1"].ToString()));
                }
            }

            firstLevelNeighbor.Sort();
            return firstLevelNeighbor;
        }
        // gets the next hop forward neighborhood nodes IDs
        public static List<int> nextForwardLevelNeighbor(List<int> discoveredIDs, List<int> currentLevelIDs, List<List<int>> firstForwardNeighbors)
        {
            List<int> nextLevelNeighbor = new List<int>();
            HashSet<int> nextLevelNeighborID = new HashSet<int>();
            HashSet<int> discoveredIDsHash = new HashSet<int>(discoveredIDs);
            for (int i = 0; i < currentLevelIDs.Count; i++)
            {
                List<int> neighbors = firstForwardNeighbors[currentLevelIDs[i] - 1];
                foreach (int prime in neighbors)
                {
                    nextLevelNeighborID.Add(prime);
                }
            }
            nextLevelNeighborID.ExceptWith(discoveredIDsHash);
            nextLevelNeighbor = nextLevelNeighborID.ToList();
            nextLevelNeighbor.Sort();
            return nextLevelNeighbor;
        }
        // gets the next hop backward neighborhood nodes IDs
        public static List<int> nextBackwardLevelNeighbor(List<int> discoveredIDs, List<int> currentLevelIDs, List<List<int>> firstBackwardNeighbors)
        {
            List<int> nextLevelNeighbor = new List<int>();
            HashSet<int> nextLevelNeighborID = new HashSet<int>();
            HashSet<int> discoveredIDsHash = new HashSet<int>(discoveredIDs);
            for (int i = 0; i < currentLevelIDs.Count; i++)
            {
                List<int> neighbors = firstBackwardNeighbors[currentLevelIDs[i] - 1];
                foreach (int prime in neighbors)
                {
                    nextLevelNeighborID.Add(prime);
                }
            }
            nextLevelNeighborID.ExceptWith(discoveredIDsHash);
            nextLevelNeighbor = nextLevelNeighborID.ToList();
            nextLevelNeighbor.Sort();
            return nextLevelNeighbor;
        }
        // form the transaction of SQL insertion commands for each level neighborhood indexing
        public static DataTable insertIntoIndexing(List<int> neighbors, int nodeID, int distance)
        {
            DataTable insertTable = new DataTable();
            insertTable.Columns.Add("ID", typeof(byte[]));
            insertTable.Columns.Add("Distance", typeof(int));
            insertTable.Columns.Add("Sigbyte", typeof(byte[]));
            insertTable.Columns.Add("Count", typeof(int));
            insertTable.Columns.Add("IDs", typeof(byte[]));
            byte[] id = new byte[4];
            id = BitConverter.GetBytes(nodeID);
            Array.Reverse(id, 0, id.Length);
            if (neighbors.Count == 0)
            {
                return insertTable;
            }

            byte[] firstNeighborID = BitConverter.GetBytes(neighbors[0]);
            byte sigbit = firstNeighborID[2];
            List<byte> entry = new List<byte>();
            //ignore the update 8 bits
            //entry.Add(neighbors[0][0]);
            entry.Add(firstNeighborID[1]);
            entry.Add(firstNeighborID[0]);
            int count = 1;
            bool lastOneDone = false;
            for (int i = 1; i < neighbors.Count; i++)
            {
                byte[] neighborID = new byte[4];
                neighborID = BitConverter.GetBytes(neighbors[i]);
                while (neighborID[2] == sigbit)
                {
                    count++;
                    entry.Add(neighborID[1]);
                    entry.Add(neighborID[0]);
                    i++;
                    if (i == neighbors.Count)
                    {
                        lastOneDone = true;
                        break;
                    }
                    neighborID = BitConverter.GetBytes(neighbors[i]);
                }
                byte[] sigbits = new byte[1];
                sigbits[0] = sigbit;
                insertTable.Rows.Add(id, distance, sigbits, count, entry.ToArray());
                count = 1;
                sigbit = neighborID[2];
                entry.Clear();
                entry.Add(neighborID[1]);
                entry.Add(neighborID[0]);
            }
            if (!lastOneDone)
            {
                byte[] sigbits2 = new byte[1];
                sigbits2[0] = sigbit;
                insertTable.Rows.Add(id, distance, sigbits2, count, entry.ToArray());
            }
            return insertTable;
        }
    }
}