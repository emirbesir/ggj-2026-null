using UnityEngine;
using UnityEngine.UI;

public class MaskVisualFeedback : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _screenOverlay;
    
    [Header("Visual Settings")]
    [SerializeField] private Color _maskOnColor = new Color(0.9f, 0.95f, 1f, 0.1f);  // Slight bright/peaceful tint
    [SerializeField] private Color _maskOffColor = new Color(0.2f, 0.15f, 0.1f, 0.3f); // Gritty/dark tint
    [SerializeField] private float _transitionDuration = 0.3f;
    
    private MaskController _maskController;
    private Color _targetColor;
    private Color _currentColor;
    
    private void Start()
    {
        _maskController = MaskController.Instance;
        
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged += OnMaskStateChanged;
            
            // Set initial state
            bool isMaskOn = _maskController.IsMaskOn;
            _targetColor = isMaskOn ? _maskOnColor : _maskOffColor;
            _currentColor = _targetColor;
            _screenOverlay.color = _currentColor;
        }
    }
    
    private void OnDestroy()
    {
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged -= OnMaskStateChanged;
        }
    }
    
    private void Update()
    {
        // Smooth color transition
        _currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime / _transitionDuration);
        _screenOverlay.color = _currentColor;
    }
    
    private void OnMaskStateChanged(bool isMaskOn)
    {
        _targetColor = isMaskOn ? _maskOnColor : _maskOffColor;
    }
}
