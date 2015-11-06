using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Services;
using System.Data.SqlClient;
using System;
using System.Text;

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
    public static readonly string PREDICATE = "Predict";

    public RDFQueryWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string GetSubjects(string databaseName, string tableName)
    {
        if (tableName.Contains("]"))
        {
            return "ERROR: Invalid table name.";
        }else if (databaseName.Contains("]"))
        {
            return "ERROR: Invalid database name.";
        }

        try {
            SqlConnection myConnection = new SqlConnection("server=.\\SQLEXPRESS;" +
                                           "Trusted_Connection=yes;" +
                                           "database="+databaseName+"; " +
                                           "connection timeout=30");
            myConnection.Open();
            SqlCommand c = new SqlCommand("SELECT DISTINCT s.* FROM ((SELECT ["+SUBJECT+"] FROM ["+databaseName+"].[dbo].["+tableName+ "]) UNION (SELECT ["+OBJECT+"] FROM [" + databaseName + "].[dbo].[" + tableName + "])) s", myConnection);
            SqlDataReader reader = c.ExecuteReader();
            StringBuilder results = new StringBuilder();
            while (reader.Read())
            {
                results.Append(reader[0].ToString()+" ");
            }
            return results.ToString();
        }catch(Exception e)
        {
            return e.ToString();
        }
    }

    [WebMethod]
    public string GetEdges(string databaseName, string tableName)
    {
        if (tableName.Contains("]"))
        {
            return "ERROR: Invalid table name.";
        }
        else if (databaseName.Contains("]"))
        {
            return "ERROR: Invalid database name.";
        }

        try
        {
            SqlConnection myConnection = new SqlConnection("server=.\\SQLEXPRESS;" +
                                           "Trusted_Connection=yes;" +
                                           "database=" + databaseName + "; " +
                                           "connection timeout=30");
            myConnection.Open();
            SqlCommand c = new SqlCommand("SELECT DISTINCT ["+PREDICATE+"] FROM [" + databaseName + "].[dbo].[" + tableName + "]", myConnection);
            SqlDataReader reader = c.ExecuteReader();
            StringBuilder results = new StringBuilder();
            while (reader.Read())
            {
                results.Append(reader[0].ToString() + " ");
            }
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

}
