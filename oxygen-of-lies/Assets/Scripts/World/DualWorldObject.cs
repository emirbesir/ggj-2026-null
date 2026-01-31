using UnityEngine;

public class DualWorldObject : MonoBehaviour
{
    [Header("World Representations")]
    [SerializeField] private GameObject _maskedVersion;  // Beautiful/peaceful (mask ON)
    [SerializeField] private GameObject _realVersion;    // Scary/grim (mask OFF)
    
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
        if (_maskedVersion != null)
        {
            _maskedVersion.SetActive(isMaskOn);
        }
        
        if (_realVersion != null)
        {
            _realVersion.SetActive(!isMaskOn);
        }
    }
}
