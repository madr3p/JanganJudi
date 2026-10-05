using UnityEngine;
using System;
using System.Threading.Tasks;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;


// ========================================
// PLAYER PROFILE MODEL
// ========================================

[Table("profiles")]
public class PlayerProfile : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("username")]
    public string Username { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("balance")]
    public int Balance { get; set; }
}


// ========================================
// PLAYER MANAGER
// ========================================

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public string UserId { get; private set; }
    public string Username { get; private set; }
    public int Balance { get; private set; }

    private Supabase.Client Client =>
        SupabaseManager.Instance.Client;


    // ========================================
    // AWAKE
    // ========================================

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


    // ========================================
    // LOAD PLAYER
    // ========================================

    public async Task<bool> LoadPlayer()
    {
        var user = Client.Auth.CurrentUser;

        if (user == null)
        {
            Debug.LogWarning(
                "NO USER LOGGED IN"
            );

            return false;
        }

        UserId = user.Id.ToString();

        try
        {
            var response = await Client
                .From<PlayerProfile>()
                .Where(x =>
                    x.Id == Guid.Parse(user.Id)
                )
                .Single();

            if (response == null)
            {
                Debug.LogWarning(
                    "PLAYER PROFILE NOT FOUND"
                );

                return false;
            }

            Username = response.Username;
            Balance = response.Balance;

            Debug.Log("PLAYER LOADED");

            Debug.Log(
                "USERNAME: " +
                Username
            );

            Debug.Log(
                "BALANCE: RM" +
                Balance
            );

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "PROFILE LOAD FAILED: " +
                e.Message
            );

            return false;
        }
    }


    // ========================================
    // CREATE PROFILE IF MISSING
    // ========================================

    public async Task<bool> CreateProfileIfMissing(
        string username,
        string email)
    {
        var user = Client.Auth.CurrentUser;

        if (user == null)
        {
            Debug.LogError(
                "CANNOT CREATE PROFILE: " +
                "NO AUTHENTICATED USER."
            );

            return false;
        }

        UserId = user.Id.ToString();

        try
        {
            // First try to load existing profile.
            var response = await Client
                .From<PlayerProfile>()
                .Where(x =>
                    x.Id == Guid.Parse(user.Id)
                )
                .Single();

            if (response != null)
            {
                Username = response.Username;
                Balance = response.Balance;

                Debug.Log(
                    "EXISTING PROFILE LOADED"
                );

                Debug.Log(
                    "USERNAME: " +
                    Username
                );

                Debug.Log(
                    "BALANCE: RM" +
                    Balance
                );

                return true;
            }
        }
        catch
        {
            // Profile does not exist.
            // Continue to create it.
        }

        try
        {
            // Create missing profile.
            var profile = new PlayerProfile
            {
                Id = Guid.Parse(user.Id),
                Username = username,
                Email = email,
                Balance = 1000
            };

            await Client
                .From<PlayerProfile>()
                .Insert(profile);

            Username = username;
            Balance = 1000;

            Debug.Log(
                "NEW PLAYER PROFILE CREATED"
            );

            Debug.Log(
                "USERNAME: " +
                Username
            );

            Debug.Log(
                "BALANCE: RM" +
                Balance
            );

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "PROFILE CREATION FAILED: " +
                e.Message
            );

            return false;
        }
    }
}