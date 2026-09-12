using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portal : MonoBehaviour
{
    [field: SerializeField] public portal other_portal {get; private set;}
    private List<portalable_object> portal_objects = new List<portalable_object>();
    public MeshRenderer portal_renderer;
    private new BoxCollider collider;


        void Awake()
    {
        portal_renderer = GetComponent<MeshRenderer>();
    }









}
