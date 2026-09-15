using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] private GameManger GameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Win();
        }
    }
}
