using UnityEngine;
using FMODUnity;

public class WeaponRPlay : MonoBehaviour
{
    public static WeaponRPlay instance {get; private set;}

    private void Awake()
    {
        if (instance != null)
        {
        }
        instance = this;
    }

   public void PlayOneShot(EventReference sound, Vector3 worldPos)
   {
    RuntimeManager.PlayOneShot(sound, worldPos);
   }
}
