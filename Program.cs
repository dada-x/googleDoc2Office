using System;
using Google.Apis.Auth.OAuth2;
using System.IO;
using Google.Apis.Services;
using Google.Apis.Drive.v3;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using System.Security;
using System.Linq;
using System.Net.Http;
using WebDAVClient;
using System.Timers;

namespace googleDoc2Office
{
    class Program
    {
        static Timer _processTimer;
        static DriveService _googleDriveService;
        static GraphServiceClient _graphServiceClient;
        static Client _nutCloudClient;
        static void Main(string[] args)
        {
            InitalizeClients();
            ProcessFile();

            _processTimer = new Timer();
            _processTimer.Interval = 1000 * 3600 * 24;
            _processTimer.AutoReset = true;
            _processTimer.Elapsed += (s, e) =>
            {
                try
                {
                    ProcessFile();
                }
                catch
                {

                }
            };
            _processTimer.Start();

            Console.Read();
        }

        static void ProcessFile()
        {
            DownloadFromGoogleDrive();
            //PublishToOneDrive();
            PublishToNutCloud();
        }

        static void InitalizeClients()
        {
            _googleDriveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleAuthProvider.GetCredential(),
                ApplicationName = AppSettings.AppName
            });

            _graphServiceClient = new GraphServiceClient(OneDriveAuthProvider.GetCredential());

            _nutCloudClient = new Client(NutCloudAuthProvider.GetCredential());
            _nutCloudClient.Server = AppSettings.NutCouldDAVHost;
            _nutCloudClient.BasePath = "/dav/";
            _nutCloudClient.Port = -1;
        }

        static void DownloadFromGoogleDrive()
        {
            var request = _googleDriveService.Files.Export(AppSettings.GoogleSpreadSheetId, AppSettings.GoogleMimeType);
            using (var mStream = new MemoryStream())
            {
                if (System.IO.File.Exists(AppSettings.LocalFile))
                {
                    System.IO.File.Delete(AppSettings.LocalFile);
                }

                using (var fileStream = new FileStream(AppSettings.LocalFile, FileMode.CreateNew, FileAccess.Write))
                {
                    var res = request.ExecuteAsStream();
                    res.CopyTo(mStream);
                    mStream.WriteTo(fileStream);
                    res.Dispose();
                }
            }
        }

        static void PublishToOneDrive()
        {
            using (var fileStream = new FileStream(AppSettings.LocalFile, FileMode.Open))
            {
                var uploadedFile = _graphServiceClient.Me.Drive.Items[AppSettings.OneDriveFolderId]
                                        .ItemWithPath(AppSettings.LocalFile)
                                        .Content
                                        .Request()
                                        .PutAsync<DriveItem>(fileStream).Result;
            }
        }

        static void PublishToNutCloud()
        {
            try
            {
                _nutCloudClient.DeleteFile($"XGP/{AppSettings.LocalFile}").Wait();
            }
            catch
            {

            }

            using (var fileStream = new FileStream(AppSettings.LocalFile, FileMode.Open))
            {
                var result = _nutCloudClient.Upload($"XGP/master_list", fileStream, AppSettings.LocalFile).Result;
            }
        }
    }
}
