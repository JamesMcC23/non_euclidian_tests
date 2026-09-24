using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class portal_camera : MonoBehaviour
{
    
    [SerializeField] private portal[] new_portals = new portal[2];
    [SerializeField] private Camera linked_camera;
    [field: SerializeField] private Rigidbody player_controller;
    [SerializeField] private int iterations = 7;
    private RenderTexture temp_texture_1;
    private RenderTexture temp_texture_2;
    [SerializeField] private Camera player_camera;
    public portal in_portal;
    public portal out_portal;
    private Plane camera_plane;
    private Vector3 plane_distance;
    [SerializeField] private float near_clip_offset = 0.05f;
    [SerializeField] private float near_clip_limit = 0.2f;
    



    void Awake()
    {
        temp_texture_1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        temp_texture_2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        linked_camera.enabled = false;

        in_portal = new_portals[0];
        out_portal = new_portals[1];

    }
    void Start()
    {
        new_portals[0].portal_renderer.material.mainTexture = temp_texture_1;
        new_portals[1].portal_renderer.material.mainTexture = temp_texture_2;

    }

    void OnEnable()
    {
        linked_camera.ResetProjectionMatrix();
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }
    void OnDisable()
    {
        linked_camera.ResetProjectionMatrix();
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

   void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {

        if (new_portals[0].portal_renderer.isVisible)
        {
            linked_camera.targetTexture = temp_texture_1;
            for(int i = iterations - 1; i >= 0; --i)
            {
                linked_camera.ResetProjectionMatrix();
                RenderCamera(new_portals[0], new_portals[1], i, SRC);              
            }
        }

        if (new_portals[1].portal_renderer.isVisible)
        {
            linked_camera.targetTexture = temp_texture_2;
            for(int i = iterations - 1; i >= 0; --i)
            {
                linked_camera.ResetProjectionMatrix();
                RenderCamera(new_portals[1], new_portals[0], i, SRC);  
            }
        }
        
    }
    
    private void RenderCamera(portal in_portal, portal out_portal, int iteration_ID, ScriptableRenderContext SRC)
    {
        Transform in_transform = in_portal.transform;
        Transform out_transform = out_portal.transform;

        Transform camera_transform = linked_camera.transform;
        camera_transform.position = player_camera.transform.position;
        camera_transform.rotation = player_camera.transform.rotation;
        


        
        for(int i = 0; i <= iteration_ID; ++i)
        {
            Vector3 relative_pos = in_transform.InverseTransformPoint(camera_transform.position);
            relative_pos = Quaternion.Euler(0.0f, 0.0f, 0.0f) * (relative_pos * -1);
            camera_transform.position = out_transform.TransformPoint(relative_pos.x * -1, relative_pos.y, relative_pos.z);

            Quaternion relative_rot = in_transform.rotation * camera_transform.rotation;
            relative_rot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relative_rot;
            camera_transform.rotation = out_transform.rotation * relative_rot;

        }
        
/*
        //initial code
        camera_plane = new Plane(camera_transform.forward, camera_transform.position);
        Vector4 clip_plane_world_space = new Vector4(camera_plane.normal.x, camera_plane.normal.y, camera_plane.normal.z, camera_plane.distance);
        Vector4 clip_plane_camera_space = 
            Matrix4x4.Transpose(Matrix4x4.Inverse(linked_camera.worldToCameraMatrix)) * clip_plane_world_space;

        var new_matrix = player_camera.CalculateObliqueMatrix(clip_plane_camera_space);
        linked_camera.projectionMatrix = new_matrix;
*/
        //reference 2 iteration
        Transform clip_plane = transform;
        int dot = System.Math.Sign (Vector3.Dot (clip_plane.forward, transform.position - linked_camera.transform.position));

        Vector3 camera_space_pos = linked_camera.worldToCameraMatrix.MultiplyPoint (clip_plane.position);
        Vector3 camera_space_normal = linked_camera.worldToCameraMatrix.MultiplyVector (clip_plane.forward) * dot;
        float camera_space_distance = -Vector3.Dot (camera_space_pos, camera_space_normal) + near_clip_offset;

        if (Mathf.Abs (camera_space_distance) > near_clip_limit)
        {
            Vector4 clip_plane_camera_space = new Vector4 (camera_space_normal.x, camera_space_normal.y, camera_space_normal.z, camera_space_distance);
            linked_camera.projectionMatrix = player_camera.CalculateObliqueMatrix (clip_plane_camera_space);
        }
        else
        {
            linked_camera.projectionMatrix = player_camera.projectionMatrix;
        }


        
        
        
    }
}
