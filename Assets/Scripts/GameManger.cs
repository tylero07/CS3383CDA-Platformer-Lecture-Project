using UnityEngine;

public class GameManger : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    public bool won { get; private set; }
    public bool fellOutOfBounds { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Win()
    {
        won = true;
        winScreen.SetActive(true);
    }

    // Update is called once per frame
    public void FellOutOfBounds()
    {
        fellOutOfBounds = true;
        
    }
    
}
