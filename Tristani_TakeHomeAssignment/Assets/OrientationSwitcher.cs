using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationSwitcher : MonoBehaviour
{
    // I will do a reletivley primitive solution to solving the Portrait vs landscape issue
    // (This will allow responsivness on all device sizes and orientations)
    // Our canvas already has a scaler that will keep the UI sized within the bounds of the screen
    // However, I want to tackle the aspect ratio issue head on with this method of switching the layout
    // Depending on which aspect ratio we are on, that way we ensure responivness on all devices

    // Holds the canvases for portrait and landscape
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup portraitGroup;
    [SerializeField] private CanvasGroup landscapeGroup;

    private bool lastPortrait;

    void Start()
    {
        // Set the initial layout state
        UpdateLayout(); 
    }

    void Update()
    {
        // Update function will keep the last portrait saved so we dont resign every update
        // This can still be optimized further of course, but for now this is how I'll do it
        bool isPortrait = Screen.height > Screen.width;
        if (isPortrait != lastPortrait)
        {
            lastPortrait = isPortrait;
            UpdateLayout();
        }
    }

    void UpdateLayout()
    {
        // The reason we are not just setting game objects on or off is because I want to still keep references of both layouts at all times
        // So this way, the visibility is just turned off, but we can still programatically interact with both sides
        bool isPortrait = Screen.height > Screen.width;

        SetGroupVisibility(portraitGroup, isPortrait);
        SetGroupVisibility(landscapeGroup, !isPortrait);
    }

    void SetGroupVisibility(CanvasGroup group, bool visible)
    {
        if (group == null) return;

        // This is how we actually enable/disable the visibility and functionality of each side of the UI in the canvasgroups.
        group.alpha = visible ? 1 : 0;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }
}
