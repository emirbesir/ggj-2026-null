using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Searchable object - press E when masked to see description,
/// hold E when unmasked to search.
/// </summary>
public class Searchable : MonoBehaviour, IInteractable
{
    [Header("Search Settings")]
    [SerializeField] private float _searchDuration = 2f;
    
    [Header("Loot")]
    [SerializeField] private string _grantFlag = "hasAntenna";
    [SerializeField] private bool _hasLoot = true;
    
    [Header("Prompts")]
    [SerializeField] private string _defaultPrompt = "E - İncele";
    [SerializeField] private string _maskedDescription = "Yumuşak bir yastık...";
    [SerializeField] private string _emptyPrompt = "Boş...";
    [SerializeField] private string _foundPrompt = "Bir şey buldun!";
    
    [Header("Events")]
    public UnityEvent OnSearchComplete;
    public UnityEvent OnLootFound;
    
    private bool _isSearched = false;
    private bool _showingMaskedDescription = false;
    private bool _showingFoundPrompt = false;
    
    // IInteractable implementation - dynamic based on mask state
    public bool RequiresHold => !IsMasked() && !_isSearched;
    public float HoldDuration => _searchDuration;
    
    private bool IsMasked() => MaskController.Instance?.IsMaskOn ?? false;
    
    public string GetPrompt(bool isMaskOn)
    {
        if (_showingFoundPrompt) return _hasLoot ? _foundPrompt : _emptyPrompt;
        if (_isSearched) return _emptyPrompt;
        if (_showingMaskedDescription) return _maskedDescription;
        return _defaultPrompt;
    }
    
    public bool CanInteract(bool isMaskOn)
    {
        if (_isSearched) return false;
        return true;
    }
    
    public void OnInteract()
    {
        // Called when pressing E (instant, no hold) - when masked
        if (IsMasked())
        {
            _showingMaskedDescription = true;
            Debug.Log($"[Searchable] Masked description: {_maskedDescription}");
        }
    }
    
    public void OnHoldProgress(float progress)
    {
        _showingMaskedDescription = false;
    }
    
    public void OnHoldComplete()
    {
        if (_isSearched) return;
        
        _isSearched = true;
        _showingMaskedDescription = false;
        _showingFoundPrompt = true;
        
        OnSearchComplete?.Invoke();
        
        if (_hasLoot && !string.IsNullOrEmpty(_grantFlag))
        {
            GameFlags.Instance?.SetFlag(_grantFlag, true);
            OnLootFound?.Invoke();
            Debug.Log($"[Searchable] Found loot! Flag set: {_grantFlag}");
        }
        else
        {
            Debug.Log("[Searchable] No loot found.");
        }
    }
}


