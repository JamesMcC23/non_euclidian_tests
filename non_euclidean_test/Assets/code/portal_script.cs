using UnityEngine;
using System.Collections;

public class portal_script : MonoBehaviour
{
    [SerializeField] GameObject linked_portal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnCollisionEnter (Collider other)
    {
        if(other.CompareTag("player"))
        {
            other.transform.position = linked_portal.transform.position;
            //other.transform.rotation.y = linked_portal.transform.rotation.z;
        }

    }
}
