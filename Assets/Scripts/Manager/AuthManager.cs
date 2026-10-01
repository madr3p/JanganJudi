using UnityEngine;
using Supabase.Gotrue;
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

    public async Task<bool> Register(string email, string password, string username)
{
    try
    {
        var session = await Client.Auth.SignUp(email, password);

        if (session?.User == null)
        {
            Debug.LogError("REGISTER FAILED: No user returned.");
            return false;
        }

        var profile = new PlayerProfile
        {
            Id = System.Guid.Parse(session.User.Id),
            Username = username,
            Balance = 1000
        };

        await Client
            .From<PlayerProfile>()
            .Insert(profile);

        Debug.Log("REGISTER SUCCESS");
        Debug.Log("PROFILE CREATED");

        return true;
    }
    catch (System.Exception e)
    {
        Debug.LogError("REGISTER FAILED: " + e.Message);
        return false;
    }
}

    public async Task<bool> Login(string email, string password)
    {
        try
        {
            await Client.Auth.SignIn(email, password);

            Debug.Log("LOGIN SUCCESS");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LOGIN FAILED: " + e.Message);
            return false;
        }
    }

    public async Task Logout()
    {
        await Client.Auth.SignOut();
        Debug.Log("LOGOUT SUCCESS");
    }
}