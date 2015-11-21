using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace GBE.QueryEngine
{
    public class generateNeighbor
    {
        public generateNeighbor()
        {
        }
        public static Dictionary<string, Dictionary<int, int>> returnNeighborhood(DataTable queryGraph, int ID)
        {
            Dictionary<string, Dictionary<int, int>> indexNeighborhood = new Dictionary<string, Dictionary<int, int>>();
            DataRow[] neighborhoodForward = queryGraph.Select("ID1=" + ID);
            int rows = neighborhoodForward.Count();
            DataTable forwardNode = new DataTable();
            List<int> firstLevelForwardNeighbor = new List<int>();
            if (!(rows == 0))
            {
                forwardNode = neighborhoodForward.CopyToDataTable();
            }
            if (!(rows == 0))
            {

                for (int i = 0; i < rows; i++)
                {
                    string Node = forwardNode.Rows[i][3].ToString();
                    firstLevelForwardNeighbor.Add(int.Parse(forwardNode.Rows[i][2].ToString()));
                    if (indexNeighborhood.ContainsKey(Node))
                    {
                        Dictionary<int, int> index = new Dictionary<int, int>();
                        index = indexNeighborhood[Node];
                        if (index.ContainsKey(1))
                        {
                            (indexNeighborhood[Node])[1] = (indexNeighborhood[Node])[1] + 1;
                        }
                        else
                        {
                            (indexNeighborhood[Node]).Add(1, 1);
                        }
                    }
                    else
                    {
                        Dictionary<int, int> index = new Dictionary<int, int>();
                        index.Add(1, 1);
                        indexNeighborhood.Add(Node, index);
                    }
                }
            }
            DataRow[] neighborhoodBackward = queryGraph.Select("ID2=" + ID);
            int rows2 = neighborhoodBackward.Count();
            DataTable backwardNode = new DataTable();
            List<int> firstLevelBackwardNeighbor = new List<int>();
            if (!(rows2 == 0))
            {
                backwardNode = neighborhoodBackward.CopyToDataTable();
            }
            if (!(rows2 == 0))
            {

                for (int i = 0; i < rows2; i++)
                {
                    string Node = backwardNode.Rows[i][1].ToString();
                    firstLevelBackwardNeighbor.Add(int.Parse(backwardNode.Rows[i][0].ToString()));
                    if (indexNeighborhood.ContainsKey(Node))
                    {
                        Dictionary<int, int> index = new Dictionary<int, int>();
                        index = indexNeighborhood[Node];
                        if (index.ContainsKey(-1))
                        {
                            (indexNeighborhood[Node])[-1] = (indexNeighborhood[Node])[-1] + 1;
                        }
                        else
                        {
                            (indexNeighborhood[Node]).Add(-1, 1);
                        }
                    }
                    else
                    {
                        Dictionary<int, int> index = new Dictionary<int, int>();
                        index.Add(-1, 1);
                        indexNeighborhood.Add(Node, index);
                    }
                }
            }
            //first level indexing finish
            //begin the second level
            List<int> discoveredForwardIDs = new List<int>();
            List<int> discoveredBackwardIDs = new List<int>();
            discoveredForwardIDs.AddRange(firstLevelForwardNeighbor);
            discoveredBackwardIDs.AddRange(firstLevelBackwardNeighbor);
            List<int> secondForwardLevelNeighbor = new List<int>();
            List<int> secondBackwardLevelNeighbor = new List<int>();

            for (int i = 0; i < firstLevelForwardNeighbor.Count; i++)
            {
                //here might need to clear the datarow set
                neighborhoodForward = queryGraph.Select("ID1=" + firstLevelForwardNeighbor[i]);
                rows = neighborhoodForward.Count();
                forwardNode = new DataTable();
                if (!(rows == 0))
                {
                    forwardNode = neighborhoodForward.CopyToDataTable();
                }
                if (!(rows == 0))
                {

                    for (int j = 0; j < rows; j++)
                    {
                        string Node = forwardNode.Rows[j][3].ToString();
                        int newID = int.Parse(forwardNode.Rows[j][2].ToString());
                        if (discoveredForwardIDs.Contains(newID))
                        {
                        }
                        else
                        {
                            secondForwardLevelNeighbor.Add(newID);
                            discoveredForwardIDs.Add(newID);
                            if (indexNeighborhood.ContainsKey(Node))
                            {
                                Dictionary<int, int> index = new Dictionary<int, int>();
                                index = indexNeighborhood[Node];
                                if (index.ContainsKey(2))
                                {
                                    (indexNeighborhood[Node])[2] = (indexNeighborhood[Node])[2] + 1;
                                }
                                else
                                {
                                    (indexNeighborhood[Node]).Add(2, 1);
                                }
                            }
                            else
                            {
                                Dictionary<int, int> index = new Dictionary<int, int>();
                                index.Add(2, 1);
                                indexNeighborhood.Add(Node, index);
                            }
                        }
                    }
                }
            }



            for (int i = 0; i < firstLevelBackwardNeighbor.Count; i++)
            {
                //here might need to clear the datarow set
                neighborhoodBackward = queryGraph.Select("ID2=" + firstLevelBackwardNeighbor[i]);
                rows = neighborhoodBackward.Count();
                backwardNode = new DataTable();
                if (!(rows == 0))
                {
                    backwardNode = neighborhoodBackward.CopyToDataTable();
                }
                if (!(rows == 0))
                {

                    for (int j = 0; j < rows; j++)
                    {
                        string Node = backwardNode.Rows[j][1].ToString();
                        int newID = int.Parse(backwardNode.Rows[j][0].ToString());
                        if (discoveredBackwardIDs.Contains(newID))
                        {
                        }
                        else
                        {
                            secondBackwardLevelNeighbor.Add(newID);
                            discoveredBackwardIDs.Add(newID);
                            if (indexNeighborhood.ContainsKey(Node))
                            {
                                Dictionary<int, int> index = new Dictionary<int, int>();
                                index = indexNeighborhood[Node];
                                if (index.ContainsKey(-2))
                                {
                                    (indexNeighborhood[Node])[-2] = (indexNeighborhood[Node])[-2] + 1;
                                }
                                else
                                {
                                    (indexNeighborhood[Node]).Add(-2, 1);
                                }
                            }
                            else
                            {
                                Dictionary<int, int> index = new Dictionary<int, int>();
                                index.Add(-2, 1);
                                indexNeighborhood.Add(Node, index);
                            }
                        }
                    }
                }
            }




            // Second level indexing is finished, third level indexing begins

            for (int i = 0; i < secondForwardLevelNeighbor.Count; i++)
            {
                //here might need to clear the datarow set
                neighborhoodForward = queryGraph.Select("ID1=" + secondForwardLevelNeighbor[i]);
                rows = neighborhoodForward.Count();
                forwardNode = new DataTable();
                if (!(rows == 0))
                {
                    forwardNode = neighborhoodForward.CopyToDataTable();
                }
                if (!(rows == 0))
                {

                    for (int j = 0; j < rows; j++)
                    {
                        string Node = forwardNode.Rows[j][3].ToString();
                        int newID = int.Parse(forwardNode.Rows[j][2].ToString());
                        if (discoveredForwardIDs.Contains(newID))
                        {
                        }
                        else
                        {
                            discoveredForwardIDs.Add(newID);
                            if (indexNeighborhood.ContainsKey(Node))
                            {
                                Dictionary<int, int> index = new Dictionary<int, int>();
                                index = indexNeighborhood[Node];
                                if (index.ContainsKey(3))
                                {
                                    (indexNeighborhood[Node])[3] = (indexNeighborhood[Node])[3] + 1;
                                }
                                else
                                {
                                    (indexNeighborhood[Node]).Add(3, 1);
                                }
                            }
                            else
                            {
                                Dictionary<int, int> index = new Dictionary<int, int>();
                                index.Add(3, 1);
                                indexNeighborhood.Add(Node, index);
                            }
                        }
                    }
                }
            }



            for (int i = 0; i < secondBackwardLevelNeighbor.Count; i++)
            {
                //here might need to clear the datarow set
                neighborhoodBackward = queryGraph.Select("ID2=" + secondBackwardLevelNeighbor[i]);
                rows = neighborhoodBackward.Count();
                backwardNode = new DataTable();
                if (!(rows == 0))
                {
                    backwardNode = neighborhoodBackward.CopyToDataTable();
                }
                if (!(rows == 0))
                {

                    for (int j = 0; j < rows; j++)
                    {
                        string Node = backwardNode.Rows[j][1].ToString();
                        int newID = int.Parse(backwardNode.Rows[j][0].ToString());
                        if (discoveredBackwardIDs.Contains(newID))
                        {
                        }
                        else
                        {
                            discoveredBackwardIDs.Add(newID);
                            if (indexNeighborhood.ContainsKey(Node))
                            {
                                Dictionary<int, int> index = new Dictionary<int, int>();
                                index = indexNeighborhood[Node];
                                if (index.ContainsKey(-3))
                                {
                                    (indexNeighborhood[Node])[-3] = (indexNeighborhood[Node])[-3] + 1;
                                }
                                else
                                {
                                    (indexNeighborhood[Node]).Add(-3, 1);
                                }
                            }
                            else
                            {
                                Dictionary<int, int> index = new Dictionary<int, int>();
                                index.Add(-3, 1);
                                indexNeighborhood.Add(Node, index);
                            }
                        }
                    }
                }
            }

            return indexNeighborhood;
        }
    }
}