using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for RDFQueryWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class RDFQueryWebService : System.Web.Services.WebService
{

    public RDFQueryWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld()
    {
        return "Hello World";
    }

    [WebMethod]
    public string HelloName(string name)
    {
        return "Hello "+name;
    }

    [WebMethod]
    public string Hello(string names)
    {
        Dictionary<string, string[]> ns = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(names);
        var greetings = new Dictionary<string, List<string>>();
        greetings.Add("lines", new List<string>());
        foreach (string n in ns["names"])
        {
            greetings["lines"].Add("Hello " + n);
        }
        return JsonConvert.SerializeObject(greetings);
    }

}
