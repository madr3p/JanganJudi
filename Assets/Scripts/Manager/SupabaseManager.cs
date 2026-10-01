using UnityEngine;
using Supabase;
using System.Threading.Tasks;

public class SupabaseManager : MonoBehaviour
{
    public static SupabaseManager Instance;

    public Client Client { get; private set; }

    [SerializeField] private SupabaseSettings settings;

    private async void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await Initialize();
    }

    private async Task Initialize()
    {
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false
        };

        Client = new Client(
            settings.supabaseUrl.TrimEnd('/'),
            settings.supabaseAnonKey.Trim(),
            options
        );

        await Client.InitializeAsync();

        Debug.Log("SUPABASE CONNECTED");
    }
}