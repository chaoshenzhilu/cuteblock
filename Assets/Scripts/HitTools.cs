using System;
using UnityEngine;

/// <summary>
/// 射线检测工具
/// </summary>
public class HitTools
{
    /// <summary>
    /// 按照距离排序
    /// </summary>
    /// <param name="hits"></param>
    public static void SortHits(ref RaycastHit[] hits)
    {
        Array.Sort<RaycastHit>(hits, HitComparison); // 将结果按照远近排序
    }

    private static int HitComparison(RaycastHit a, RaycastHit b)
    {
        if (a.distance <= b.distance)
        {
            return -1;
        }
        return 1;
    }
    public static void SortHits2D(ref RaycastHit2D[] hits)
    {
        Array.Sort<RaycastHit2D>(hits, HitComparison); // 将结果按照远近排序
    }

    private static int HitComparison(RaycastHit2D a, RaycastHit2D b)
    {
        if (a.distance <= b.distance)
        {
            return -1;
        }
        return 1;
    }


    /// <summary>
    /// 射线检测 发从从相机经过鼠标点的射线   返回第一个检测到的物体 （layerName==PlaneLayer 就是返回屏幕的世界坐标）
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="hit"></param>
    /// <param name="layerName"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    public static bool Raycast(Camera camera,out RaycastHit hit,string layerName,Vector3 pos = new Vector3() ,int maxDistance=100)
    {
        if (pos==Vector3.zero)
        {
            pos = Input.mousePosition;
        }
        Ray ray = camera.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit2, maxDistance, 1 << LayerMask.NameToLayer(layerName)))
        {
            hit = hit2;
            return true;
        }
        hit = default;
        return false;
    }
        
    public static RaycastHit2D Raycast2D(Vector3 pos,out RaycastHit2D hit,string layerName,float maxDis=30f)
    {
        hit = Physics2D.Raycast(pos, Vector2.zero,maxDis,1 << LayerMask.NameToLayer(layerName));
        return hit;
    }
        
    public static RaycastHit2D[] Raycast2DAll(Vector3 pos,out RaycastHit2D[] hit,string layerName,float maxDis=30f)
    {
        hit = Physics2D.RaycastAll(pos, Vector2.zero,maxDis,1 << LayerMask.NameToLayer(layerName));
        return hit;
    }
}