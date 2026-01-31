using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Puzzle that requires a flag to be true.
/// Implements IInteractable for unified interaction system.
/// </summary>
public class PuzzleTrigger : MonoBehaviour, IInteractable
{
    [Header("Requirements")]
    [SerializeField] private string _requiredFlag = "hasAntenna";
    [SerializeField] private bool _requireMaskOff = false;
    [SerializeField] private bool _consumeFlag = true;
    
    [Header("Puzzle State")]
    [SerializeField] private string _puzzleSolvedFlag = "radioFixed";
    
    [Header("Prompts")]
    [SerializeField] private string _lockedPrompt = "Bir şey eksik...";
    [SerializeField] private string _unlockedPrompt = "E - Kullan";
    [SerializeField] private string _solvedPrompt = "Tamamlandı.";
    
    [Header("Events")]
    public UnityEvent OnPuzzleSolved;
    
    private bool _isSolved = false;
    
    // IInteractable implementation
    public bool RequiresHold => false;
    public float HoldDuration => 0f;
    
    public string GetPrompt(bool isMaskOn)
    {
        if (_isSolved) return _solvedPrompt;
        
        if (_requireMaskOff && isMaskOn) return _lockedPrompt;
        
        bool hasRequirement = string.IsNullOrEmpty(_requiredFlag) || 
                              (GameFlags.Instance?.GetFlag(_requiredFlag) ?? false);
        
        if (!hasRequirement) return _lockedPrompt;
        
        return _unlockedPrompt;
    }
    
    public bool CanInteract(bool isMaskOn)
    {
        if (_isSolved) return false;
        if (_requireMaskOff && isMaskOn) return false;
        
        bool hasRequirement = string.IsNullOrEmpty(_requiredFlag) || 
                              (GameFlags.Instance?.GetFlag(_requiredFlag) ?? false);
        return hasRequirement;
    }
    
    public void OnInteract()
    {
        bool isMaskOn = MaskController.Instance?.IsMaskOn ?? false;
        if (!CanInteract(isMaskOn)) return;
        
        Solve();
    }
    
    public void OnHoldProgress(float progress) { }
    public void OnHoldComplete() { }
    
    private void Solve()
    {
        _isSolved = true;
        
        if (_consumeFlag && !string.IsNullOrEmpty(_requiredFlag) && GameFlags.Instance != null)
        {
            GameFlags.Instance.SetFlag(_requiredFlag, false);
        }
        
        if (!string.IsNullOrEmpty(_puzzleSolvedFlag) && GameFlags.Instance != null)
        {
            GameFlags.Instance.SetFlag(_puzzleSolvedFlag, true);
        }
        
        OnPuzzleSolved?.Invoke();
        Debug.Log($"[PuzzleTrigger] Puzzle solved! {_puzzleSolvedFlag} = true");
    }
}
