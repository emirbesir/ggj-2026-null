/// <summary>
/// Interface for all interactable objects in the game.
/// Searchables, Puzzles, Notes, etc. all implement this.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Get the prompt text to show (e.g., "E - Ara", "E - Kullan")
    /// </summary>
    string GetPrompt(bool isMaskOn);
    
    /// <summary>
    /// Can the player interact with this right now?
    /// </summary>
    bool CanInteract(bool isMaskOn);
    
    /// <summary>
    /// Called when player presses E (instant interaction)
    /// </summary>
    void OnInteract();
    
    /// <summary>
    /// Does this require holding E? (for searchables)
    /// </summary>
    bool RequiresHold { get; }
    
    /// <summary>
    /// Hold duration in seconds (only used if RequiresHold is true)
    /// </summary>
    float HoldDuration { get; }
    
    /// <summary>
    /// Called continuously while holding E
    /// </summary>
    void OnHoldProgress(float progress);
    
    /// <summary>
    /// Called when hold is completed
    /// </summary>
    void OnHoldComplete();
}
