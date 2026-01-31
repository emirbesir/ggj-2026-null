using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaskVisualFeedback : MonoBehaviour
{
    [Header("Screen Overlay")]
    [SerializeField] private Image _screenOverlay;
    [SerializeField] private Color _maskOnColor = new Color(0.9f, 0.95f, 1f, 0.1f);
    [SerializeField] private Color _maskOffColor = new Color(0.2f, 0.15f, 0.1f, 0.3f);
    [SerializeField] private float _colorTransitionDuration = 0.3f;
    
    [Header("Flower Border")]
    [SerializeField] private Image _flowerBorder;
    [SerializeField] private float _borderVisibleAlpha = 0.6f;
    [SerializeField] private float _borderFadeSpeed = 3f;
    
    [Header("Animation Sync")]
    [SerializeField] private float _animationDelay = 0.2f;
    
    private MaskController _maskController;
    private Color _targetColor;
    private Color _currentColor;
    private float _targetBorderAlpha;
    private Coroutine _delayedTransition;
    
    private void Start()
    {
        _maskController = MaskController.Instance;
        
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged += OnMaskStateChanged;
            
            // Set initial state (no delay on start)
            bool isMaskOn = _maskController.IsMaskOn;
            
            _targetColor = isMaskOn ? _maskOnColor : _maskOffColor;
            _currentColor = _targetColor;
            if (_screenOverlay != null) _screenOverlay.color = _currentColor;
            
            _targetBorderAlpha = isMaskOn ? _borderVisibleAlpha : 0f;
            SetBorderAlphaImmediate(_targetBorderAlpha);
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
        // Screen overlay color transition
        if (_screenOverlay != null)
        {
            _currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime / _colorTransitionDuration);
            _screenOverlay.color = _currentColor;
        }
        
        // Flower border alpha transition
        if (_flowerBorder != null)
        {
            Color c = _flowerBorder.color;
            c.a = Mathf.Lerp(c.a, _targetBorderAlpha, Time.deltaTime * _borderFadeSpeed);
            _flowerBorder.color = c;
        }
    }
    
    private void OnMaskStateChanged(bool isMaskOn)
    {
        if (_delayedTransition != null)
        {
            StopCoroutine(_delayedTransition);
        }
        
        _delayedTransition = StartCoroutine(DelayedTransition(isMaskOn));
    }
    
    private IEnumerator DelayedTransition(bool isMaskOn)
    {
        yield return new WaitForSeconds(_animationDelay);
        
        _targetColor = isMaskOn ? _maskOnColor : _maskOffColor;
        _targetBorderAlpha = isMaskOn ? _borderVisibleAlpha : 0f;
        
        _delayedTransition = null;
    }
    
    private void SetBorderAlphaImmediate(float alpha)
    {
        if (_flowerBorder == null) return;
        
        Color c = _flowerBorder.color;
        c.a = alpha;
        _flowerBorder.color = c;
    }
}
