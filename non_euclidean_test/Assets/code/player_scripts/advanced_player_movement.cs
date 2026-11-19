using UnityEngine;

public class advanced_player_movement : MonoBehaviour
{
    //variables
    #region serialized fields
    [Header("Advanced movement stats")]
    [SerializeField] private float decceleration = 10f;
    [SerializeField] private int max_linear_velocity = 10;

    [Header("Player collisions")]
    [SerializeField] float floor_detection_range = 5f;
    [SerializeField] float wall_detection_radius = 5f;

    #endregion
    #region movement
    private Vector2 movement_direction;
    private bool is_grounded;
    private bool on_wall;
    private Vector3 current_velocity;

    #endregion
    #region misc variables

    #endregion
    #region components
    private player_controller player_stats;
    private Rigidbody player_body;

    #endregion


    void Awake()
    {
        //GetComponent 
        if(GetComponent<player_controller>() != null)
        {
            player_stats = GetComponent<player_controller>();
        }
        else
        {
            Debug.LogWarning("player doesn't have controller script");
        }
        player_body = GetComponent<Rigidbody>();
   
    }



    // Update is called once per frame
    void Update()
    {
        int acceleration_speed = player_stats.movement_speed;
        check_collisions();


        linear_movement(acceleration_speed);
      
    }


    #region movement functions
    private void linear_movement(int acceleration)
    {
        
        float current_speed;
        current_speed = player_body.linearVelocity.magnitude;
        movement_direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        if(movement_direction.magnitude != 0)
        {
            if (current_speed > max_linear_velocity && is_grounded == false)
            {
                player_body.linearVelocity = movement_direction * current_speed;
            }
            else if (current_speed > max_linear_velocity && is_grounded == true)
            {
                
            }
            else
            {
                if(current_speed < max_linear_velocity)
                {
                    current_velocity += new Vector3(movement_direction.x*acceleration,player_body.linearVelocity.y, movement_direction.y*acceleration);
                }
                else
                {
                    current_velocity = movement_direction*max_linear_velocity;
                }
            }

            player_body.linearVelocity = current_velocity;


            
        }


        
    }

    private void check_collisions()
    {
        //ground check
        RaycastHit ground_hit;
        Physics.Raycast(this.transform.position,Vector3.down,out ground_hit,floor_detection_range);
        if(ground_hit.collider != null)
        {
            if(ground_hit.collider.gameObject.CompareTag("floor"))
            {
                is_grounded = true;
            }
            else
            {
                is_grounded = false;
            }
        }
        else
        {
            is_grounded = false;
        }

        //wall check
        RaycastHit wall_hit;
        Physics.SphereCast(this.transform.position,wall_detection_radius,Vector3.forward,out wall_hit,wall_detection_radius);
        if(wall_hit.collider != null)
        {
            if(wall_hit.collider.gameObject.CompareTag("wall"))
            {
                on_wall = true;
            }
            else
            {
                on_wall = false;
            }

            
        }
        else
        {
            on_wall = false;
        }

        

        
        
    }
    private void dash()
    {
        
    }

    #endregion
    #region debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position,Vector3.down*floor_detection_range);
        Gizmos.DrawWireSphere(this.transform.position,wall_detection_radius);
        
    }

    #endregion
}
