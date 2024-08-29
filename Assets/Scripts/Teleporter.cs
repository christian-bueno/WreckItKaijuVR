using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleporter : MonoBehaviour
{
    public GameObject m_Pointer;
    public SteamVR_Action_Boolean m_TeleportAction;
    private SteamVR_Behaviour_Pose m_Pose = null;
    private bool m_HasPosition = false;
    private bool m_IsTeleporting = false;
    private float m_FadeTime = 0.5f;


    private void  Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
    }

    private void Update()
    {

        if (SceneHandler.isGamePaused)  // Check if the game is paused
        {
            return;  // Skip any teleportation updates
        }

        m_HasPosition = UpdatePointer();
        m_Pointer.SetActive(m_HasPosition);

        // Teleport
        if (m_TeleportAction.GetStateUp(m_Pose.inputSource) && m_HasPosition && !m_IsTeleporting)
        {
            TryTeleport();
        }
    }

    private void TryTeleport()
    {
        // Get camera rig and head position
        Transform cameraRig = SteamVR_Render.Top().origin;
        Vector3 headPosition = SteamVR_Render.Top().head.position;
        
        // Figure out the translation 
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        Vector3 translateVector = m_Pointer.transform.position - groundPosition;
        
        // Move
        StartCoroutine(MoveRig(cameraRig, translateVector));

    }

    private IEnumerator MoveRig(Transform cameraRig, Vector3 translation)
    {
        // Set flag m_IsTeleporting
        m_IsTeleporting = true;

        // Fade
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        // Apply translation once fade to black
        yield return new WaitForSeconds(m_FadeTime);
        cameraRig.position += translation;

        // Clear fade
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        // Clear teleport flag
        m_IsTeleporting = false;

    }

    private bool UpdatePointer()
    {
        // Ray from controller
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        // If its a hit
        if(Physics.Raycast(ray, out hit))
        {
            m_Pointer.transform.position = hit.point;
            return true;
        }
        // if not a hit
        return false;
    }
}
