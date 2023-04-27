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
        public List<GameObject> blockPrefabs;
        [SerializeField]
        private List<Transform> posList;
        
        [SerializeField]
        private BgGroup bgGroup;
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
            CreateThreeBlock();
        }

        [SerializeField]
        private List<int> indexList = new List<int>();
        private CuteBlock _cuteBlock;
        [SerializeField]
        private Transform rightBox;
        public void CreateThreeBlock()
        {
            foreach (Transform pos in posList)
            {
                if (pos.childCount > 0)
                {
                    return;
                }
            }
            List<GameObject> prefabs=blockPrefabs.Where((o, i) =>
            !_cuteBlock||_cuteBlock.index!=i).ToList();
            indexList.Clear();
            for (int i = 0; i < 3; i++)
            {
                int range = Random.Range(0, prefabs.Count);
                var instance = Instantiate(prefabs[range],posList[i]);
                instance.transform.localPosition=Vector3.zero;
                instance.transform.localRotation=Quaternion.identity;
                instance.transform.localScale=Vector3.one*0.5f;
                prefabs.RemoveAt(range);
            }
        }

        public bool CheckIsCanPutDown(CuteBlock cuteBlock,Dictionary<Vector2Int,BlockItem> map)
        {
            return bgGroup.CheckIsCanPutDown(cuteBlock.childList, map);
        }

        public void PutDown(Dictionary<Vector2Int,BlockItem> map,Action action)
        {
            bgGroup.PutDown(map,action);
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

        
        public void CheckDestroy()
        {
            bgGroup.CheckDestroy();
        }

        public Vector2 GetPositionByPos(Vector2Int pos)
        {
            return bgGroup.GetPositionByPos(pos);
        }
    }
}