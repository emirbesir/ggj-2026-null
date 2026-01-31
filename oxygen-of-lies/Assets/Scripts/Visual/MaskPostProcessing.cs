using UnityEngine;
using UnityEngine.Rendering;

public class MaskPostProcessing : MonoBehaviour
{
    [Header("Volumes")]
    [SerializeField] private Volume _maskedVolume;     // Bright, peaceful
    [SerializeField] private Volume _realVolume;       // Gritty, desaturated
    
    [Header("Transition Settings")]
    [SerializeField] private float _transitionSpeed = 3f;
    
    private MaskController _maskController;
    private float _targetMaskedWeight = 1f;
    private float _targetRealWeight = 0f;
    
    private void Start()
    {
        _maskController = MaskController.Instance;
        
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged += OnMaskStateChanged;
            
            // Set initial state immediately
            bool isMaskOn = _maskController.IsMaskOn;
            SetTargetWeights(isMaskOn);
            ApplyWeightsImmediate();
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
        LerpWeights();
    }
    
    private void OnMaskStateChanged(bool isMaskOn)
    {
        SetTargetWeights(isMaskOn);
    }
    
    private void SetTargetWeights(bool isMaskOn)
    {
        _targetMaskedWeight = isMaskOn ? 1f : 0f;
        _targetRealWeight = isMaskOn ? 0f : 1f;
    }
    
    private void LerpWeights()
    {
        if (_maskedVolume != null)
        {
            _maskedVolume.weight = Mathf.Lerp(_maskedVolume.weight, _targetMaskedWeight, Time.deltaTime * _transitionSpeed);
        }
        
        if (_realVolume != null)
        {
            _realVolume.weight = Mathf.Lerp(_realVolume.weight, _targetRealWeight, Time.deltaTime * _transitionSpeed);
        }
    }
    
    private void ApplyWeightsImmediate()
    {
        if (_maskedVolume != null)
        {
            _maskedVolume.weight = _targetMaskedWeight;
        }
        
        if (_realVolume != null)
        {
            _realVolume.weight = _targetRealWeight;
        }
    }
}
