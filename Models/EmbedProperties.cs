using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoldBIEmbedSample.Models
{
    public class EmbedProperties
    {
        //BoldBI server URL(ex: http://localhost:5000/bi, http://demo.boldbi.com/bi)
        public static string RootUrl = "http://localhost:5000/bi";

        //  For Bold BI Enterprise edition, it should be like `site/site1`. For Bold BI Cloud, it should be empty string.
          public static string SiteIdentifier = "site/site1";

        //Enter your BoldBI credentials here.
        public static string UserEmail = ""; //Provide the User Email

        //Provide the dashboard id of the dashboard you want to render
        public static string DashboardId = ""; 

        // Your Bold BI application environment. (If Cloud, you should use `cloud`, if Enterprise, you should use `enterprise`)
        public static string Environment = "enterprise";

        // Get the embedSecret key from Bold BI.
        public static string EmbedSecret = ""; //Provide the embed secret key.
    }
}
