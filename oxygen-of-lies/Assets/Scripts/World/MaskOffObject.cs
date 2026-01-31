using UnityEngine;

public class MaskOffObject : MonoBehaviour
{
    [Header("Visual (Only visible when mask OFF)")]
    [SerializeField] private GameObject _bridgeVisual;
    
    private MaskController _maskController;
    
    private void Start()
    {
        _maskController = MaskController.Instance;
        
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged += OnMaskStateChanged;
            UpdateVisibility(_maskController.IsMaskOn);
        }
    }
    
    private void OnDestroy()
    {
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged -= OnMaskStateChanged;
        }
    }
    
    private void OnMaskStateChanged(bool isMaskOn)
    {
        UpdateVisibility(isMaskOn);
    }
    
    private void UpdateVisibility(bool isMaskOn)
    {
        // Visual only shows when mask is OFF (seeing reality)
        if (_bridgeVisual != null)
        {
            _bridgeVisual.SetActive(!isMaskOn);
        }
        
        // Collider is ALWAYS active - you can walk on it even if you can't see it
        // This creates the puzzle: you must remove mask to see where to walk
    }
}
