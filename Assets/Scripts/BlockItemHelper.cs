using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class CuteBlockPosBean
    {
        

    }
    public class BlockItemHelper : MonoBehaviour
    {
        /// <summary>
        /// 射线检测的 bgItem；
        /// </summary>
        [SerializeField]
        private BgItem bgItem;
        /// <summary>
        /// 基准点 用于生成时 判断是否是棋盘需要的
        /// </summary>
        public BlockItem blockItem;

        public List<Vector2Int> otherRelativePos;
        public bool IsCanPutInCheckerboard(BgItem bgItem,out List<Vector2Int> list)
        {
            list = new List<Vector2Int>();
            list.Add(bgItem.pos);
            foreach (var vector2Int in otherRelativePos)
            {
                Vector2Int pos = new Vector2Int();
                pos.x = bgItem.pos.x + vector2Int.x;
                pos.y = bgItem.pos.y + vector2Int.y;
                if (!GameManager.Instance.IsCanPutBlockItem(pos))
                {
                    return false;
                }
                list.Add(pos);
            }
            return true;
        }

        
        public bool IsMoveStateChange(BgItem newBgItem)
        {
            bool isChange = newBgItem != bgItem;
            bgItem = newBgItem;
            return isChange;
        }
    }
}