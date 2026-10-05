using UnityEngine;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    private Supabase.Client Client => SupabaseManager.Instance.Client;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =========================
    // REGISTER
    // =========================

public async Task<bool> Register(
    string email,
    string password,
    string username)
{
    try
    {
        // Store username inside Supabase Auth metadata.
        var options = new SignUpOptions
        {
            Data = new Dictionary<string, object>
            {
                { "username", username }
            }
        };

        // Create Supabase Auth account.
        var session = await Client.Auth.SignUp(
            email,
            password,
            options
        );

        if (session?.User == null)
        {
            Debug.LogError(
                "REGISTER FAILED: No user returned."
            );

            return false;
        }

        Debug.Log(
            "AUTH USER CREATED: " +
            session.User.Id
        );

        // The database trigger now automatically creates
        // the profiles row when auth.users is created.

        if (Client.Auth.CurrentSession == null)
        {
            Debug.Log(
                "REGISTRATION COMPLETE - " +
                "EMAIL CONFIRMATION REQUIRED."
            );

            return true;
        }

        Debug.Log("REGISTER SUCCESS");

        return true;
    }
    catch (Exception e)
    {
        Debug.LogError(
            "REGISTER FAILED: " +
            e.Message
        );

        return false;
    }
}

    // =========================
    // LOGIN
    // =========================

    public async Task<bool> Login(
        string username,
        string password)
    {
        try
        {
            // Username cannot be queried directly from
            // profiles because the user is not authenticated yet.
            //
            // Use Supabase RPC to find the email belonging
            // to this username.

            var parameters =
                new Dictionary<string, object>
                {
                    {
                        "username_input",
                        username
                    }
                };

            var email = await Client.Rpc<string>(
                "get_email_by_username",
                parameters
            );

            if (string.IsNullOrEmpty(email))
            {
                Debug.LogError(
                    "LOGIN FAILED: Username not found."
                );

                return false;
            }

            Debug.Log(
                "USERNAME FOUND: " +
                username
            );

            // Login using the email found from the username.
            await Client.Auth.SignIn(
                email,
                password
            );

            // Make sure authentication actually succeeded.
            if (Client.Auth.CurrentSession == null)
            {
                Debug.LogError(
                    "LOGIN FAILED: No active session."
                );

                return false;
            }

            Debug.Log("LOGIN SUCCESS");

            // Make sure the player profile exists.
            bool profileReady =
                await PlayerManager.Instance
                    .CreateProfileIfMissing(
                        username,
                        email
                    );

            if (!profileReady)
            {
                Debug.LogError(
                    "LOGIN FAILED: Profile could not be loaded."
                );

                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "LOGIN FAILED: " +
                e.Message
            );

            return false;
        }
    }

    // =========================
    // CREATE PROFILE
    // =========================

    private async Task CreateProfile(
        string userId,
        string username,
        string email)
    {
        var profile = new PlayerProfile
        {
            Id = Guid.Parse(userId),
            Username = username,
            Email = email,
            Balance = 1000
        };

        await Client
            .From<PlayerProfile>()
            .Insert(profile);

        Debug.Log(
            "PROFILE CREATED FOR: " +
            username
        );
    }

    // =========================
    // LOGOUT
    // =========================

    public async Task Logout()
    {
        try
        {
            await Client.Auth.SignOut();

            Debug.Log("LOGOUT SUCCESS");
        }
        catch (Exception e)
        {
            Debug.LogError(
                "LOGOUT FAILED: " +
                e.Message
            );
        }
    }
}