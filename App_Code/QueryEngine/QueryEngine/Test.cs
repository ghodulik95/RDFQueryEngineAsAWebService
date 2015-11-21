using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using GBE.QueryEngine;

namespace GBENB
{
    class Test
    {

        //Build NB index
        /*
        static void Main(string[] args)
        {
            buildIndex dataset = new buildIndex();
            dataset.build3HopIndex(488378, "N3B");
        }
        */
         
        /*
       //build IDMap Index
       static void Main(string[] args)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy("server=(local);uid=sa;pwd=-q-q3278800;database=LUBMOld;Connect Timeout=500");
            sqlBulkCopy.DestinationTableName = "IDlist";
            sqlBulkCopy.BulkCopyTimeout = 600;
            sqlBulkCopy.WriteToServer(directedNeighborhood.buildIDMAP());
            sqlBulkCopy.Close();
        }
         * */



        
        static void Main(string[] args)
        {
            // inital step of query graph and original graph
            DataTable queryGraph = new DataTable();
            DataTable keywordIDList = new DataTable();
            Dictionary<int, string> queryIDWithKeyword = new Dictionary<int, string>();
            string sql = "select * from IDlist order by px asc";
            DataTable IDlist = DataHelpe.GetDataTable(sql);
            //update new index in memory method to improve query performance.
            Dictionary<int, List<NBEntry>> NBindex = IndexInMemory.loadNBIndex("N3B");
            DataTable insertTable = new DataTable();
            insertTable.Columns.Add("queryTemplateID", typeof(int));
            insertTable.Columns.Add("queryTime", typeof(double));
            int i = 2;
            int j = 0;
            while (i < 11)
            {
                string sqlQueryGraph = "select * from queryGraphTemplates where queryTemplateID=" + i;
                queryGraph = DataHelpe.GetDataTable(sqlQueryGraph);
                string keywordList = "select * from queryIDlist where queryTemplateID=" + i;
                keywordIDList = DataHelpe.GetDataTable(keywordList);
                if (queryGraph.Rows.Count == 0)
                {
                    queryGraph.Clear();
                    keywordIDList.Clear();
                    queryIDWithKeyword.Clear();
                    Console.WriteLine(i);
                    i++;
                    continue;
                }
                foreach (DataRow dr in keywordIDList.Rows)
                {
                    queryIDWithKeyword.Add((int)dr["queryNodeID"], (string)dr["generalizedKeyword"]);
                }
                Dictionary<string, List<int>> intervalForString = intervalString.intervalStringGeneration(queryIDWithKeyword, IDlist);
                DataTable queryGraph1 = new DataTable();

                //begin 3 hops
                double diff1 = 0;
                while (true)
                {
                    queryGraph1 = queryGraph.Copy();
                    Console.WriteLine("Begin one level tree Graph By Example algorithm for full 3 level indexing");
                    TimeSpan diff11 = GBEBasedOnOneLevelTreeForThreeHops.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    queryGraph1 = queryGraph.Copy();
                    Console.WriteLine("Begin one level tree Graph By Example algorithm for full 3 level indexing");
                    TimeSpan diff12 = GBEBasedOnOneLevelTreeForThreeHops.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    queryGraph1 = queryGraph.Copy();
                    Console.WriteLine("Begin one level tree Graph By Example algorithm for full 3 level indexing");
                    TimeSpan diff13 = GBEBasedOnOneLevelTreeForThreeHops.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    queryGraph1 = queryGraph.Copy();
                    Console.WriteLine("Begin one level tree Graph By Example algorithm for full 3 level indexing");
                    TimeSpan diff14 = GBEBasedOnOneLevelTreeForThreeHops.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    queryGraph1 = queryGraph.Copy();
                    Console.WriteLine("Begin one level tree Graph By Example algorithm for full 3 level indexing");
                    TimeSpan diff15 = GBEBasedOnOneLevelTreeForThreeHops.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    diff1 = (diff12.TotalMilliseconds + diff13.TotalMilliseconds + diff14.TotalMilliseconds + diff15.TotalMilliseconds) / 4;
                    double deviation = Math.Pow((diff12.TotalMilliseconds - diff1), 2) + Math.Pow((diff13.TotalMilliseconds - diff1), 2) + Math.Pow((diff14.TotalMilliseconds - diff1), 2) + Math.Pow((diff15.TotalMilliseconds - diff1), 2);
                    if (deviation < (0.2 * diff1) * (0.2 * diff1) * 4)
                    {
                        break;
                    }
                }
                Console.WriteLine(diff1);
                insertTable.Rows.Add(i, diff1);


                //begin 1 level tree
                double diff4 = 0;
                while (true)
                {
                    queryGraph1 = queryGraph.Copy();
                    Console.WriteLine("Begin STWIGPLUS");
                    TimeSpan diff41 = STWIGPlus.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    queryGraph1 = queryGraph.Copy();
                    TimeSpan diff42 = STWIGPlus.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    queryGraph1 = queryGraph.Copy();
                    TimeSpan diff43 = STWIGPlus.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    queryGraph1 = queryGraph.Copy();
                    TimeSpan diff44 = STWIGPlus.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    queryGraph1 = queryGraph.Copy();
                    TimeSpan diff45 = STWIGPlus.componentMatchingTime(queryGraph1, queryIDWithKeyword, IDlist, NBindex, intervalForString);
                    diff4 = (diff42.TotalMilliseconds + diff43.TotalMilliseconds + diff44.TotalMilliseconds + diff45.TotalMilliseconds) / 4;
                    double deviation = Math.Pow((diff42.TotalMilliseconds - diff4), 2) + Math.Pow((diff43.TotalMilliseconds - diff4), 2) + Math.Pow((diff44.TotalMilliseconds - diff4), 2) + Math.Pow((diff45.TotalMilliseconds - diff4), 2);
                    if (deviation < (0.2 * diff4) * (0.2 * diff4) * 4)
                    {
                        break;
                    }
                }
                Console.WriteLine(diff4);
                insertTable.Rows.Add(i + 300, diff4);
                //finish 1 level tree
                queryGraph.Clear();
                keywordIDList.Clear();
                queryIDWithKeyword.Clear();
                Console.WriteLine(i);
                i++;
                j++;
                if (j == 1)
                {
                    SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(DataHelpe.Constr);
                    sqlBulkCopy.DestinationTableName = "queryTime";
                    sqlBulkCopy.BulkCopyTimeout = 600;
                    sqlBulkCopy.WriteToServer(insertTable);
                    sqlBulkCopy.Close();
                    insertTable.Clear();
                    j = 0;
                }
            }

        }
 
    }
}
