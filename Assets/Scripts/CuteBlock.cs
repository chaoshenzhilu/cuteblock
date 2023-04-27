using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class CuteBlock : MonoBehaviour
{
    public int index;
    [SerializeField]
    private Transform childContent;
    public List<BlockItem> childList=new List<BlockItem>();

    private PuzzleDragMove _dragMove;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in childContent)
        {
            BlockItem blockItem = child.GetComponent<BlockItem>();
            blockItem.Copy();
            childList.Add(blockItem);
        }
        transform.Find("Center")?.gameObject.SetActive(false);
        _dragMove = GetComponent<PuzzleDragMove>();
        _dragMove.MouseDown = MouseDown;
        _dragMove.MouseDrag = MouseDrag;
        _dragMove.MouseUp = MouseUp;
        
    }
    private void MouseDown()
    {
        childList.ForEach((child)=>child.HighSort());
    }
    private void MouseDrag()
    {
        bool isCanPutDown = GameManager.Instance.CheckIsCanPutDown(this,map);
        if (isCanPutDown)
        {
            childList.ForEach((child)=>child.TestPutDow());
        }
        else
        {
            childList.ForEach((child)=>child.CloseTestPutDow());
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
                GameManager.Instance.CheckDestroy();
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
}
