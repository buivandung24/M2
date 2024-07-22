using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LoginSceneManager : MonoBehaviour
{
    //public GameObject loadingAnimation; // Assign your loading animation GameObject here
    public GameObject loginPanel; // Assign your login panel GameObject here
    private string serverURL = "https://your-cloud-server.com/connect"; // Your server URL

    private void Start()
    {
        ConnectToServer();
    }

    // Method to connect to the server
    void ConnectToServer()
    {
        StartCoroutine(ServerConnectionCoroutine());
    }

    IEnumerator ServerConnectionCoroutine()
    {
        //loadingAnimation.SetActive(true); // Activate the loading animation
        loginPanel.SetActive(false); // Deactivate the login panel

        UnityWebRequest request = UnityWebRequest.Get(serverURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Connected to server successfully");
            OnConnectedToServer();
        }
        else
        {
            Debug.LogError("Error connecting to server: " + request.error);
            OnFailedToConnect();
        }
    }

    // This would be the method called when the connection to the server is established
    public void OnConnectedToServer()
    {
        StartCoroutine(LoadingToLoginTransition());
    }

    // Coroutine to manage the transition from the loading animation to the login panel
    IEnumerator LoadingToLoginTransition()
    {
        // Assume the loading animation takes some time and simulate a delay
        yield return new WaitForSeconds(2.0f); // Adjust the time according to your animation

        // Turn off the loading animation and enable the login panel
        //loadingAnimation.SetActive(false);
        loginPanel.SetActive(true);
    }

    // This would be the method called when the connection fails
    public void OnFailedToConnect()
    {
        // Handle failed connection, e.g., by showing an error message to the user
        Debug.LogError("Failed to connect to the server.");
        // Possibly re-enable login panel or show retry button
        loginPanel.SetActive(true);
    }
}