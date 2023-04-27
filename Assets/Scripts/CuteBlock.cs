using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

public class CuteBlock : MonoBehaviour
{
    public int index;
    private float previousTime;
    public static int height = 20;
    public static int width = 10;
    private static Transform[,] grid = new Transform[width, height];
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
    
    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //rotate !
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), 90);
            if (!ValidMove())
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
        }


        /*if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
        {
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLines();

                this.enabled = false;
                FindObjectOfType<SpawnTetromino>().NewTetromino();

            }
            previousTime = Time.time;
        }*/
    }

    void CheckForLines()
    {
        for (int i = height-1; i >= 0; i--)
        {
            if(HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    bool HasLine(int i)
    {
        for(int j = 0; j< width; j++)
        {
            if (grid[j, i] == null)
                return false;
        }

        return true;
    }

    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }
    }

    void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if(grid[j,y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }




    void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX, roundedY] = children;
        }
    }

    bool ValidMove()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if(roundedX < 0 || roundedX >= width || roundedY < 0 ||roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
                return false;
        }

        return true;
    }

    public void ResetPos(Transform box)
    {
        _dragMove.ResetPos(box);
    }
}
