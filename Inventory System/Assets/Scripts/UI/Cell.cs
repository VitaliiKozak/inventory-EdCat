using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class Cell : MonoBehaviour, IDropHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public event Action<Cell,Cell> OnDropEvent;
        public event Action<Cell> OnBeginDragEvent;
        public event Action<Cell> OnEndDragEvent;

        [field: SerializeField] public UIItem Item { get; private set; }

        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private GameObject _lockObject;
        [SerializeField] private Image _iconImage;
        public bool IsDragProcess { get; private set; } = false;

        public ISlotInfo SlotInfo{ get; private set; }

        public void Init(ISlotInfo slotInfo)
        {
            SlotInfo = slotInfo;
        }

        public void UpdateData()
        {
            InfoChangeCallback(SlotInfo);
        }
        
        public void Subscribe()
        {
            SlotInfo.OnInfoChangeEvent.AddListener(InfoChangeCallback);
        }

        public void Unsubscribe()
        {
            SlotInfo.OnInfoChangeEvent.RemoveListener(InfoChangeCallback);
        }
        
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
            if (IsDragProcess == false) return;
            OnEndDragEvent?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsDragProcess == false) return;

            Item.transform.position = Input.mousePosition;
        }

        public void SetDragProcess(bool status)
        {
            IsDragProcess = status;
        }
        
        private void InfoChangeCallback(ISlotInfo slotInfo)
        {
            _lockObject.SetActive(slotInfo.IsAvailable == false);
            if (slotInfo.IsFree == true)
            {
                _nameText.text = "Free";
                _countText.text = "";
                _iconImage.enabled = false;
            }
            else
            {
                _nameText.text =slotInfo.Item.Name.ToString();
                _countText.text = slotInfo.Count.ToString();
                _iconImage.enabled = true;
            }
        }
    }
}
