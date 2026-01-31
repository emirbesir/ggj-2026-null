using UnityEngine;
using System;

[RequireComponent(typeof(PlayerInputHandler))]
public class MaskController : MonoBehaviour
{
    public static MaskController Instance { get; private set; }
    
    [Header("Mask Settings")]
    [SerializeField] private bool _startWithMaskOn = true;
    
    private PlayerInputHandler _playerInputHandler;
    
    private bool _isMaskOn;
    public bool IsMaskOn => _isMaskOn;
    
    public event Action<bool> OnMaskStateChanged;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _isMaskOn = _startWithMaskOn;
    }
    
    private void OnEnable()
    {
        _playerInputHandler.OnMaskToggle += HandleMaskToggle;
    }
    
    private void OnDisable()
    {
        _playerInputHandler.OnMaskToggle -= HandleMaskToggle;
    }
    
    private void Start()
    {
        // Fire initial state event so listeners can set up correctly
        OnMaskStateChanged?.Invoke(_isMaskOn);
        
        Debug.Log($"[MaskController] Mask initialized: {(_isMaskOn ? "ON" : "OFF")}");
    }
    
    private void HandleMaskToggle()
    {
        ToggleMask();
    }
    
    public void ToggleMask()
    {
        _isMaskOn = !_isMaskOn;
        OnMaskStateChanged?.Invoke(_isMaskOn);
        
        Debug.Log($"[MaskController] Mask toggled: {(_isMaskOn ? "ON" : "OFF")}");
    }
    
    public void SetMaskState(bool isOn)
    {
        if (_isMaskOn != isOn)
        {
            _isMaskOn = isOn;
            OnMaskStateChanged?.Invoke(_isMaskOn);
            
            Debug.Log($"[MaskController] Mask set to: {(_isMaskOn ? "ON" : "OFF")}");
        }
    }
}
