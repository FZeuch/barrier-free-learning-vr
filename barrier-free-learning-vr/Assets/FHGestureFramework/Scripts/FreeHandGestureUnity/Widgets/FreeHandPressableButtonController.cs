using UnityEngine;

namespace FreeHandGestureUnity.Widgets
{

    public class FreeHandPressableButtonController : MonoBehaviour
    {
        public GameObject Button;
        public bool ToggleMode;
        public bool Pressed = false;
        public void OnPress()
        {
            if (ToggleMode && Pressed == true)
            {
                Button.transform.position += new Vector3(0,.005f,0);
                Pressed = false;
            }
            else
            {
                if (Pressed == false) Button.transform.position -= new Vector3(0,.005f,0);
                Pressed = true;
            }
        }
        public void OnRelease()
        {
            if (!ToggleMode && Pressed == true)
            {
                Button.transform.position += new Vector3(0,.005f,0);
                Pressed = false;
            }
        }
    }
}