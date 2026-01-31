using UnityEngine;
using System;

public class OxygenController : MonoBehaviour
{
    public static OxygenController Instance { get; private set; }
    
    [Header("Oxygen Settings")]
    [SerializeField] private float _maxOxygen = 100f;
    [SerializeField] private float _oxygenDrainRate = 10f;  // Per second when mask is OFF
    [SerializeField] private float _oxygenRecoveryRate = 5f; // Per second when mask is ON
    
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _suffocationDamageRate = 15f; // Damage per second when oxygen is depleted
    
    private MaskController _maskController;
    
    private float _currentOxygen;
    private float _currentHealth;
    
    public float CurrentOxygen => _currentOxygen;
    public float MaxOxygen => _maxOxygen;
    public float OxygenPercent => _currentOxygen / _maxOxygen;
    
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public float HealthPercent => _currentHealth / _maxHealth;
    
    public bool IsAlive => _currentHealth > 0;
    public bool IsSuffocating => _currentOxygen <= 0 && !_maskController.IsMaskOn;
    
    public event Action<float> OnOxygenChanged;      // Passes current oxygen percentage (0-1)
    public event Action<float> OnHealthChanged;      // Passes current health percentage (0-1)
    public event Action OnSuffocationStarted;
    public event Action OnSuffocationEnded;
    public event Action OnPlayerDeath;
    
    private bool _wasSuffocating = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        _maskController = MaskController.Instance;
        
        _currentOxygen = _maxOxygen;
        _currentHealth = _maxHealth;
        
        OnOxygenChanged?.Invoke(OxygenPercent);
        OnHealthChanged?.Invoke(HealthPercent);
    }
    
    private void Update()
    {
        if (!IsAlive) return;
        
        UpdateOxygen();
        UpdateHealth();
        CheckSuffocationState();
    }
    
    private void UpdateOxygen()
    {
        if (_maskController.IsMaskOn)
        {
            // Recover oxygen when mask is on
            _currentOxygen = Mathf.Min(_maxOxygen, _currentOxygen + _oxygenRecoveryRate * Time.deltaTime);
        }
        else
        {
            // Drain oxygen when mask is off (exposed to toxic air)
            _currentOxygen = Mathf.Max(0, _currentOxygen - _oxygenDrainRate * Time.deltaTime);
        }
        
        OnOxygenChanged?.Invoke(OxygenPercent);
    }
    
    private void UpdateHealth()
    {
        if (IsSuffocating)
        {
            // Take damage when suffocating
            _currentHealth = Mathf.Max(0, _currentHealth - _suffocationDamageRate * Time.deltaTime);
            OnHealthChanged?.Invoke(HealthPercent);
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
    }
    
    private void CheckSuffocationState()
    {
        bool isSuffocating = IsSuffocating;
        
        if (isSuffocating && !_wasSuffocating)
        {
            OnSuffocationStarted?.Invoke();
            Debug.Log("[OxygenController] Suffocation started!");
        }
        else if (!isSuffocating && _wasSuffocating)
        {
            OnSuffocationEnded?.Invoke();
            Debug.Log("[OxygenController] Suffocation ended.");
        }
        
        _wasSuffocating = isSuffocating;
    }
    
    private void Die()
    {
        Debug.Log("[OxygenController] Player died from suffocation!");
        OnPlayerDeath?.Invoke();
    }
    
    public void AddOxygen(float amount)
    {
        _currentOxygen = Mathf.Min(_maxOxygen, _currentOxygen + amount);
        OnOxygenChanged?.Invoke(OxygenPercent);
        Debug.Log($"[OxygenController] Added {amount} oxygen. Current: {_currentOxygen:F1}");
    }
    
    public void Heal(float amount)
    {
        if (!IsAlive) return;
        
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(HealthPercent);
        Debug.Log($"[OxygenController] Healed {amount}. Current health: {_currentHealth:F1}");
    }
    
    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;
        
        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(HealthPercent);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
}
