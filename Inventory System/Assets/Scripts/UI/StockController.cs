using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class StockController : MonoBehaviour
    {
        [SerializeField] private RectTransform _dragItemParent;
        [SerializeField] private RectTransform _cellParent;
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private TestInventoryController _inventoryController;
        [SerializeField] private SlotTag _undragedSlot;
        private List<Cell> _cells;

        private void Awake()
        {
            _cells = new List<Cell>();
        }

        [ContextMenu("TestInit")]
        private void TestInit()
        {
            for (int i = 0; i < _inventoryController.InventoryController.Inventory.Slots.Count; i++)
            {
                var cell = Instantiate(_cellPrefab, Vector3.zero, Quaternion.identity, _cellParent);
                _cells.Add(cell);
                cell.name = i.ToString();
                cell.OnBeginDragEvent += BeginDragCallback;
                cell.OnEndDragEvent += EndDragCallback;
                cell.OnDropEvent += DropCallback;
                cell.Init(_inventoryController.InventoryController.Inventory.Slots[i]);
                cell.Subscribe();
                cell.UpdateData();
            }
        }

        private void DropCallback(Cell cell, Cell dragged)
        {
            Debug.LogError($"Drop Item");
            if(dragged.IsDragProcess == false) return;
            Debug.LogError($"Drop Item from Cell = {dragged.name} To {cell.name}");
            _inventoryController.InventoryController.MoveItemTo(dragged.SlotInfo.Item.Name, cell.SlotInfo.Id);
        }

        private void EndDragCallback(Cell cell)
        {
            Debug.LogError($"EndDragCallback");
            cell.Item.transform.SetParent(cell.transform);
            cell.SetDragProcess(false);
            cell.Item.transform.localPosition = Vector3.zero;
        }

        private void BeginDragCallback(Cell cell)
        {
            if(cell.SlotInfo.IsFree == true || cell.SlotInfo.IsAvailable == false) return;
            if(cell.SlotInfo.Tags.HasFlag(_undragedSlot) == true) return;
            Debug.LogError($"BeginDragCallback");
            cell.Item.transform.SetParent(_dragItemParent);
            cell.SetDragProcess(true);
        }
    }
}