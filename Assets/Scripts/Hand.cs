using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Boolean m_SqueezeGripAction = null;
    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;
    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();
    private Rigidbody targetBody;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }


    private void Update()
    {
        // Only run update logic if the game is playing to prevent post-game modification issues
        if (!Application.isPlaying)
        {
            return;
        }

        // Clean up any null references in the contact interactables list
         m_ContactInteractables.RemoveAll(item => item == null || item.gameObject == null);

        // Get grab down state
        if(m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "Trigger Down");
            Pickup();
        }
        // Get grab up state
        if(m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "Trigger Up");
            Drop();
        }

        // Get grip down state
        if(m_SqueezeGripAction.GetStateDown(m_Pose.inputSource) && m_CurrentInteractable != null)
        {
            BreakInteractable();
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(!other.gameObject.CompareTag("Interactable"))
            return;

        var interactable = other.gameObject.GetComponent<Interactable>();
        if (interactable != null && !m_ContactInteractables.Contains(interactable))
        {
            m_ContactInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(!other.gameObject.CompareTag("Interactable"))
            return;

        var interactable = other.gameObject.GetComponent<Interactable>();
        if (interactable != null && m_ContactInteractables.Contains(interactable))
        {
            m_ContactInteractables.Remove(interactable);
        }      
    }

    public void Pickup()
    {
        // get nearest interactable
        m_CurrentInteractable = GetNearestInteractable();

        // null check for empty hand
        if(!m_CurrentInteractable || m_CurrentInteractable.m_ActiveHand != null)
            return;

        targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();

        // check if held by other hand, if so, drop it 
        if(m_CurrentInteractable.m_ActiveHand)
            m_CurrentInteractable.m_ActiveHand.Drop();

        // position interactble to controller
        m_CurrentInteractable.transform.position = transform.position;

        // attach to fixed joint;
        m_Joint.connectedBody = targetBody;

        // set active hand
        m_CurrentInteractable.m_ActiveHand = this;         

    }

public void Drop()
{
    if (!m_CurrentInteractable)
        return;

    // Detach from joint
    m_Joint.connectedBody = null;

    if (targetBody != null)
    {
        // Apply velocity and angular velocity to simulate throwing
        targetBody.velocity = -5*m_Pose.GetVelocity();
        targetBody.angularVelocity = m_Pose.GetAngularVelocity();
    }

    // Clear interactable
    m_CurrentInteractable.m_ActiveHand = null;
    m_CurrentInteractable = null;
}

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach(Interactable interactable in m_ContactInteractables)
        {
            // check distance between interactable and hand
            distance = (interactable.transform.position - transform.position).sqrMagnitude;

            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }
        return nearest;
    }
    
    private void BreakInteractable()
    {
        if (m_CurrentInteractable.GetComponent<SimpleBreakable>() != null)
        {
            m_CurrentInteractable.GetComponent<SimpleBreakable>().Break();
            print("Object broken");
        }
    }
}
