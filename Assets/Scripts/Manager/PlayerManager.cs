using UnityEngine;
using System.Threading.Tasks;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

[Table("profiles")]
public class PlayerProfile : BaseModel
{
    [PrimaryKey("id")]
    public System.Guid Id { get; set; }

    [Column("username")]
    public string Username { get; set; }

    [Column("balance")]
    public int Balance { get; set; }
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public string UserId { get; private set; }
    public string Username { get; private set; }
    public int Balance { get; private set; }

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

    public async Task LoadPlayer()
    {
        var user = Client.Auth.CurrentUser;

        if (user == null)
        {
            Debug.LogWarning("NO USER LOGGED IN");
            return;
        }

        UserId = user.Id.ToString();

        try
        {
            var response = await Client
                .From<PlayerProfile>()
                .Where(x => x.Id == System.Guid.Parse(user.Id))
                .Single();

            Username = response.Username;
            Balance = response.Balance;

            Debug.Log("PLAYER LOADED");
            Debug.Log("USERNAME: " + Username);
            Debug.Log("BALANCE: RM" + Balance);
        }
        catch (System.Exception e)
        {
            Debug.LogError("PROFILE LOAD FAILED: " + e.Message);
        }
    }
}