using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Object that appears/disappears or changes based on a game flag.
/// Useful for doors that open when radioFixed = true, etc.
/// </summary>
public class FlagDependentObject : MonoBehaviour
{
    [Header("Flag Dependency")]
    [SerializeField] private string _watchFlag = "radioFixed";
    [SerializeField] private bool _activeWhenTrue = true;
    
    [Header("Mode")]
    [SerializeField] private Mode _mode = Mode.SetActive;
    
    [Header("Animation (Optional)")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _animTrigger = "Open";
    
    [Header("Events")]
    public UnityEvent OnFlagBecameTrue;
    public UnityEvent OnFlagBecameFalse;
    
    public enum Mode
    {
        SetActive,      // Enable/disable gameObject
        PlayAnimation,  // Trigger animator
        EventOnly       // Just fire events
    }
    
    private bool _lastKnownState = false;
    
    private void Start()
    {
        if (GameFlags.Instance != null)
        {
            GameFlags.Instance.OnFlagChanged += OnFlagChanged;
            
            // Initial state
            bool currentValue = GameFlags.Instance.GetFlag(_watchFlag);
            ApplyState(currentValue, true);
        }
    }
    
    private void OnDestroy()
    {
        if (GameFlags.Instance != null)
        {
            GameFlags.Instance.OnFlagChanged -= OnFlagChanged;
        }
    }
    
    private void OnFlagChanged(string flagName, bool newValue)
    {
        if (flagName != _watchFlag) return;
        
        ApplyState(newValue, false);
    }
    
    private void ApplyState(bool flagValue, bool isInitial)
    {
        bool shouldBeActive = _activeWhenTrue ? flagValue : !flagValue;
        
        switch (_mode)
        {
            case Mode.SetActive:
                gameObject.SetActive(shouldBeActive);
                break;
                
            case Mode.PlayAnimation:
                if (flagValue && !_lastKnownState && _animator != null)
                {
                    _animator.SetTrigger(_animTrigger);
                }
                break;
                
            case Mode.EventOnly:
                // Just events
                break;
        }
        
        // Fire events on change
        if (!isInitial)
        {
            if (flagValue && !_lastKnownState)
                OnFlagBecameTrue?.Invoke();
            else if (!flagValue && _lastKnownState)
                OnFlagBecameFalse?.Invoke();
        }
        
        _lastKnownState = flagValue;
    }
}
