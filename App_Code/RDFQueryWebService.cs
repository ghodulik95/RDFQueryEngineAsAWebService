using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Services;
using System.Data.SqlClient;
using System;
using System.Text;
using GBE.QueryEngine;
using GBE.Core;
using System.Data;

/// <summary>
/// Summary description for RDFQueryWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class RDFQueryWebService : System.Web.Services.WebService
{
    public static readonly string SUBJECT = "Subject";
    public static readonly string OBJECT = "Object";
    //WAS Predict
    public static readonly string PREDICATE = "Predicate";

    public RDFQueryWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string GetSubjects(string databaseName, string currentInput)
    {
        if (databaseName.Contains("]"))
        {
            return "ERROR: Invalid database name.";
        }
        if (currentInput.Contains("]"))
        {
            return "ERROR: Invalid database name.";
        }
        string tableName = "RDF";
        try {
            SqlConnection myConnection = new SqlConnection("server=.\\SQLEXPRESS;" +
                                           "Trusted_Connection=yes;" +
                                           "database="+databaseName+"; " +
                                           "connection timeout=30;" +
                                           "Integrated Security = SSPI;");
            myConnection.Open();
            SqlCommand c = new SqlCommand("SELECT DISTINCT TOP 20 s.* FROM ("+
                "(SELECT ["+SUBJECT+"] FROM ["+databaseName+"].[dbo].["+tableName+ "] WHERE [" + SUBJECT + "] LIKE '%"+currentInput+"%' ) UNION "+
                " (SELECT [" + OBJECT+"] FROM [" + databaseName + "].[dbo].[" + tableName + "] WHERE [" + OBJECT + "] LIKE '%" + currentInput + "%')) s", myConnection);
            SqlDataReader reader = c.ExecuteReader();
            StringBuilder results = new StringBuilder();
            results.Append("{ \"subjects\":[");
            Boolean isFirst = true;
            while (reader.Read())
            {
                if (!isFirst)
                {
                    results.Append(",");
                }
                results.Append("\""+ reader[0].ToString()+"\"");
                isFirst = false;
            }
            results.Append("]}");
            return results.ToString();
        }catch(Exception e)
        {
            return e.ToString();
        }
    }

    [WebMethod]
    public string GetEdges(string databaseName)
    {
        if (databaseName.Contains("]"))
        {
            return "ERROR: Invalid database name.";
        }

        string tableName = "RDF";
        try
        {
            SqlConnection myConnection = new SqlConnection("server=.\\SQLEXPRESS;" +
                                           "Trusted_Connection=yes;" +
                                           "database=" + databaseName + "; " +
                                           "connection timeout=30");
            myConnection.Open();
            SqlCommand c = new SqlCommand("SELECT DISTINCT ["+PREDICATE+"] FROM [" + databaseName + "].[dbo].[" + tableName + "]", myConnection);
            SqlDataReader reader = c.ExecuteReader();
            StringBuilder results = new StringBuilder("[");
            Boolean isFirst = true;
            while (reader.Read())
            {
                if (!isFirst)
                {
                    results.Append(",");
                }
                results.Append("\""+reader[0].ToString() + "\"");
                isFirst = false;
            }
            results.Append("]");
            return results.ToString();
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }

    [WebMethod]
    public string Hello(string names)
    {
        //List<Dictionary<int, string>> res = GUI.MainForm.RunQuery(Dictionary<int, string> vertexLabel, List<Core.EdgeID> edgeLabel)
        Dictionary <string, string[]> ns = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(names);
        var greetings = new Dictionary<string, List<string>>();
        greetings.Add("lines", new List<string>());
        foreach (string n in ns["names"])
        {
            greetings["lines"].Add("Hello " + n);
        }
        return JsonConvert.SerializeObject(greetings);
    }


    //The following two functions are written by Shi Qiao on loading the database to memory and run the query.
    /*
    private List<Dictionary<int, string>> RunQuery(Dictionary<int, string> vertexLabel, List<Core.EdgeID> edgeLabel)
    {
        //run query function.
        DataTable queryGraph = new DataTable();
        queryGraph.Columns.Add("ID1", typeof(int));
        queryGraph.Columns.Add("Node1", typeof(string));
        queryGraph.Columns.Add("ID2", typeof(int));
        queryGraph.Columns.Add("Node2", typeof(string));
        foreach (Core.EdgeID a in edgeLabel)
        {
            queryGraph.Rows.Add(a.ID1, vertexLabel[a.ID1], a.ID2, vertexLabel[a.ID2]);
        }
        Dictionary<string, List<int>> intervalForString = intervalString.intervalStringGeneration(vertexLabel, VertexIDLabel.database);
        List<Dictionary<int, int>> resultsID = new List<Dictionary<int, int>>();
        resultsID = GBEBasedOnOneLevelTreeForThreeHops.componentMatchingResult(queryGraph, vertexLabel, VertexIDLabel.database, VertexIDLabel.NBindex, intervalForString);
        List<Dictionary<int, string>> resultsString = new List<Dictionary<int, string>>();
        Dictionary<int, string> labelID = new Dictionary<int, string>();
        foreach (Dictionary<int, int> result in resultsID)
        {
            Dictionary<int, string> resultStringTemp = new Dictionary<int, string>();
            foreach (KeyValuePair<int, int> pair in result)
            {
                if (labelID.ContainsKey(pair.Value))
                {
                    resultStringTemp.Add(pair.Key, labelID[pair.Value]);
                }
                else
                {
                    DataRow[] label = VertexIDLabel.database.Select("px= " + pair.Value);
                    string labelForID = null;
                    foreach (DataRow row in label)
                    {
                        labelForID = (string)row["Node"];
                    }
                    resultStringTemp.Add(pair.Key, labelForID);
                    labelID.Add(pair.Value, labelForID);
                }
            }
            resultsString.Add(resultStringTemp);
        }
        return resultsString;
    }*/



    private List<Dictionary<int, string>> RunQueryWithPath(Dictionary<int, List<VertexID>> compVertexList, Dictionary<int, List<EdgeID>> comEdgeList, List<CompPathID> compPathList)
    {//with path.
        return new List<Dictionary<int, string>>();
    }


    [WebMethod]
    public string testQuery(string q)
    {
        Dictionary<int, string> vertexLabel = new Dictionary<int, string>();
        vertexLabel[3] = "<http://www.Department2.University1.edu/GraduateStudent*";
        vertexLabel[5] = "<http://www.lehigh.edu/~zhp2/2004/0401/univ-bench.owl#GraduateStudent>";
       //vertexLabel[2] = "<http://www.Department2.University1.edu/FullProfessor0/Publication*";
        List<GBE.Core.EdgeID> edgeLabel = new List<GBE.Core.EdgeID>();
        var edge = new GBE.Core.EdgeID(3, 5, "#type");
        edgeLabel.Add(edge);
       // edge = new GBE.Core.EdgeID(2, 0, "#publicationAuthor");
       // edgeLabel.Add(edge);

        var res = RunQuery(vertexLabel, edgeLabel);
        string toReturn = "";
        foreach(var r in res)
        {
            foreach(KeyValuePair<int,string> kv in r)
            {
                toReturn += kv.Key + " " + kv.Value + ", ";
            }
            toReturn += " -- ";
        }
        return toReturn;
    }

    [WebMethod]
    //{ nodes: [{ id: 0, label: '<http://www.Department2.University1.edu/GraduateStudent*' },{ id: 1, label: '<http://www.lehigh.edu/~zhp2/2004/0401/univ-bench.owl#GraduateStudent>' }], edges: [{ source: 0, target: 1, label: 'edge0' }] }
    public string callRunQuery(string queryAsJSON, string dbname)
    {
        Dictionary<int, string> vertexLabel = new Dictionary<int, string>();
        List<GBE.Core.EdgeID> edgeLabel = new List<GBE.Core.EdgeID>();
        try {
            dynamic query = JsonConvert.DeserializeObject(queryAsJSON);
            foreach (dynamic node in query.nodes)
            {
                vertexLabel.Add((int)node.id, (string)node.text);
            }

            foreach (dynamic edge in query.edges)
            {
                edgeLabel.Add(new GBE.Core.EdgeID((int)edge.source, (int)edge.target, (string)edge.text));
            }
        } catch (JsonSerializationException e)
        {
            return e.ToString();
        }
        catch (Exception e)
        {
            return e.ToString();
        }

        return resultsToJSON(RunQuery(vertexLabel, edgeLabel));
    }

    private string resultsToJSON(List<Dictionary<int, string>> res)
    {
        var toReturn = new StringBuilder("{ \"results\":[");
        Boolean isFirstResult = true;
        foreach (var r in res)
        {
            if (!isFirstResult)
            {
                toReturn.Append("},");
            }
            toReturn.Append("{");
            Boolean isFirstVal = true;
            foreach (KeyValuePair<int, string> kv in r)
            {
                if (!isFirstVal)
                {
                    toReturn.Append(",");
                }
                toReturn.Append("\"");
                toReturn.Append(kv.Key);
                toReturn.Append("\": \"");
                toReturn.Append(kv.Value);
                toReturn.Append("\"");
                isFirstVal = false;
            }
            isFirstResult = false;

        }
        toReturn.Append("}]}");
        return toReturn.ToString();
    }

    //The following two functions are written by Shi Qiao on loading the database to memory and run the query.
    private List<Dictionary<int, string>> RunQuery(Dictionary<int, string> vertexLabel, List<GBE.Core.EdgeID> edgeLabel)
    {
        //run query function.
        loadDatabase(1);
        DataTable queryGraph = new DataTable();
        queryGraph.Columns.Add("ID1", typeof(int));
        queryGraph.Columns.Add("Node1", typeof(string));
        queryGraph.Columns.Add("ID2", typeof(int));
        queryGraph.Columns.Add("Node2", typeof(string));
        foreach (GBE.Core.EdgeID a in edgeLabel)
        {
            queryGraph.Rows.Add(a.ID1, vertexLabel[a.ID1], a.ID2, vertexLabel[a.ID2]);
        }
        Dictionary<string, List<int>> intervalForString = intervalString.intervalStringGeneration(vertexLabel, VertexIDLabel.database);
        List<Dictionary<int, int>> resultsID = new List<Dictionary<int, int>>();
        resultsID = GBEBasedOnOneLevelTreeForThreeHops.componentMatchingResult(queryGraph, vertexLabel, VertexIDLabel.database, VertexIDLabel.NBindex, intervalForString);
        List<Dictionary<int, string>> resultsString = new List<Dictionary<int, string>>();
        Dictionary<int, string> labelID = new Dictionary<int, string>();
        foreach (Dictionary<int, int> result in resultsID)
        {
            Dictionary<int, string> resultStringTemp = new Dictionary<int, string>();
            foreach (KeyValuePair<int, int> pair in result)
            {
                if (labelID.ContainsKey(pair.Value))
                {
                    resultStringTemp.Add(pair.Key, labelID[pair.Value]);
                }
                else
                {
                    DataRow[] label = VertexIDLabel.database.Select("px= " + pair.Value);
                    string labelForID = null;
                    foreach (DataRow row in label)
                    {
                        labelForID = (string)row["Node"];
                    }
                    resultStringTemp.Add(pair.Key, labelForID);
                    labelID.Add(pair.Value, labelForID);
                }
            }
            resultsString.Add(resultStringTemp);
        }
        return resultsString;
    }



    private void loadDatabase(int ID)
    {

        string connectionString;
        switch (ID)
        {
            /*case 1:
                connectionString = "server=localhost;database=LUBM;Trusted_Connection=True;Connect Timeout=500";
                break;
            case 2:
                connectionString = "server=localhost;database=uniprotTest;Trusted_Connection=True;Connect Timeout=500";
                break;
            */
            default:
                connectionString = "server=.\\SQLEXPRESS;" +
                                       "Trusted_Connection=yes;" +
                                       "database=LUBM; " +
                                       "connection timeout=30;" +
                                       "Integrated Security = SSPI;";
                break;
        }
        DataHelpe.Constr = connectionString;
        string sql = "select * from IDlist order by px asc";
        DataTable IDlist = DataHelpe.GetDataTable(sql);
        List<DataTable> database = new List<DataTable>();
        VertexIDLabel.database = IDlist;
        Dictionary<int, List<NBEntry>> NBindex;
        switch (ID)
        {
            /*case 1:
                NBindex = IndexInMemory.loadNBIndex("N3B");
                break;
            case 2:
                NBindex = IndexInMemory.loadNBIndex("N1B2Hop");
                break;*/
            default:
                NBindex = IndexInMemory.loadNBIndex("N1B1");//("N3B");
                break;

        }
        VertexIDLabel.NBindex = NBindex;
    }

}
