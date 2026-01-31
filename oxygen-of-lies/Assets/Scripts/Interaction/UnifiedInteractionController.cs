using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Single controller that handles ALL interactable objects.
/// Replaces InteractionController, SearchController, and PuzzleInteractionController.
/// </summary>
public class UnifiedInteractionController : MonoBehaviour
{
    public static UnifiedInteractionController Instance { get; private set; }
    
    [Header("Detection")]
    [SerializeField] private float _maxDistance = 3f;
    [SerializeField] private LayerMask _interactableLayer;
    
    [Header("UI References")]
    [SerializeField] private GameObject _promptPanel;
    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private GameObject _progressBarPanel;
    [SerializeField] private Image _progressFill;
    
    private Camera _playerCamera;
    private MaskController _maskController;
    private PlayerControls _playerControls;
    
    private IInteractable _currentTarget;
    private MonoBehaviour _currentTargetMono; // For null checks
    private float _holdProgress = 0f;
    private bool _isHolding = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _playerControls = new PlayerControls();
        _playerCamera = Camera.main;
    }
    
    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Player.Interact.started += OnInteractStarted;
    }
    
    private void OnDisable()
    {
        _playerControls.Player.Interact.started -= OnInteractStarted;
        _playerControls.Disable();
    }
    
    private void Start()
    {
        _maskController = MaskController.Instance;
        HideUI();
    }
    
    private void Update()
    {
        UpdateRaycast();
        UpdateHolding();
        UpdateUI();
    }
    
    private void UpdateRaycast()
    {
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _interactableLayer))
        {
            // Try to get IInteractable from hit object
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                _currentTarget = interactable;
                _currentTargetMono = hit.collider.GetComponent<MonoBehaviour>();
                return;
            }
        }
        
        // Lost target
        if (_isHolding)
        {
            CancelHold();
        }
        _currentTarget = null;
        _currentTargetMono = null;
    }
    
    private void OnInteractStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_currentTarget == null) return;
        
        bool isMaskOn = _maskController != null && _maskController.IsMaskOn;
        if (!_currentTarget.CanInteract(isMaskOn)) return;
        
        // If requires hold, handled in Update via IsPressed(); else instant interact
        if (!_currentTarget.RequiresHold)
        {
            _currentTarget.OnInteract();
        }
    }
    
    private void UpdateHolding()
    {
        if (_currentTarget == null || _currentTargetMono == null) return;
        if (!_currentTarget.RequiresHold) return;
        
        bool isMaskOn = _maskController != null && _maskController.IsMaskOn;
        if (!_currentTarget.CanInteract(isMaskOn))
        {
            if (_isHolding) CancelHold();
            return;
        }
        
        bool isHoldingE = _playerControls.Player.Interact.IsPressed();
        
        if (isHoldingE)
        {
            if (!_isHolding)
            {
                StartHold();
            }
            
            _holdProgress += Time.deltaTime;
            float normalizedProgress = Mathf.Clamp01(_holdProgress / _currentTarget.HoldDuration);
            _currentTarget.OnHoldProgress(normalizedProgress);
            
            if (_holdProgress >= _currentTarget.HoldDuration)
            {
                CompleteHold();
            }
        }
        else if (_isHolding)
        {
            CancelHold();
        }
    }
    
    private void StartHold()
    {
        _isHolding = true;
        _holdProgress = 0f;
    }
    
    private void CancelHold()
    {
        _isHolding = false;
        _holdProgress = 0f;
    }
    
    private void CompleteHold()
    {
        _currentTarget.OnHoldComplete();
        _isHolding = false;
        _holdProgress = 0f;
    }
    
    private void UpdateUI()
    {
        bool hasTarget = _currentTarget != null && _currentTargetMono != null;
        
        if (_promptPanel != null)
            _promptPanel.SetActive(hasTarget);
        
        if (_progressBarPanel != null)
            _progressBarPanel.SetActive(hasTarget && _isHolding);
        
        if (!hasTarget) return;
        
        // Update prompt
        if (_promptText != null)
        {
            bool isMaskOn = _maskController != null && _maskController.IsMaskOn;
            _promptText.text = _currentTarget.GetPrompt(isMaskOn);
        }
        
        // Update progress bar
        if (_progressFill != null && _isHolding && _currentTarget.RequiresHold)
        {
            _progressFill.fillAmount = _holdProgress / _currentTarget.HoldDuration;
        }
        else if (_progressFill != null)
        {
            _progressFill.fillAmount = 0f;
        }
    }
    
    private void HideUI()
    {
        if (_promptPanel != null) _promptPanel.SetActive(false);
        if (_progressBarPanel != null) _progressBarPanel.SetActive(false);
    }
}
