using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Singleton that holds all game progress flags using a simple Dictionary.
/// </summary>
public class GameFlags : MonoBehaviour
{
    public static GameFlags Instance { get; private set; }
    
    private Dictionary<string, bool> _flags = new Dictionary<string, bool>();
    
    public event Action<string, bool> OnFlagChanged;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void SetFlag(string flagName, bool value)
    {
        _flags[flagName] = value;
        OnFlagChanged?.Invoke(flagName, value);
        Debug.Log($"[GameFlags] {flagName} = {value}");
    }
    
    public bool GetFlag(string flagName)
    {
        return _flags.TryGetValue(flagName, out bool value) && value;
    }
    
    public bool HasFlag(string flagName)
    {
        return _flags.ContainsKey(flagName) && _flags[flagName];
    }
}
