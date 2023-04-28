using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class CuteBlock : MonoBehaviour
{
    /// <summary>
    /// 类型id  
    /// </summary>
    public int cuteId;
    [SerializeField]
    private Transform childContent;
    public List<BlockItem> childList=new List<BlockItem>();

    private PuzzleDragMove _dragMove;
    [SerializeField]
    private Color color;

    private BlockItemHelper blockItemHelper;
    public Color Color
    {
        get => color;
        set => color = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in childContent)
        {
            BlockItem blockItem = child.GetComponent<BlockItem>();
            blockItem.Copy(GameManager.Instance.copyContent);
            childList.Add(blockItem);
        }
        transform.Find("Center")?.gameObject.SetActive(false);
        _dragMove = GetComponent<PuzzleDragMove>();
        _dragMove.MouseDown = MouseDown;
        _dragMove.MouseDrag = MouseDrag;
        _dragMove.MouseUp = MouseUp;
        blockItemHelper = GetComponent<BlockItemHelper>();

    }
    private void MouseDown()
    {
        childList.ForEach((child)=>child.HighSort());
    }
    private List<BlockItem> canClearList = new List<BlockItem>();
    private void MouseDrag()
    {
        BgItem bgItem = GameManager.Instance.GetBgItemByBaseBlock(blockItemHelper.blockItem);
        if (!IsMoveStateChange(bgItem))
        {
            return;
        }
        Debug.Log("位置发生变化");
        map.Clear();
        bool isCanPutDown = GameManager.Instance.CheckIsCanPutDown(this,map);
        if (isCanPutDown)
        {
            GameManager.Instance.ImitatePutDown(map);
            List<BlockItem> list = new List<BlockItem>();
            bool imitateDestroy = GameManager.Instance.ImitateDestroy(this,list);
            if (imitateDestroy)
            {
                canClearList.ForEach(item =>
                {
                    if (!list.Contains(item))
                    {
                        item.ResetColor();
                    }
                });
                canClearList.Clear();
                canClearList.AddRange(list);
            }
            else
            {
                canClearList.ForEach(item => item.ResetColor());
                canClearList.Clear();
            }
        }
        else
        {
            canClearList.ForEach(item => item.ResetColor());
            canClearList.Clear();
            childList.ForEach((child)=>child.CloseTestPutDow());
            GameManager.Instance.ResetAllBlock();
        }
    }


    private Dictionary<Vector2Int, BlockItem> map = new Dictionary<Vector2Int, BlockItem>();
   

    private void MouseUp()
    {
        map.Clear();
        bool isCanPutDown = GameManager.Instance.CheckIsCanPutDown(this,map);
        if (isCanPutDown)
        {
            DestroyCopyItem();
            GameManager.Instance.PutDown(map, () =>
            {
                ResetSortingOrder();
                transform.SetParent(null);
                Destroy(gameObject);
                GameManager.Instance.CreateThreeBlock();
                _dragMove.CloseDrag();
                GameManager.Instance.TryDestroy();
            });
            
            return;
        }
        if (CheckInRightBox())
        {
            return;
        }
        _dragMove.BackOldLocation(ResetSortingOrder);
    }

    private void ResetSortingOrder()
    {
        childList.ForEach((child)=>child.ResetSortingOrder());
    }

    private void DestroyCopyItem()
    {
        childList.ForEach((child)=>child.DestroyCopyItem());

    }
    private bool CheckInRightBox()
    {
        if (HitTools.Raycast2D(transform.position, out RaycastHit2D hit2D, "RightBox"))
        {
            if (hit2D.collider.CompareTag($"RightBox"))
            {
                return GameManager.Instance.PutDownRightBox(this,ResetSortingOrder);
            }
        }
        return false;
    }

    public void ResetPos(Transform box)
    {
        _dragMove.ResetPos(box);
    }

    /// <summary>
    /// 移动状态发生变化 
    /// </summary>
    /// <returns></returns>
    private bool IsMoveStateChange(BgItem bgItem)
    {
        return blockItemHelper.IsMoveStateChange(bgItem);
    }
}
