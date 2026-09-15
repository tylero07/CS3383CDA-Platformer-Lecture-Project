using UnityEngine;

public class NPCmove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float left = 10f;
    public float right = 25f;
    public float speed = 2f;
    private int direction = 1;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        if (transform.position.x >= right)
        {
            direction = -1;
        }
        else if (transform.position.x <= left)
        {
            direction = 1;
        }
    }
}
