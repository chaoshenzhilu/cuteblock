using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public List<CuteBlock> blockPrefabs;
        public List<CuteBlock> initBlockList = new List<CuteBlock>();
        
        [SerializeField]
        private List<Transform> posList;
        
        [SerializeField]
        private BgGroup bgGroup;
        
        private Dictionary<int, List<BlockItem>> lineMap = new Dictionary<int, List<BlockItem>>();
        private Dictionary<int, List<BlockItem>> rowMap = new Dictionary<int, List<BlockItem>>();
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (Transform pos in posList)
            {
                pos.GetComponent<SpriteRenderer>().enabled = false;
            }
            bgGroup.CreateBlockBg();
            InitBlockListData();
            CreateThreeBlock();
            InitClearMap();
        }

        private void InitBlockListData()
        {
            for (var i = 0; i < blockPrefabs.Count; i++)
            {
                var cuteBlock = blockPrefabs[i];
                var instance = Instantiate(cuteBlock,initContent);
                instance.transform.position=Vector3.one*200;
                instance.transform.localRotation=Quaternion.identity;
                instance.transform.localScale=Vector3.one*0.5f;
                cuteBlock.cuteId = i;
                initBlockList.Add(cuteBlock);
            }
        }
        private void InitClearMap()
        {
            for (int i = 0; i < 8; i++)
            {
                lineMap[i] = new List<BlockItem>();
                rowMap[i] = new List<BlockItem>();
            }
        }

        [SerializeField]
        private CuteBlock _cuteBlock;
        [SerializeField]
        private Transform rightBox;
        public Transform copyContent;
        public Transform initContent;

        public void CreateThreeBlock()
        {
            
            foreach (Transform pos in posList)
            {
                if (pos.childCount > 0)
                {
                    return;
                }
            }
            List<CuteBlock> prefabs = GetThreePrefab();
            for (var i = 0; i < prefabs.Count; i++)
            {
                var cuteBlock = prefabs[i];
                var instance = Instantiate(cuteBlock,posList[i]);
                instance.transform.localPosition=Vector3.zero;
                instance.transform.localRotation=Quaternion.identity;
                instance.transform.localScale=Vector3.one*0.5f;
            }
        }
        
        private List<CuteBlock> GetThreePrefab()
        {
            List<CuteBlock> list = new List<CuteBlock>();
            List<CuteBlock> otherList = new List<CuteBlock>();
            otherList.AddRange(initBlockList);
            foreach (var blockPrefab in initBlockList)
            {
                if (TryPutInCheckerboard(blockPrefab))
                {
                    list.Add(blockPrefab);
                    otherList.Remove(blockPrefab);
                    if (list.Count==3)
                    {
                        break;
                    }
                }
            }
            bgGroup.BgItems.ForEach(item => item.IsCalculate=false);
            int needCount = 3-list.Count;
            if (needCount>0)
            {
                for (int i = 0; i < needCount; i++)
                {
                    int range = Random.Range(0, otherList.Count);
                    CuteBlock cuteBlock = otherList[range];
                    list.Add(cuteBlock);
                    otherList.Remove(cuteBlock);
                }
            }
            return list;
        }

        private bool TryPutInCheckerboard(CuteBlock block)
        {
            foreach (BgItem bgItem in bgGroup.BgItems)
            {
                if (bgItem.IsCalculate)
                {
                    continue;
                }
                BlockItem blockItem = bgGroup.BgGrid[bgItem.pos];
                if (blockItem)
                {
                    continue;
                }
                var itemHelper = block.GetComponent<BlockItemHelper>();
                if (itemHelper.IsCanPutInCheckerboard(bgItem,out List<Vector2Int> list))
                {
                    foreach (Vector2Int vector2Int in list)
                    {
                        bgGroup.BgPos[vector2Int].IsCalculate = true;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool CheckIsCanPutDown(CuteBlock cuteBlock,Dictionary<Vector2Int,BlockItem> map)
        {
            bgGroup.BgItems.ForEach(item =>
            {
                if (item.transform.childCount==0)
                {
                    bgGroup.BgGrid[item.pos] = null;
                }
            });
            
            foreach (BlockItem child in cuteBlock.childList)
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
                    if (bgGroup.BgGrid[bgItem.pos])
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

        public void PutDown(Dictionary<Vector2Int,BlockItem> map,Action action)
        {
            foreach (var keyValuePair in map)
            {
                keyValuePair.Value.pos = keyValuePair.Key;
                bgGroup.BgGrid[keyValuePair.Key] = keyValuePair.Value;
                keyValuePair.Value.transform.SetParent(bgGroup.BgPos[keyValuePair.Key].transform);
                keyValuePair.Value.transform.DOLocalMove(Vector3.zero, 0.1f);
            }
            transform.DOScale(Vector3.one, 0.1f).OnComplete(action.Invoke);
        }

        public bool PutDownRightBox(CuteBlock cuteBlock,Action action)
        {
            if (rightBox.childCount==0)
            {
                _cuteBlock = cuteBlock;
                cuteBlock.transform.SetParent(rightBox);
                _cuteBlock.transform.DOLocalMove(Vector3.zero, 0.2f);
                _cuteBlock.transform.DOScale(Vector3.one*0.5f, 0.2f).OnComplete(action.Invoke);
                _cuteBlock.ResetPos(rightBox);
                CreateThreeBlock();
                return true;
            }
            return false;
        }

        
        public void TryDestroy()
        {
            CheckDestroy();
            DestroyBlock();
        }
        
        
        
        private void ClearLine(BlockItem blockItem,List<BlockItem> waitClearList)
        {
            Vector2 vector2 = blockItem.pos;
            for (int i = 0; i < 8; i++)
            {
                vector2.y = i;
                BlockItem item = bgGroup.BgGrid[vector2];
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
                BlockItem item = bgGroup.BgGrid[vector2];
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
        
        private void DestroyBlock()
        {
            foreach (var keyValuePair in rowMap)
            {
                List<BlockItem> blockItems = keyValuePair.Value;
                foreach (var blockItem in blockItems)
                {
                    if (blockItem)
                    {
                        bgGroup.BgGrid[blockItem.pos] = null;
                        blockItem.transform.parent.DetachChildren();
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
                        bgGroup.BgGrid[blockItem.pos] = null;
                        blockItem.transform.parent.DetachChildren();
                        Destroy(blockItem.gameObject);
                    }
                }
                blockItems.Clear();
            }
        }

        public Vector2 GetPositionByPos(Vector2Int pos)
        {
            return bgGroup.GetPositionByPos(pos);
        }


        /// <summary>
        /// 根据棋盘生成能放下的方块
        /// </summary>
        private void CreateBottomBlock()
        {
            Vector2Int vector2Int = new Vector2Int();
            //横着检测
            
        }

        public bool IsCanPutBlockItem(Vector2Int pos)
        {
            //棋盘外面了
            if (pos.x<0||pos.x>7||pos.y<0||pos.y>7)
            {
                return false;
            }
            if (bgGroup.BgGrid[pos])
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取能消除的方块
        /// </summary>
        private void CheckDestroy()
        {
            foreach (var keyValuePair in rowMap)
            {
                keyValuePair.Value.Clear();
            }
            foreach (var keyValuePair in lineMap)
            {
                keyValuePair.Value.Clear();
            }
            
            
            Vector2 targetPos = new Vector2();
            targetPos.x = 0;
            for (int y = 0; y < 8; y++)
            {
                targetPos.y = y;
                BlockItem blockItem = bgGroup.BgGrid[targetPos];
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
                BlockItem blockItem = bgGroup.BgGrid[targetPos];
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
        }

        /// <summary>
        /// 模拟消除  变色；
        /// </summary>
        public bool ImitateDestroy(CuteBlock cuteBlock,List<BlockItem> canClearList)
        {
            CheckDestroy();
            foreach (var keyValuePair in rowMap)
            {
                List<BlockItem> blockItems = keyValuePair.Value;
                foreach (var blockItem in blockItems)
                {
                    if (blockItem)
                    {
                        blockItem.ChangeColor(cuteBlock.Color);
                        canClearList.Add(blockItem);
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
                        blockItem.ChangeColor(cuteBlock.Color);
                        canClearList.Add(blockItem);
                    }
                }
                blockItems.Clear();
            }
            return canClearList.Count>0;;
        }

        public void ImitatePutDown(Dictionary<Vector2Int,BlockItem> map)
        {
            foreach (var keyValuePair in map)
            {
                keyValuePair.Value.pos = keyValuePair.Key;
                bgGroup.BgGrid[keyValuePair.Key] = keyValuePair.Value;
                keyValuePair.Value.TestPutDow();
            }
        }

        public BgItem GetBgItemByBaseBlock(BlockItem blockItem)
        {
            if (HitTools.Raycast2D(blockItem.transform.position, out RaycastHit2D hit2D, "BgItem"))
            {
                if (hit2D.collider.CompareTag($"BgItem"))
                {
                    BgItem bgItem = hit2D.collider.GetComponent<BgItem>();
                    return bgItem;
                }
            }
            return null;
        }

        public void ResetAllBlock()
        {
            foreach (var keyValuePair in bgGroup.BgGrid)
            {
                if (keyValuePair.Value)
                {
                    keyValuePair.Value.ResetColor();
                }
            }
        }
    }
}