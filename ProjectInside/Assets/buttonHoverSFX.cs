using UnityEngine;
using FMODUnity;
public class buttonHoverSFX : MonoBehaviour
{
    public void PlayHover()
    {
        RuntimeManager.PlayOneShot("event:/UI Hover", Vector3.zero);
    }
}
