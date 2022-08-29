using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FreeHandGestureUnity.Widgets
{

    public class FreeHandMessageBoxController : MonoBehaviour
    {
        public const int MAXBUTTONCOUNT = 4;
        public GameObject[] Buttons = new GameObject[MAXBUTTONCOUNT];
        public TMPro.TMP_Text[] ButtonTexts = new TMPro.TMP_Text[MAXBUTTONCOUNT];
        public TMPro.TMP_Text Message;
        public int ButtonCount = MAXBUTTONCOUNT;
        public GameObject Parent;
        private TaskCompletionSource<int> Tcs;
        public void Initialize(int buttonCount)
        {
            int ButtonCount = Math.Min(buttonCount,MAXBUTTONCOUNT);
            if (ButtonCount<=0) ButtonCount = 1;
            switch (ButtonCount)
            {
                case 1:
                    Buttons[1].SetActive(false);
                    Buttons[2].SetActive(false);
                    Buttons[3].SetActive(false);
                    Buttons[0].transform.localPosition = new Vector3(0,-.4f,-.2f);
                    break;
                case 2:
                    Buttons[1].SetActive(true);
                    Buttons[2].SetActive(false);
                    Buttons[3].SetActive(false);
                    Buttons[0].transform.localPosition = new Vector3(-.25f,-.4f,-.2f);
                    Buttons[1].transform.localPosition = new Vector3(.25f,-.4f,-.2f);
                    break;
                case 3:
                    Buttons[1].SetActive(true);
                    Buttons[2].SetActive(true);
                    Buttons[3].SetActive(false);
                    Buttons[0].transform.localPosition = new Vector3(-.25f,-.25f,-.2f);
                    Buttons[1].transform.localPosition = new Vector3(.25f,-.25f,-.2f);
                    Buttons[2].transform.localPosition = new Vector3(0,-.4f,-.2f);
                    break;
                case 4:
                    Buttons[1].SetActive(true);
                    Buttons[2].SetActive(true);
                    Buttons[3].SetActive(true);
                    Buttons[0].transform.localPosition = new Vector3(-.25f,-.25f,-.2f);
                    Buttons[1].transform.localPosition = new Vector3(.25f,-.25f,-.2f);
                    Buttons[2].transform.localPosition = new Vector3(-.25f,-.4f,-.2f);
                    Buttons[3].transform.localPosition = new Vector3(.25f,-.4f,-.2f);
                    break;
            }
        }
        public void ShowMessageBox(TaskCompletionSource<int> tcs, String message, String btn1_text, String btn2_text=null, String btn3_text=null, String btn4_text=null)
        {
            int btncount=1;
            if(btn2_text!=null) btncount++;
            if(btn3_text!=null) btncount++;
            if(btn4_text!=null) btncount++;
            Initialize(btncount);

            Message.text = message;
            ButtonTexts[0].text = btn1_text;
            ButtonTexts[1].text = btn2_text;
            ButtonTexts[2].text = btn3_text;
            ButtonTexts[3].text = btn4_text;

            Tcs=tcs;

            Parent.SetActive(true);
        }

        public void OnClickButton(int index)
        {
            if(Tcs!=null)
            {
                Parent.SetActive(false);
                Tcs.SetResult(index);
            }
        }

    }
}