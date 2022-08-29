using UnityEngine;

namespace FreeHandGestureUnity.Widgets
{
    public class FreeHandCheckboxController : MonoBehaviour
    {
        public Material Unchecked_Material;
        public Material Checked_Material;
        private MeshRenderer _meshRenderer;
        private bool _checked;
        public bool Checked
        {
            get{return _checked;}
            set
            {
                if (value == true)
                {
                    _meshRenderer.material = Checked_Material;
                }
                else
                {
                    _meshRenderer.material = Unchecked_Material;
                }
                _checked = value;
            }
        }
        // Start is called before the first frame update
        void Awake()
        {
            _meshRenderer = gameObject.GetComponent <MeshRenderer>();
        }

        // Update is called once per frame
        public void Toggle()
        {
            Checked = !_checked;
        }
    }
}