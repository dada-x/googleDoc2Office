using Google.Apis.Drive.v3;

public class AppSettings
{
    //google doc
    public static readonly string AppName = "";
    public static readonly string GoogleSpreadSheetId = "";
    public static readonly string GoogleMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public static readonly string LocalFile = "xgp_master_list.xlsx";
    public static readonly string[] GoogleDriveScopes = { DriveService.Scope.DriveReadonly };
    public static readonly string[] OneDriveScopes = { "Files.ReadWrite" };

    //onedrive
    public static readonly string OneDriveAppId = "";
    public static readonly string OneDriveTenantId = "consumers";
    public static readonly string OneDriveClientSecret = "";
    public static readonly string OneDriveFolderId = "";
    public static readonly string OneDriveRefreshToken = "";


    //nutcloud
    public static readonly string NutCouldDAVHost = "";
    public static readonly string NutCloudUserName = "";
    public static readonly string NutCloudPW = "";
}