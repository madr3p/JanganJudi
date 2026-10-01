using UnityEngine;

[CreateAssetMenu(fileName = "SupabaseSettings", menuName = "JanganJudi/Supabase Settings")]
public class SupabaseSettings : ScriptableObject
{
    public string supabaseUrl;
    public string supabaseAnonKey;
}