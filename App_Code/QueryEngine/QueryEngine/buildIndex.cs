using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

namespace GBE.QueryEngine
{
    public class buildIndex
    {

        public buildIndex()
        {

        }
        public void build3HopIndex(int nodes, string indexTable)
        {
            List<int> firstForwardLevelNeighbor = new List<int>();
            List<int> firstBackwardLevelNeighbor = new List<int>();
            List<int> secondForwardLevelNeighbor = new List<int>();
            List<int> secondBackwardLevelNeighbor = new List<int>();
            List<int> thirdForwardLevelNeighbor = new List<int>();
            List<int> thirdBackwardLevelNeighbor = new List<int>();
            List<int> discoveredForwardIDs = new List<int>();
            List<int> discoveredBackwardIDs = new List<int>();
            int j = 0;
            int k = 0;
            //change the number of nodes in a dataset correspondingly
            int numberOfNodes = nodes;
            DataTable insertTable = new DataTable();
            insertTable.Columns.Add("ID", typeof(byte[]));
            insertTable.Columns.Add("Distance", typeof(int));
            insertTable.Columns.Add("Sigbyte", typeof(byte[]));
            insertTable.Columns.Add("Count", typeof(int));
            insertTable.Columns.Add("IDs", typeof(byte[]));
            List<List<int>> firstForwardNeighbors = new List<List<int>>();
            List<List<int>> firstBackwardNeighbors = new List<List<int>>();
            string sql = "select ID1, ID2 from idGraph";
            DataTable idGraph = DataHelpe.GetDataTable(sql);
            for (int i = 1; i < numberOfNodes + 1; i++)
            {
                firstForwardNeighbors.Add(directedNeighborhood.forwardFirstLevelNeighbor(i, idGraph));
                firstBackwardNeighbors.Add(directedNeighborhood.backwardFirstLevelNeighbor(i, idGraph));
                Console.WriteLine(i);
            }
            Console.WriteLine("OK");
            for (int i = 1; i < numberOfNodes + 1; i++)
            {
                firstForwardLevelNeighbor.AddRange(firstForwardNeighbors[i - 1]);
                if (!(firstForwardLevelNeighbor == null))
                {
                    insertTable.Merge(directedNeighborhood.insertIntoIndexing(firstForwardLevelNeighbor, i, 1));

                }
                firstBackwardLevelNeighbor.AddRange(firstBackwardNeighbors[i - 1]);
                if (!(firstBackwardLevelNeighbor == null))
                {
                    insertTable.Merge(directedNeighborhood.insertIntoIndexing(firstBackwardLevelNeighbor, i, -1));
                }
                secondForwardLevelNeighbor = directedNeighborhood.nextForwardLevelNeighbor(firstForwardLevelNeighbor, firstForwardLevelNeighbor, firstForwardNeighbors);
                if (!(secondForwardLevelNeighbor == null))
                {
                    insertTable.Merge(directedNeighborhood.insertIntoIndexing(secondForwardLevelNeighbor, i, 2));
                }
                secondBackwardLevelNeighbor = directedNeighborhood.nextBackwardLevelNeighbor(firstBackwardLevelNeighbor, firstBackwardLevelNeighbor, firstBackwardNeighbors);
                if (!(secondBackwardLevelNeighbor == null))
                {
                    insertTable.Merge(directedNeighborhood.insertIntoIndexing(secondBackwardLevelNeighbor, i, -2));
                }
                discoveredForwardIDs.AddRange(firstForwardLevelNeighbor);
                discoveredForwardIDs.AddRange(secondForwardLevelNeighbor);
                discoveredBackwardIDs.AddRange(firstBackwardLevelNeighbor);
                discoveredBackwardIDs.AddRange(secondBackwardLevelNeighbor);
                thirdForwardLevelNeighbor = directedNeighborhood.nextForwardLevelNeighbor(discoveredForwardIDs, secondForwardLevelNeighbor, firstForwardNeighbors);
                if (!(thirdForwardLevelNeighbor == null))
                {
                    insertTable.Merge(directedNeighborhood.insertIntoIndexing(thirdForwardLevelNeighbor, i, 3));
                }
                thirdBackwardLevelNeighbor = directedNeighborhood.nextBackwardLevelNeighbor(discoveredBackwardIDs, secondBackwardLevelNeighbor, firstBackwardNeighbors);
                if (!(thirdBackwardLevelNeighbor == null))
                {
                    insertTable.Merge(directedNeighborhood.insertIntoIndexing(thirdBackwardLevelNeighbor, i, -3));
                }
                j++;
                if (j > 99 | i == numberOfNodes)
                {
                    j = 0;
                    string connection = DataHelpe.Constr;
                    SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection);
                    sqlBulkCopy.DestinationTableName = indexTable;
                    sqlBulkCopy.BulkCopyTimeout = 600;
                    sqlBulkCopy.WriteToServer(insertTable);
                    sqlBulkCopy.Close();
                    insertTable.Clear();
                    Console.WriteLine(k);
                    k++;
                }
                firstForwardLevelNeighbor.Clear();
                firstBackwardLevelNeighbor.Clear();
                secondForwardLevelNeighbor.Clear();
                secondBackwardLevelNeighbor.Clear();
                thirdForwardLevelNeighbor.Clear();
                thirdBackwardLevelNeighbor.Clear();
                discoveredForwardIDs.Clear();
                discoveredBackwardIDs.Clear();
            }
        }
    }
}