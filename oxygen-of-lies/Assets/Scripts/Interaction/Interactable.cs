using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Descriptions")]
    [SerializeField][TextArea(2, 5)] private string _maskedDescription = "A beautiful vase.";
    [SerializeField][TextArea(2, 5)] private string _realDescription = "It's... a skull.";
    
    [Header("Settings")]
    [SerializeField] private float _interactionDistance = 3f;
    
    public string MaskedDescription => _maskedDescription;
    public string RealDescription => _realDescription;
    public float InteractionDistance => _interactionDistance;
    
    public string GetDescription(bool isMaskOn)
    {
        return isMaskOn ? _maskedDescription : _realDescription;
    }
}
