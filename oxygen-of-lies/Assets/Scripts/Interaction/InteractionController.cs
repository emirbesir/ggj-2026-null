using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InteractionController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _interactionPanel;
    [SerializeField] private TextMeshProUGUI _interactionText;
    [SerializeField] private GameObject _crosshairHighlight;
    
    [Header("Settings")]
    [SerializeField] private float _maxInteractionDistance = 5f;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private float _textDisplayDuration = 3f;
    
    private MaskController _maskController;
    private Camera _playerCamera;
    private Interactable _currentTarget;
    private float _textTimer;
    private PlayerControls _playerControls;
    
    public event Action<Interactable> OnInteract;
    
    private void Awake()
    {
        _playerControls = new PlayerControls();
        _playerCamera = Camera.main;
    }
    
    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Player.Attack.performed += OnInteractPerformed;
    }
    
    private void OnDisable()
    {
        _playerControls.Player.Attack.performed -= OnInteractPerformed;
        _playerControls.Disable();
    }
    
    private void Start()
    {
        _maskController = MaskController.Instance;
        
        if (_interactionPanel != null)
        {
            _interactionPanel.SetActive(false);
        }
        
        if (_crosshairHighlight != null)
        {
            _crosshairHighlight.SetActive(false);
        }
    }
    
    private void Update()
    {
        UpdateRaycast();
        UpdateTextTimer();
    }
    
    private void UpdateRaycast()
    {
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, _maxInteractionDistance, _interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            
            if (interactable != null && hit.distance <= interactable.InteractionDistance)
            {
                _currentTarget = interactable;
                
                if (_crosshairHighlight != null)
                {
                    _crosshairHighlight.SetActive(true);
                }
                return;
            }
        }
        
        _currentTarget = null;
        
        if (_crosshairHighlight != null)
        {
            _crosshairHighlight.SetActive(false);
        }
    }
    
    private void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_currentTarget == null) return;
        
        string description = _currentTarget.GetDescription(_maskController.IsMaskOn);
        ShowText(description);
        
        OnInteract?.Invoke(_currentTarget);
    }
    
    private void ShowText(string text)
    {
        if (_interactionText != null)
        {
            _interactionText.text = text;
        }
        
        if (_interactionPanel != null)
        {
            _interactionPanel.SetActive(true);
        }
        
        _textTimer = _textDisplayDuration;
    }
    
    private void UpdateTextTimer()
    {
        if (_textTimer > 0)
        {
            _textTimer -= Time.deltaTime;
            
            if (_textTimer <= 0 && _interactionPanel != null)
            {
                _interactionPanel.SetActive(false);
            }
        }
    }
    
    public void HideText()
    {
        _textTimer = 0;
        
        if (_interactionPanel != null)
        {
            _interactionPanel.SetActive(false);
        }
    }
}
