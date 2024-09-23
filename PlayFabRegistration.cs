using System;
using System.Text.RegularExpressions;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script handles the registration process using PlayFab.
public class PlayFabRegistration : MonoBehaviour
{
    // Reference to the input fields for email/username, password and re-enter password.
    public TMP_InputField emailOrUsernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField reEnterPasswordInput;
    public GameObject textFailure;

    // Reference to the register button.
    public Button registerButton;

    public Panel panel;

    private void Start()
    {
        // Add a listener to the register button click event.
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    // Called when the register button is clicked.
    private void OnRegisterButtonClicked()
    {
        // Validate the input fields.
        if (IsValidInput())
        {
            // Attempt to register the user with PlayFab.
            //RegisterPlayFabUser();
            AttemptSignUp();
        }
    }

    // Validates the input fields.
    private bool IsValidInput()
    {
        // Check if any of the fields are empty.
        if (string.IsNullOrEmpty(emailOrUsernameInput.text) ||
            string.IsNullOrEmpty(passwordInput.text) ||
            string.IsNullOrEmpty(reEnterPasswordInput.text))
        {
            Debug.LogError("All fields must be filled out.");
            return false;
        }

        // Check if the passwords match.
        if (passwordInput.text != reEnterPasswordInput.text)
        {
            Debug.LogError("Passwords do not match.");
            return false;
        }

        // Add more validation if needed (e.g., email format, password strength, etc.)

        return true;
    }

    // Registers a new PlayFab user.
    /*
    private void RegisterPlayFabUser()
    {
        RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest
        {
            Username = emailOrUsernameInput.text, // Assuming the username is the same as the email for simplicity.
            Email = emailOrUsernameInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false // You can choose to require both username and email if needed.
        };

        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }
    */
    public void AttemptSignUp()
    {
        string input = emailOrUsernameInput.text;
        string password = passwordInput.text;

        if (IsValidEmail(input))
        {
            RegisterWithEmail(input, password);
        }
        else if (IsValidUsername(input))
        {
            RegisterWithUsername(input, password);
        }
        else
        {
            Debug.LogError("Input is neither a valid email nor a valid username.");
        }
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
    }

    private bool IsValidUsername(string username)
    {
        return Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
    }

    private void RegisterWithEmail(string email, string password)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void RegisterWithUsername(string username, string password)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    // Callback for successful registration.
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registration successful!");
        // Handle successful registration (e.g., navigate to the login screen or main menu).
        panel.TurnOnScene(panel.scenePanel[1]);
    }

    // Callback for failed registration.
    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError($"Registration failed: {error.GenerateErrorReport()}");
        // Handle failed registration (e.g., display error message to the user).
        //textFailure.SetActive(true);
    }
}