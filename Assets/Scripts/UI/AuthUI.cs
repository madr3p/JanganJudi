using UnityEngine;
using TMPro;

public class AuthUI : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] private TMP_InputField loginUsernameInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private TMP_Text loginStatusText;

    [Header("Register")]
    [SerializeField] private TMP_InputField registerUsernameInput;
    [SerializeField] private TMP_InputField registerEmailInput;
    [SerializeField] private TMP_InputField registerPasswordInput;
    [SerializeField] private TMP_Text registerStatusText;

public async void Login()
{
    string username = loginUsernameInput.text.Trim();
    string password = loginPasswordInput.text;

    if (string.IsNullOrEmpty(username) ||
        string.IsNullOrEmpty(password))
    {
        loginStatusText.text =
            "Please fill in all fields.";

        return;
    }

    loginStatusText.text =
        "Logging in...";

    bool success =
        await AuthManager.Instance.Login(
            username,
            password
        );

    if (success)
    {
        loginStatusText.text =
            "Login successful.";

        Debug.Log(
            "WELCOME " +
            PlayerManager.Instance.Username
        );

        Debug.Log(
            "BALANCE: RM" +
            PlayerManager.Instance.Balance
        );
    }
    else
    {
        loginStatusText.text =
            "Login failed.";
    }
}

    public async void Register()
    {
        string username = registerUsernameInput.text.Trim();
        string email = registerEmailInput.text.Trim();
        string password = registerPasswordInput.text;

        if (string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password))
        {
            registerStatusText.text = "Please fill in all fields.";
            return;
        }

        registerStatusText.text = "Registering...";

        bool success = await AuthManager.Instance.Register(
            email,
            password,
            username
        );

        registerStatusText.text = success
            ? "Registration successful."
            : "Registration failed.";
    }
}