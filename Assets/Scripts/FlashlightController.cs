using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    public Light spotlight;
    private bool isLightOn = false;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            isLightOn = !isLightOn;
            if (spotlight != null)
            {
                spotlight.enabled = isLightOn;
                if (AudioManager.instance != null && AudioManager.instance.buttonClick != null) AudioManager.instance.PlaySFX(AudioManager.instance.buttonClick);
            }
        }
    }
}
