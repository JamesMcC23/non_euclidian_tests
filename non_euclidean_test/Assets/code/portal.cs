using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portal : MonoBehaviour
{
    [SerializeField] public portal other_portal {get; private set;}
    private List<portalable_object> portal_objects = new List<portalable_object>();
    public Renderer Renderer {get; private set;}
    private new BoxCollider collider;

    public new Texture test_texture;


    private void Awake()
    {
        test_texture = Renderer.material.mainTexture;
    }


}
