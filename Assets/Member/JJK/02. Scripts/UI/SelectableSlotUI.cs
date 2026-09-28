using UnityEngine;

namespace Member.JJK._02._Scripts.UI
{
    public class SelectableSlotUI : MonoBehaviour
    {
        [SerializeField] private GameObject selectImage;

        public void SetSelected(bool selected)
        {
            if (selectImage != null)
                selectImage.SetActive(selected);
        }
    }
}
