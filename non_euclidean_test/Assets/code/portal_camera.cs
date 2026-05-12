using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class portal_camera : MonoBehaviour
{
    
    [SerializeField] private portal[] new_portals = new portal[2];
    [SerializeField] private Camera linked_camera;
    [SerializeField] private int iterations = 7;
    private RenderTexture temp_texture_1;
    private RenderTexture temp_texture_2;
    private Camera main_camera;


    void Awake()
    {
        main_camera = GetComponent<Camera>();
        temp_texture_1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        temp_texture_2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }
    void Start()
    {
        new_portals[0].test_texture = temp_texture_1;
        new_portals[1].test_texture = temp_texture_2;
    }

    void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }
    void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

    void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {

        if (new_portals[0].Renderer.isVisible)
        {
            linked_camera.targetTexture = temp_texture_1;
            for(int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(new_portals[0], new_portals[1], i, SRC);
            }
        }

        if (new_portals[1].Renderer.isVisible)
        {
            linked_camera.targetTexture = temp_texture_2;
            for(int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(new_portals[1], new_portals[0], i, SRC);  
            }
        }
    }
    private void RenderCamera(portal in_portal, portal out_portal, int iteration_ID, ScriptableRenderContext SRC)
    {
        Transform in_transform = in_portal.transform;
        Transform out_transform = out_portal.transform;

        Transform camera_transform = linked_camera.transform;
        camera_transform.position = transform.position;
        camera_transform.rotation = transform.rotation;

        for(int i = 0; i <= iteration_ID; ++i)
        {
            Vector3 relative_pos = in_transform.InverseTransformPoint(camera_transform.position);
            relative_pos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relative_pos;
            camera_transform.position = out_transform.TransformPoint(relative_pos);

            Quaternion relative_rot = Quaternion.Inverse(in_transform.rotation) * camera_transform.rotation;
            relative_rot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relative_rot;
            camera_transform.rotation = out_transform.rotation * relative_rot;
        }

        Plane p = new Plane(-out_transform.forward, out_transform.position);
        Vector4 clip_plane_world_space = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clip_plane_camera_space = 
            Matrix4x4.Transpose(Matrix4x4.Inverse(linked_camera.worldToCameraMatrix)) * clip_plane_world_space;

        var new_matrix = main_camera.CalculateObliqueMatrix(clip_plane_camera_space);
        linked_camera.projectionMatrix = new_matrix;

        //UniversalRenderPipeline.RenderSingleCamera(SRC, linked_camera);
    }
    


}
