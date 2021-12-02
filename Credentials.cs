using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using WebDAVClient;
using System.Net;

public class OneDriveAuthProvider : IAuthenticationProvider
{
    HttpClient _httpClient = new HttpClient();
    public static OneDriveAuthProvider GetCredential()
    {
        return new OneDriveAuthProvider();
    }

    public async Task AuthenticateRequestAsync(HttpRequestMessage request)
    {
        request.Headers.Authorization = new System.Net.Http.Headers
            .AuthenticationHeaderValue("bearer", await GetToken());
    }

    private async Task<string> GetToken()
    {
        var content = new Dictionary<string, string>();
        content.Add("scope", "Files.ReadWrite offline_access");
        content.Add("client_id", AppSettings.OneDriveAppId);
        content.Add("grant_type", "refresh_token");
        content.Add("refresh_token", AppSettings.OneDriveRefreshToken);
        content.Add("client_secret", AppSettings.OneDriveClientSecret);

        var res = await _httpClient.PostAsync($"https://login.microsoftonline.com/{AppSettings.OneDriveTenantId}/oauth2/v2.0/token", new FormUrlEncodedContent(content));
        var resContent = await res.Content.ReadAsStringAsync();
        return JsonDocument.Parse(resContent).RootElement.GetProperty("access_token").GetString();
    }
}

public class GoogleAuthProvider
{
    public static GoogleCredential GetCredential()
    {
        GoogleCredential credential;
        using (var stream = new FileStream("googleDriveAuth.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(AppSettings.GoogleDriveScopes);
        }
        return credential;
    }
}

public class NutCloudAuthProvider
{
    public static NetworkCredential GetCredential()
    {
        return new NetworkCredential
        {
            UserName = AppSettings.NutCloudUserName,
            Password = AppSettings.NutCloudPW
        };
    }
}