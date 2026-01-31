using UnityEngine;
using UnityEngine.UI;

public class LungIconUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _lungFillImage;
    [SerializeField] private RectTransform _lungTransform;
    
    [Header("Colors")]
    [SerializeField] private Color _healthyColor = new Color(0.4f, 0.8f, 1f);
    [SerializeField] private Color _lowColor = new Color(1f, 0.6f, 0.3f);
    [SerializeField] private Color _criticalColor = new Color(1f, 0.2f, 0.2f);
    
    [Header("Thresholds")]
    [SerializeField] private float _lowThreshold = 0.4f;
    [SerializeField] private float _criticalThreshold = 0.15f;
    
    [Header("Animation")]
    [SerializeField] private float _breatheSpeed = 2f;
    [SerializeField] private float _breatheIntensity = 0.1f;
    [SerializeField] private float _panicSpeed = 8f;
    [SerializeField] private float _panicIntensity = 0.15f;
    
    private OxygenController _oxygenController;
    private Vector3 _originalScale;
    private bool _isCritical = false;
    
    private void Start()
    {
        if (_lungTransform != null)
        {
            _originalScale = _lungTransform.localScale;
        }
        
        _oxygenController = OxygenController.Instance;
        
        if (_oxygenController != null)
        {
            _oxygenController.OnOxygenChanged += OnOxygenChanged;
            _oxygenController.OnSuffocationStarted += OnSuffocationStarted;
            _oxygenController.OnSuffocationEnded += OnSuffocationEnded;
        }
    }
    
    private void OnDestroy()
    {
        if (_oxygenController != null)
        {
            _oxygenController.OnOxygenChanged -= OnOxygenChanged;
            _oxygenController.OnSuffocationStarted -= OnSuffocationStarted;
            _oxygenController.OnSuffocationEnded -= OnSuffocationEnded;
        }
    }
    
    private void Update()
    {
        AnimateLung();
    }
    
    private void OnOxygenChanged(float oxygenPercent)
    {
        if (_lungFillImage == null) return;
        
        _lungFillImage.fillAmount = oxygenPercent;
        
        // Update color based on oxygen level
        if (oxygenPercent <= _criticalThreshold)
        {
            _lungFillImage.color = _criticalColor;
        }
        else if (oxygenPercent <= _lowThreshold)
        {
            _lungFillImage.color = Color.Lerp(_criticalColor, _lowColor, 
                (oxygenPercent - _criticalThreshold) / (_lowThreshold - _criticalThreshold));
        }
        else
        {
            _lungFillImage.color = Color.Lerp(_lowColor, _healthyColor, 
                (oxygenPercent - _lowThreshold) / (1f - _lowThreshold));
        }
    }
    
    private void OnSuffocationStarted() => _isCritical = true;
    private void OnSuffocationEnded() => _isCritical = false;
    
    private void AnimateLung()
    {
        if (_lungTransform == null) return;
        
        float speed = _isCritical ? _panicSpeed : _breatheSpeed;
        float intensity = _isCritical ? _panicIntensity : _breatheIntensity;
        
        float scale = 1f + Mathf.Sin(Time.time * speed) * intensity;
        _lungTransform.localScale = _originalScale * scale;
    }
}
