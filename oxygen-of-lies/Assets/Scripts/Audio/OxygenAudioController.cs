using UnityEngine;

public class OxygenAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _breathingSource;
    [SerializeField] private AudioSource _heartbeatSource;
    [SerializeField] private AudioSource _maskSfxSource;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip _breathingLoop;
    [SerializeField] private AudioClip _heartbeatLoop;
    [SerializeField] private AudioClip _maskOnSound;
    [SerializeField] private AudioClip _maskOffSound;
    
    [Header("Breathing Settings")]
    [SerializeField] private float _minBreathingPitch = 0.8f;
    [SerializeField] private float _maxBreathingPitch = 1.5f;
    [SerializeField] private float _minBreathingVolume = 0.2f;
    [SerializeField] private float _maxBreathingVolume = 1f;
    
    [Header("Heartbeat Settings")]
    [SerializeField] private float _heartbeatStartThreshold = 0.3f; // Start when oxygen below 30%
    [SerializeField] private float _minHeartbeatVolume = 0.3f;
    [SerializeField] private float _maxHeartbeatVolume = 1f;
    
    private OxygenController _oxygenController;
    private MaskController _maskController;
    private bool _isHeartbeatPlaying = false;
    
    private void Start()
    {
        _oxygenController = OxygenController.Instance;
        _maskController = MaskController.Instance;
        
        if (_oxygenController != null)
        {
            _oxygenController.OnOxygenChanged += OnOxygenChanged;
            _oxygenController.OnSuffocationStarted += OnSuffocationStarted;
            _oxygenController.OnSuffocationEnded += OnSuffocationEnded;
        }
        
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged += OnMaskStateChanged;
        }
        
        // Start breathing loop
        if (_breathingSource != null && _breathingLoop != null)
        {
            _breathingSource.clip = _breathingLoop;
            _breathingSource.loop = true;
            _breathingSource.Play();
        }
        
        // Prepare heartbeat (don't play yet)
        if (_heartbeatSource != null && _heartbeatLoop != null)
        {
            _heartbeatSource.clip = _heartbeatLoop;
            _heartbeatSource.loop = true;
            _heartbeatSource.volume = 0f;
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
        
        if (_maskController != null)
        {
            _maskController.OnMaskStateChanged -= OnMaskStateChanged;
        }
    }
    
    private void OnOxygenChanged(float oxygenPercent)
    {
        UpdateBreathingAudio(oxygenPercent);
        UpdateHeartbeatAudio(oxygenPercent);
    }
    
    private void UpdateBreathingAudio(float oxygenPercent)
    {
        if (_breathingSource == null) return;
        
        // Lower oxygen = faster breathing (higher pitch) and louder
        float invertedPercent = 1f - oxygenPercent;
        
        _breathingSource.pitch = Mathf.Lerp(_minBreathingPitch, _maxBreathingPitch, invertedPercent);
        _breathingSource.volume = Mathf.Lerp(_minBreathingVolume, _maxBreathingVolume, invertedPercent);
    }
    
    private void UpdateHeartbeatAudio(float oxygenPercent)
    {
        if (_heartbeatSource == null) return;
        
        bool shouldPlayHeartbeat = oxygenPercent < _heartbeatStartThreshold;
        
        if (shouldPlayHeartbeat && !_isHeartbeatPlaying)
        {
            _heartbeatSource.Play();
            _isHeartbeatPlaying = true;
        }
        else if (!shouldPlayHeartbeat && _isHeartbeatPlaying)
        {
            _heartbeatSource.Stop();
            _isHeartbeatPlaying = false;
        }
        
        if (_isHeartbeatPlaying)
        {
            // Volume increases as oxygen decreases
            float heartbeatIntensity = 1f - (oxygenPercent / _heartbeatStartThreshold);
            _heartbeatSource.volume = Mathf.Lerp(_minHeartbeatVolume, _maxHeartbeatVolume, heartbeatIntensity);
        }
    }
    
    private void OnMaskStateChanged(bool isMaskOn)
    {
        if (_maskSfxSource == null) return;
        
        AudioClip clip = isMaskOn ? _maskOnSound : _maskOffSound;
        
        if (clip != null)
        {
            _maskSfxSource.PlayOneShot(clip);
        }
    }
    
    private void OnSuffocationStarted()
    {
        Debug.Log("[OxygenAudioController] Suffocation started - audio intensifying");
    }
    
    private void OnSuffocationEnded()
    {
        Debug.Log("[OxygenAudioController] Suffocation ended - audio normalizing");
    }
}
