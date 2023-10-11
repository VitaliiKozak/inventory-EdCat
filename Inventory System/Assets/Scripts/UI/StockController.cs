using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class StockController : MonoBehaviour
    {
        [SerializeField] private RectTransform _dragItemParent;
        [SerializeField] private RectTransform _cellParent;
        [SerializeField] private Cell _cellPrefab;

        [SerializeField] private int _cellCount;
        
        private List<Cell> _cells;

        private void Awake()
        {
            _cells = new List<Cell>();
        }

        [ContextMenu("TestInit")]
        private void TestInit()
        {
            for (int i = 0; i < _cellCount; i++)
            {
                var cell = Instantiate(_cellPrefab, Vector3.zero, Quaternion.identity, _cellParent);
                _cells.Add(cell);
                cell.name = i.ToString();
                cell.OnBeginDragEvent += BeginDragCallback;
                cell.OnEndDragEvent += EndDragCallback;
                cell.OnDropEvent += DropCallback;
            }
        }

        private void DropCallback(Cell cell, Cell dragged)
        {
            Debug.LogError($"Drag Item from Cell = {dragged.name} To {cell.name}");
        }

        private void EndDragCallback(Cell cell)
        {
            Debug.LogError($"EndDragCallback");
            cell.Item.transform.SetParent(cell.transform);
            cell.SetDragProcess(false);
        }

        private void BeginDragCallback(Cell cell)
        {
            Debug.LogError($"BeginDragCallback");
            cell.Item.transform.SetParent(_dragItemParent);
            cell.SetDragProcess(true);
        }
    }
}