using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDropZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Skill card entered drop zone: {other.name}");
    }
}