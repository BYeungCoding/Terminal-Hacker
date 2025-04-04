using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public int moveSpeed = 4;
    public Rigidbody2D PlayerBody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)){
            PlayerBody.linearVelocity = Vector2.up * moveSpeed;
        }else if(Input.GetKeyUp(KeyCode.W)){
            PlayerBody.linearVelocity = Vector2.zero;
        }
        if (Input.GetKey(KeyCode.A)){
            PlayerBody.linearVelocity = Vector2.left * moveSpeed;
        }else if(Input.GetKeyUp(KeyCode.A)){
            PlayerBody.linearVelocity = Vector2.zero;
        }
        if (Input.GetKey(KeyCode.S)){
            PlayerBody.linearVelocity = Vector2.down * moveSpeed;
        }else if(Input.GetKeyUp(KeyCode.S)){
            PlayerBody.linearVelocity = Vector2.zero;
        }
        if (Input.GetKey(KeyCode.D)){
            PlayerBody.linearVelocity = Vector2.right * moveSpeed;
        }else if(Input.GetKeyUp(KeyCode.D)){
            PlayerBody.linearVelocity = Vector2.zero;
        }
    }
}
