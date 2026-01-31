using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class OxygenPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float _oxygenAmount = 50f;
    [SerializeField] private bool _destroyOnPickup = true;
    [SerializeField] private float _respawnTime = 30f; // Only used if destroyOnPickup is false
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject _visualModel;
    
    private Collider _collider;
    private bool _isActive = true;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        
        OxygenController oxygenController = other.GetComponent<OxygenController>();
        if (oxygenController != null)
        {
            oxygenController.AddOxygen(_oxygenAmount);
            
            if (_destroyOnPickup)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(RespawnRoutine());
            }
        }
    }
    
    private IEnumerator RespawnRoutine()
    {
        _isActive = false;
        
        if (_visualModel != null)
        {
            _visualModel.SetActive(false);
        }
        
        yield return new WaitForSeconds(_respawnTime);
        
        _isActive = true;
        
        if (_visualModel != null)
        {
            _visualModel.SetActive(true);
        }
    }
}
