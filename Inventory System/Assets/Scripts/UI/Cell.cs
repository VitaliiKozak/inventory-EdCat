using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Cell : MonoBehaviour, IDropHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public event Action<Cell,Cell> OnDropEvent;
        public event Action<Cell> OnBeginDragEvent;
        public event Action<Cell> OnEndDragEvent;

        [field: SerializeField] public UIItem Item { get; private set; }

        private bool _isDragProcess = false;

        public void OnDrop(PointerEventData eventData)
        {
            Debug.LogError("Drop" + eventData.pointerDrag.name);
            if (eventData.pointerDrag.TryGetComponent<Cell>(out var cell) == true)
            {
                OnDropEvent?.Invoke(this,cell);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragEvent?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragEvent?.Invoke(this);
            Item.transform.localPosition = Vector3.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragProcess == false) return;

            Item.transform.position = Input.mousePosition;
        }

        public void SetDragProcess(bool status)
        {
            _isDragProcess = status;
        }
    }
}
