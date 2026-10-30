using UnityEngine;
using UnityEngine.InputSystem;

public class player_controller : MonoBehaviour
{
    #region serialized fields
    [Header("Movement stats")]
    [SerializeField] private int movement_speed = 10;
    [Header("camera stats")]
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float smoothing = 1.5f;

    #endregion
    #region misc variables
    private Rigidbody player_body;
    private InputAction move_action;
    private InputAction jump_action;

    #endregion

 
    private Camera player_camera;
    private Vector2 frame_velocity;
    private Vector2 velocity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_body = GetComponent<Rigidbody>();
        player_camera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //checks to ensure all components are present 
        if(!player_camera)
        {
            Debug.LogError("No camera is attached to the player");
        }

        //camera controlls
        Vector2 mouse_delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 raw_frame_velocity = Vector2.Scale(mouse_delta, Vector2.one * sensitivity);
        frame_velocity = Vector2.Lerp(frame_velocity, raw_frame_velocity, 1 / smoothing);
        velocity += frame_velocity;
        velocity.y = Mathf.Clamp(velocity.y,-90,90);

        player_camera.transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        this.transform.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);

        if(Keyboard.current.wKey.isPressed)
        {
            player_body.linearVelocity = player_body.transform.forward * movement_speed;
        }
        else if(Keyboard.current.sKey.isPressed)
        {
            player_body.linearVelocity = player_body.transform.forward * -movement_speed;
        }
        else if(Keyboard.current.aKey.isPressed)
        {
            player_body.linearVelocity = player_body.transform.right * -movement_speed;
        }
        else if(Keyboard.current.dKey.isPressed)
        {
            player_body.linearVelocity = player_body.transform.right * movement_speed;
        }
        
        Debug.Log(player_body.linearVelocity);

    }


}
