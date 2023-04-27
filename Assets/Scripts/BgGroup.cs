using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class BgGroup : MonoBehaviour
    { [SerializeField]
        public GameObject bgPrefab;
        [SerializeField]
        private int tableRow = 8, tableColumn = 8;
        [SerializeField]
        private Vector2 cuteSize = new Vector2(1, 1);

        [SerializeField]
        private Color color1, color2;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Vector2, BlockItem> bgGrid = new Dictionary<Vector2, BlockItem>();
        /// <summary>
        /// 64个格子
        /// </summary>
        private Dictionary<Vector2, BgItem> bgPos = new Dictionary<Vector2, BgItem>();
        public void CreateBlockBg()
        {
            for (int i = 0; i < 64; i++)
            {
                int xIndex = i % tableColumn;
                int yIndex = i / tableColumn;
                Transform bgItem = Instantiate(bgPrefab, transform).transform;
                bgItem.position = GetPositionByIndex(xIndex,yIndex);
                bgItem.GetComponent<SpriteRenderer>().color = (xIndex+yIndex)%2==0? color1: color2;
                bgItem.name = $"{xIndex}_{yIndex}";
                Vector2Int vector2 = new Vector2Int(xIndex, yIndex);
                bgGrid[vector2] = null;
                bgPos[vector2] = bgItem.GetComponent<BgItem>();
                bgItem.GetComponent<BgItem>().pos = vector2;
            }
            InitClearMap();
        }

        private void InitClearMap()
        {
            for (int i = 0; i < 8; i++)
            {
                lineMap[i] = new List<BlockItem>();
                rowMap[i] = new List<BlockItem>();
            }
        }
        
        private Vector3 GetPositionByIndex(int xIndex,int yIndex)
        {
            float x = (xIndex - (tableColumn - 1) / 2f) * (cuteSize.x);
            float y = ((tableRow - 1) / 2f - yIndex) * (cuteSize.y);
            Vector3 pos = new Vector3(x, y,0) + transform.position;
            return pos;
        }

        public bool CheckIsCanPutDown(List<BlockItem> cuteBlockChildList,Dictionary<Vector2Int,BlockItem> map)
        {
            foreach (BlockItem child in cuteBlockChildList)
            {
                if (!IsCanPutDown(child,map))
                {
                    return false;
                }
            }
            return true;
        }
        
        private bool IsCanPutDown(BlockItem child,Dictionary<Vector2Int,BlockItem> map)
        {
            if (HitTools.Raycast2D(child.transform.position, out RaycastHit2D hit2D, "BgItem"))
            {
                if (hit2D.collider.CompareTag($"BgItem"))
                {
                    BgItem bgItem = hit2D.collider.GetComponent<BgItem>();
                    if (bgGrid[bgItem.pos])
                    {
                        return false;
                    }
                    else
                    {
                        map[bgItem.pos] = child;
                        child.pos = bgItem.pos;
                        return true;
                    }
                }
            }
            return false;
        }

        public void PutDown(Dictionary<Vector2Int, BlockItem> map,Action moveEndAction)
        {
            foreach (var keyValuePair in map)
            {
                keyValuePair.Value.pos = keyValuePair.Key;
                bgGrid[keyValuePair.Key] = keyValuePair.Value;
                keyValuePair.Value.transform.SetParent(bgPos[keyValuePair.Key].transform);
                keyValuePair.Value.transform.DOLocalMove(Vector3.zero, 0.1f);
            }
            transform.DOScale(Vector3.one, 0.1f).OnComplete(moveEndAction.Invoke);
        }

        private Dictionary<int, List<BlockItem>> lineMap = new Dictionary<int, List<BlockItem>>();
        private Dictionary<int, List<BlockItem>> rowMap = new Dictionary<int, List<BlockItem>>();
        public void CheckDestroy()
        {
            Vector2 targetPos = new Vector2();
            targetPos.x = 0;
            for (int y = 0; y < 8; y++)
            {
                targetPos.y = y;
                BlockItem blockItem = bgGrid[targetPos];
                if (!blockItem)
                {
                    continue;
                }

                List<BlockItem> clearList = rowMap[y];
                clearList.Add(blockItem);
                ClearRow(blockItem,clearList);
                if (clearList.Count!=8)
                {
                    clearList.Clear();
                }
            }
            targetPos.y = 0;
            for (int x = 0; x < 8; x++)
            {
                targetPos.x = x;
                BlockItem blockItem = bgGrid[targetPos];
                if (!blockItem)
                {
                    continue;
                }
                List<BlockItem> clearList = lineMap[x];
                clearList.Add(blockItem);
                ClearLine(blockItem,clearList);
                if (clearList.Count!=8)
                {
                    clearList.Clear();
                }
            }
            DestroyBlock();
        }
        

        private void DestroyBlock()
        {
            foreach (var keyValuePair in rowMap)
            {
                List<BlockItem> blockItems = keyValuePair.Value;
                foreach (var blockItem in blockItems)
                {
                    if (blockItem)
                    {
                        Destroy(blockItem.gameObject);
                    }
                }
                blockItems.Clear();
            }
            foreach (var keyValuePair in lineMap)
            {
                List<BlockItem> blockItems = keyValuePair.Value;
                foreach (var blockItem in blockItems)
                {
                    if (blockItem)
                    {
                        Destroy(blockItem.gameObject);
                    }
                }
                blockItems.Clear();
            }
        }
        

        private void ClearLine(BlockItem blockItem,List<BlockItem> waitClearList)
        {
            Vector2 vector2 = blockItem.pos;
            for (int i = 0; i < 8; i++)
            {
                vector2.y = i;
                BlockItem item = bgGrid[vector2];
                if (waitClearList.Contains(item))
                {
                    continue;
                }
                if (!item)
                {
                    Debug.Log(  $"竖向检测中断 i={i},vector2={vector2},list={waitClearList.Count}");
                    return;
                }
                waitClearList.Add(item);
            }
        }
        private void ClearRow(BlockItem blockItem,List<BlockItem> waitClearList)
        {
            Vector2 vector2 = blockItem.pos;
            for (int i = 0; i < 8; i++)
            {
                vector2.x = i;
                BlockItem item = bgGrid[vector2];
                if (waitClearList.Contains(item))
                {
                    continue;
                }
                if (!item)
                {
                    Debug.Log(  $"横向检测中断 i={i},vector2={vector2},list={waitClearList.Count}");
                    return;
                }
                waitClearList.Add(item);
            }
        }

        public Vector2 GetPositionByPos(Vector2Int pos)
        {
            if (bgPos.ContainsKey(pos))
            {
                return bgPos[pos].transform.position;
            }
            return Vector2.zero;
        }
    }
    
}