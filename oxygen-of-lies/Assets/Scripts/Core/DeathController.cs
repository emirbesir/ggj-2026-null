using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _fadeImage;
    [SerializeField] private CanvasGroup _deathUI;
    
    [Header("Settings")]
    [SerializeField] private float _fadeOutDuration = 2f;
    [SerializeField] private float _respawnDelay = 3f;
    [SerializeField] private bool _reloadSceneOnDeath = true;
    
    private OxygenController _oxygenController;
    private bool _isDead = false;
    
    private void Start()
    {
        _oxygenController = OxygenController.Instance;
        
        if (_oxygenController != null)
        {
            _oxygenController.OnPlayerDeath += OnPlayerDeath;
        }
        
        // Ensure fade starts invisible
        if (_fadeImage != null)
        {
            Color c = _fadeImage.color;
            c.a = 0f;
            _fadeImage.color = c;
        }
        
        if (_deathUI != null)
        {
            _deathUI.alpha = 0f;
            _deathUI.gameObject.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        if (_oxygenController != null)
        {
            _oxygenController.OnPlayerDeath -= OnPlayerDeath;
        }
    }
    
    private void OnPlayerDeath()
    {
        if (_isDead) return;
        
        _isDead = true;
        Debug.Log("[DeathController] Player has died!");
        
        StartCoroutine(DeathSequence());
    }
    
    private IEnumerator DeathSequence()
    {
        // Show death UI
        if (_deathUI != null)
        {
            _deathUI.gameObject.SetActive(true);
            _deathUI.alpha = 1f;
        }
        
        // Fade to black
        float elapsed = 0f;
        
        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _fadeOutDuration;
            
            if (_fadeImage != null)
            {
                Color c = _fadeImage.color;
                c.a = t;
                _fadeImage.color = c;
            }
            
            yield return null;
        }
        
        // Wait before respawn
        yield return new WaitForSeconds(_respawnDelay);
        
        // Reload or respawn
        if (_reloadSceneOnDeath)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
