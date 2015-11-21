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
                                           "connection timeout=30;" +
                                           "Integrated Security = SSPI;");
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



    private List<Dictionary<int, string>> RunQueryWithPath(Dictionary<int, List<VertexID>> compVertexList, Dictionary<int, List<EdgeID>> comEdgeList, List<CompPathID> compPathList)
    {//with path.
        return new List<Dictionary<int, string>>();
    }


    /*
    /// <summary>
    /// This function deal with the query results we get from Shi Qiao's program. It count the result and update the status bar. Also it display the first result
    /// in the IDLabel table and create a new Template to display the result.
    /// </summary>
    /// <param name="results"></param>
    private void DealWithResults(List<Dictionary<int, string>> results)
    {
        //resultSet = results;
        resultPosition = 0;//initialize;
        Model activeTemplate = (Model)docManager.ActiveDocument;
        int count = results.Count;//result count;
                                  //We show the number of the result count, and only display one by one. Maybe only keep the first 20 results.
        lblStatus.Text = String.Format("Query:   " + activeTemplate.Name + " Query Time: " + querytime + " ms    Result Count:  " + count);
        //IDLabel.DataSource = null;
        //IDLabel.DataSource = new BindingSource(results[0], null);//display the first result in the table.
        Model newModel = activeTemplate.Clone();
        newModel.Name = String.Format(newModel.Name + "  result");
        if (results.Count == 0)
        {
            newModel.Name = String.Format(newModel.Name + ": empty");//no result.
        }
        Template newTemplate = (Template)newModel;

        //we need to disable some tools when displaying. Tools like query and loadDatabase and some others will be disabled.
        // UpdateTemplate(results[0]);

        DisplayResults(newTemplate, displayNum);


        docManager.AddOrActivate(newTemplate);
        //UpdateTemplate(results,0);
        //DisableTools();
        if (count > displayNum)
        {
            toolNext.Enabled = true;
        }
    }

    //important assumption here: it assumes the newTemplate has one set of result graph.
    private void DisplayResults(Template newTemplate, int num)
    {
        if (resultSet.Count == 1) return;

        if (resultSet.Count == 0)
        {
            int eCount = newTemplate.ShapeCount;
            while (eCount > 0)
            {
                newTemplate.RemoveLastEntity();
                eCount--;
            }
            return;//no results. so just create an empty result file.
        }
        int elCount = newTemplate.ShapeCount;
        newTemplate.RedrawSuspended = true;
        newTemplate.SelectAll();
        newTemplate.SelectedFrame = newTemplate.SelectedRectangle();
        //add new results.
        //here select all the current vertices and edges. Currently can only do this which will result in 2^x results display.
        //results paste has a fixed offset of 400, which need to changed to accomodate the graph template.
        //May do select only the first vertices and edges and calcualte offset based on this and the num and paste to display a new result.
        newTemplate.Copy();
        newTemplate.HorizontalPaste();
        newTemplate.SelectAll();
        newTemplate.Copy();
        for (int i = 1; i < num / 2 && i < (resultSet.Count + 1) / 2; i++)
        {
            newTemplate.VerticalPaste();
            newTemplate.Copy();
        }
        if (resultSet.Count < num && resultSet.Count % 2 == 1)
        {
            while (elCount > 0)
            {
                newTemplate.RemoveLastEntity();
                elCount--;
            }
        }
        newTemplate.DeselectAll();
        GBE.TemplateDesigner.Clipboard.Clear();
        newTemplate.RedrawSuspended = false;
        newTemplate.Redraw();
    }
    */

    [WebMethod]
    public string testQuery()
    {
        Dictionary<int, string> vertexLabel = new Dictionary<int, string>();
        vertexLabel[0] = "<http://www.Department2.University1.edu/GraduateStudent*";
        vertexLabel[1] = "<http://www.lehigh.edu/~zhp2/2004/0401/univ-bench.owl#GraduateStudent>";
        List<GBE.Core.EdgeID> edgeLabel = new List<GBE.Core.EdgeID>();
        var edge = new GBE.Core.EdgeID(0, 1, "#type");
        edgeLabel.Add(edge);

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
