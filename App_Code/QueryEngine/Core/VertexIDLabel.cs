using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using GBE.QueryEngine;

namespace GBE.Core
{//this class was meant to help the query, however some parts of it may not be used in recent versions.
    public class VertexIDLabel
    {
        public static List<string> vertexLabel=new List<string>();

        public static Dictionary<int, List<string>> vertexLabelForQuery = new Dictionary<int, List<string>>();//targeted to separate the list by the queries.

        public static List<EdgeID> edgeLabel = new List<EdgeID>();

        public static Dictionary<int, List<EdgeID>> edgeLabelForQuery = new Dictionary<int, List<EdgeID>>();

        public static Dictionary<int,string> getVertexID()
        {
            return   vertexLabel.Select((s, i) => new { s, i }).ToDictionary(x => x.i, x => x.s);
        }

        public static DataTable database = new DataTable();
        public static Dictionary<int, List<NBEntry>> NBindex = new Dictionary<int, List<NBEntry>>();
    }
}
