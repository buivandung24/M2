using UnityEngine;
using Photon.Pun; // For Photon functions
using Photon.Realtime; // For Photon networking
using UnityEngine.SceneManagement;
using DA_Assets.FCU.Drawers.CanvasDrawers; // For scene management

// Class that handles the connection to the Photon server and transitions to the "login" scene
public class PhotonConnector : MonoBehaviourPunCallbacks
{
    public GameObject loginScene;
    public GameObject firstScene;
    void Start()
    {
        ConnectToPhoton();
    }

    // Method to connect to Photon server
    void ConnectToPhoton()
    {
        // Check if we are connected, if not, connect to the server
        if (!PhotonNetwork.IsConnected)
        {
            // Connect using the current settings specified in the PhotonServerSettings
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            // If already connected, we can directly go to the login scene
            GoToLoginScene();
        }
    }

    // This override function is called when the connection to the server is established
    public override void OnConnectedToMaster()
    {
        // Once connected to the Photon master server, transition to the "login" scene
        GoToLoginScene();
    }

    // Method to load the "login" scene
    void GoToLoginScene()
    {
        // Load the "login" scene
        loginScene.SetActive(true);
        firstScene.SetActive(false);
    }

    // This override function is called when the connection failed
    public override void OnDisconnected(DisconnectCause cause)
    {
        // Log or handle the disconnection
        Debug.LogError("Disconnected from Photon server with cause: " + cause.ToString());
    }
}