using UnityEngine;
using TMPro;

public class AuthUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text statusText;

    public async void Register()
    {
        string username = usernameInput.text.Trim();
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password))
        {
            statusText.text = "Please fill in all fields.";
            return;
        }

        statusText.text = "Registering...";

        bool success = await AuthManager.Instance.Register(
            email,
            password,
            username
        );

        statusText.text = success
            ? "Registration successful."
            : "Registration failed.";
    }

    public async void Login()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter email and password.";
            return;
        }

        statusText.text = "Logging in...";

        bool success = await AuthManager.Instance.Login(
            email,
            password
        );

        if (success)
        {
            await PlayerManager.Instance.LoadPlayer();
            statusText.text = "Login successful.";
        }
        else
        {
            statusText.text = "Login failed.";
        }
    }
}