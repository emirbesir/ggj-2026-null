using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple examinable object - press E to see description.
/// Implements IInteractable for unified interaction system.
/// </summary>
public class Interactable : MonoBehaviour, IInteractable
{
    [Header("Descriptions")]
    [SerializeField][TextArea(2, 5)] private string _maskedDescription = "A beautiful vase.";
    [SerializeField][TextArea(2, 5)] private string _realDescription = "It's... a skull.";
    
    [Header("Prompts")]
    [SerializeField] private string _defaultPrompt = "E - İncele";
    
    [Header("Events")]
    public UnityEvent OnExamined;
    
    private bool _showingDescription = false;
    
    // IInteractable implementation
    public bool RequiresHold => false;
    public float HoldDuration => 0f;
    
    public string GetPrompt(bool isMaskOn)
    {
        if (_showingDescription)
        {
            return isMaskOn ? _maskedDescription : _realDescription;
        }
        return _defaultPrompt;
    }
    
    public bool CanInteract(bool isMaskOn)
    {
        return true;
    }
    
    public void OnInteract()
    {
        _showingDescription = true;
        OnExamined?.Invoke();
        Debug.Log($"[Interactable] Showing description");
    }
    
    public void OnHoldProgress(float progress) { }
    public void OnHoldComplete() { }
    
    // Reset when player looks away (called by controller)
    public void ResetState()
    {
        _showingDescription = false;
    }
}

