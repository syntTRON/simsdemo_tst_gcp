using RestSharp;

namespace SIMS
{
    public static class Helper
    {
        static string APIURL = Environment.GetEnvironmentVariable("api") ?? "http://localhost:8888";
        
        public static string URL_getToken = $"{APIURL}/AuthService?username=%1&password=%2";
        public static string URL_checkToken = $"{APIURL}/AuthService/check?username=%1&token=%2";

        public static string getToken(string username, string password)
        {
            RestClient client = new RestClient(Helper.URL_getToken.Replace("%1", username).Replace("%2", password));
            RestRequest request = new RestRequest("", Method.Post);
            RestResponse response = client.Execute(request);
            string token = response.Content ?? "";
            token = token.Replace("\"", "");
            return token;
        }

        public static bool checkToken(string username, string token)
        {
            RestClient client = new RestClient(Helper.URL_checkToken.Replace("%1", username).Replace("%2", token));
            RestRequest request = new RestRequest("", Method.Get);
            RestResponse response = client.Execute(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK ? true : false;
        }
    }
}
