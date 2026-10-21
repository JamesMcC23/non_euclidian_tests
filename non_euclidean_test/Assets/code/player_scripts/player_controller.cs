using UnityEngine;
using UnityEngine.InputSystem;

public class player_controller : MonoBehaviour
{
    private Rigidbody player_body;
    private InputAction move_action;
    private InputAction jump_action;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //camera controlls
        Vector2 mouse_delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        

        if(Keyboard.current.wKey.isPressed)
        {
            player_body.linearVelocity = player_body.transform.forward * 10;
        }
        else if(Keyboard.current.sKey.isPressed)
        {
            player_body.linearVelocity = player_body.transform.forward * -10;
        }
        //this.transform.rotation = 0;
        Debug.Log(player_body.linearVelocity);

    }


}
