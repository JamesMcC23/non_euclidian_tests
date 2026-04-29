using UnityEngine;

public class portal_script : MonoBehaviour
{
    [SerializeField] Gameobject linked_portal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private OnCollisionEnter(Collision other)
    {
        if(other.CompareTag("player"))
        {
            other.transform.position = linked_portal.transform.position;
            other.transform.rotation.y = linked_portal.transform.rotation.z;
        }

    }
}
