using System.Net.Http;
using System.Data.Entity;
using Layout_FrameMenu;

namespace DatabaseApp.DataProvider
{
    class APIHelper
    {
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
   
    }

    public class RoutingContext : DbContext
    {

        public DbSet<Edge> Positions { get; set; }
        

        public DbSet<CostData> Costs { get; set; }
    }
}
