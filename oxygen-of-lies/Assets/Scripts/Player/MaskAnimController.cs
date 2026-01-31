using UnityEngine;

public class MaskAnimController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _handAnimator;
    
    [Header("Animator Triggers")]
    [SerializeField] private string _maskOnTrigger = "MaskOn";
    [SerializeField] private string _maskOffTrigger = "MaskOff";
    
    private MaskController _maskController;
    
    private void Start()
    {
        _maskController = MaskController.Instance;
        
        if (_handAnimator == null)
        {
            _handAnimator = GetComponent<Animator>();
        }
        
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged += OnMaskStateChanged;
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
        if (_handAnimator == null) return;
        
        if (isMaskOn)
        {
            _handAnimator.SetTrigger(_maskOnTrigger);
        }
        else
        {
            _handAnimator.SetTrigger(_maskOffTrigger);
        }
    }
}
