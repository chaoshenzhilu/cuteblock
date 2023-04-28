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

        public Dictionary<Vector2, BlockItem> BgGrid
        {
            get => bgGrid;
            set => bgGrid = value;
        }

        private List<BgItem> bgItems = new List<BgItem>();

        public List<BgItem> BgItems
        {
            get => bgItems;
            set => bgItems = value;
        }

        /// <summary>
        /// 64个格子
        /// </summary>
        private Dictionary<Vector2, BgItem> bgPos = new Dictionary<Vector2, BgItem>();

        public Dictionary<Vector2, BgItem> BgPos
        {
            get => bgPos;
            set => bgPos = value;
        }

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
                BgItem item = bgItem.GetComponent<BgItem>();
                bgPos[vector2] = item;
                item.pos = vector2;
                BgItems.Add(item);
            }
        }

        
        
        private Vector3 GetPositionByIndex(int xIndex,int yIndex)
        {
            float x = (xIndex - (tableColumn - 1) / 2f) * (cuteSize.x);
            float y = ((tableRow - 1) / 2f - yIndex) * (cuteSize.y);
            Vector3 pos = new Vector3(x, y,0) + transform.position;
            return pos;
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