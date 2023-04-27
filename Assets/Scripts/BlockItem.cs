using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class BlockItem : MonoBehaviour
    {
        public Vector2Int pos;

        private SpriteRenderer _copyItem;
        private SpriteRenderer _spriteRenderer;
        public void Copy()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _copyItem = Instantiate(gameObject, transform, true).GetComponent<SpriteRenderer>();
            _copyItem.DOFade(0.5f,0);
            _copyItem.gameObject.SetActive(false);
        }

        public void TestPutDow()
        {
            Vector2 vector2 = GameManager.Instance.GetPositionByPos(pos);
            _copyItem.transform.position = vector2;
            _copyItem.gameObject.SetActive(true);
        }

        public void CloseTestPutDow()
        {
            _copyItem.gameObject.SetActive(false);
        }

        public void DestroyCopyItem()
        {
            Destroy(_copyItem.gameObject);
        }

        public void ResetSortingOrder()
        {
            _spriteRenderer.sortingOrder -= 100;
        }

        public void HighSort()
        {
            _spriteRenderer.sortingOrder += 100;
        }
    }
}