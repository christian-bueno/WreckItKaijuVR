using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class SceneHandler : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;
    public GameObject button;
    public static bool isGamePaused = true;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;

        Time.timeScale = 0;
        button.transform.parent.gameObject.SetActive(true);
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {

        
        if (e.target.gameObject == button)  // Check if the start panel was clicked
        {
            StartGame();
        }
        else if(!isGamePaused) // Only allow object interaction if the game is not paused
        {
            ProcessObjectInteraction(sender, e);  // Handle other object interactions
        }
        
    }

    private void StartGame()
    {
        isGamePaused = false;  // Set the game state to not paused
        Time.timeScale = 1;  // Resume game
        button.transform.parent.gameObject.SetActive(false);  // Hide start panel
        Debug.Log("Game Started");
    }

    private void ProcessObjectInteraction(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<SimpleBreakable>())
        {
            e.target.GetComponent<SimpleBreakable>().Break();
            Debug.Log(e.target.name + " was clicked and should break.");
        }
        else
        {
            Debug.Log("Clicked object is not interactable: " + e.target.name);
        }
    }


    public void PointerInside(object sender, PointerEventArgs e)
    {
        if (!isGamePaused && e.target.GetComponent<SimpleBreakable>())
        {
            Debug.Log("Entered breakable object: " + e.target.name);
        }
        else
        {
            Debug.Log("Entered: " + e.target.name);
        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (!isGamePaused && e.target.GetComponent<SimpleBreakable>())  // Only react to exit events if the game is not paused
        {
            Debug.Log("Exited breakable object: " + e.target.name);
        }
        else
        {
            Debug.Log("Exited: " + e.target.name);
        }
    }
}

