using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive 
{
    public class EditSlider : Slider, IEndDragHandler
    {
        public Action<float> EndDrag;

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDrag?.Invoke(value);
        }
    }
}

