using UnityEngine;
using UnityEngine.UI;

public class FlowerBorderUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _borderImage;
    
    [Header("Settings")]
    [SerializeField] private float _visibleAlpha = 0.6f;
    [SerializeField] private float _invisibleAlpha = 0f;
    [SerializeField] private float _fadeSpeed = 3f;
    
    private MaskController _maskController;
    private float _targetAlpha;
    
    private void Start()
    {
        _maskController = MaskController.Instance;
        
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged += OnMaskStateChanged;
            
            // Set initial state
            _targetAlpha = _maskController.IsMaskOn ? _visibleAlpha : _invisibleAlpha;
            SetAlphaImmediate(_targetAlpha);
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
        if (_borderImage == null) return;
        
        Color c = _borderImage.color;
        c.a = Mathf.Lerp(c.a, _targetAlpha, Time.deltaTime * _fadeSpeed);
        _borderImage.color = c;
    }
    
    private void OnMaskStateChanged(bool isMaskOn)
    {
        _targetAlpha = isMaskOn ? _visibleAlpha : _invisibleAlpha;
    }
    
    private void SetAlphaImmediate(float alpha)
    {
        if (_borderImage == null) return;
        
        Color c = _borderImage.color;
        c.a = alpha;
        _borderImage.color = c;
    }
}
