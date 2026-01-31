using UnityEngine;
using UnityEngine.UI;

public class DamageVignetteUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _vignetteImage;      // Red vignette overlay
    [SerializeField] private Image _veinsImage;         // Veins overlay
    
    [Header("Settings")]
    [SerializeField] private float _fadeSpeed = 3f;
    [SerializeField] private float _healthThreshold = 0.7f;  // Start showing below 70% health
    [SerializeField] private float _maxVignetteAlpha = 0.6f;
    [SerializeField] private float _maxVeinsAlpha = 0.8f;
    
    [Header("Pulse Effect")]
    [SerializeField] private float _pulseSpeed = 2f;
    [SerializeField] private float _pulseIntensity = 0.15f;
    
    private OxygenController _oxygenController;
    private float _targetVignetteAlpha = 0f;
    private float _targetVeinsAlpha = 0f;
    private bool _isDamaged = false;
    
    private void Start()
    {
        _oxygenController = OxygenController.Instance;
        
        if (_oxygenController != null)
        {
            _oxygenController.OnHealthChanged += OnHealthChanged;
        }
        
        // Start invisible
        SetAlphaImmediate(_vignetteImage, 0f);
        SetAlphaImmediate(_veinsImage, 0f);
    }
    
    private void OnDestroy()
    {
        if (_oxygenController != null)
        {
            _oxygenController.OnHealthChanged -= OnHealthChanged;
        }
    }
    
    private void Update()
    {
        LerpAlpha(_vignetteImage, _targetVignetteAlpha);
        LerpAlpha(_veinsImage, _targetVeinsAlpha);
        
        // Pulse effect when damaged
        if (_isDamaged && _veinsImage != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * _pulseSpeed) * _pulseIntensity;
            _veinsImage.transform.localScale = Vector3.one * pulse;
        }
    }
    
    private void OnHealthChanged(float healthPercent)
    {
        _isDamaged = healthPercent < _healthThreshold;
        
        if (_isDamaged)
        {
            // Calculate intensity based on how low health is
            float damagePercent = 1f - (healthPercent / _healthThreshold);
            
            _targetVignetteAlpha = Mathf.Lerp(0f, _maxVignetteAlpha, damagePercent);
            _targetVeinsAlpha = Mathf.Lerp(0f, _maxVeinsAlpha, damagePercent);
        }
        else
        {
            _targetVignetteAlpha = 0f;
            _targetVeinsAlpha = 0f;
            
            // Reset scale
            if (_veinsImage != null)
            {
                _veinsImage.transform.localScale = Vector3.one;
            }
        }
    }
    
    private void LerpAlpha(Image image, float targetAlpha)
    {
        if (image == null) return;
        
        Color c = image.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * _fadeSpeed);
        image.color = c;
    }
    
    private void SetAlphaImmediate(Image image, float alpha)
    {
        if (image == null) return;
        
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}
