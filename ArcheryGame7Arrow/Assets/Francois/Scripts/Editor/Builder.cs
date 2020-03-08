using UnityEditor;

/// <summary>
/// Editor class to help manage the builds of the project
/// </summary>
public class Builder
{
#if UNITY_EDITOR
    
    // All scenes used for the game
    private static string[] _clientScenes = new[]
    {
        "Assets/Francois/Scene/MainMenu.unity",
        "Assets/Francois/Scene/GameScene.unity"
    };
    private static string[] _serverScenes = new[]
    {
        "Assets/Francois/Scene/StartServerScene.unity",
        "Assets/Francois/Scene/GameScene.unity"
    };
    // Default Build folder
    private static string _buildFolder = "Builds/";
    // Subbuild folder for windows builds
    private static string _serverFolder = "Server/";
    private static string _clientFolder = "Client/";
    // Android executable name
    private static string ServerExe = "Server.exe";
    private static string GameEXE = "Client.exe";

    [MenuItem("AnkleBreaker/Build/Server", false, 100)]
    public static void ServerBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = _serverScenes;
        buildPlayerOptions.locationPathName = _buildFolder + _serverFolder + ServerExe;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    
    [MenuItem("AnkleBreaker/Build/Client", false, 100)]
    public static void ClientBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = _clientScenes;
        buildPlayerOptions.locationPathName = _buildFolder + _clientFolder + GameEXE;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
#endif

}