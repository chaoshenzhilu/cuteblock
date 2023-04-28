using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class BlockItem : MonoBehaviour
    {
        public Vector2Int pos;

        private SpriteRenderer _copyItem;
        private SpriteRenderer _spriteRenderer;
        private Color color;
        public void Copy(Transform copyContent)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            color = _spriteRenderer.color;
            _copyItem = Instantiate(gameObject, copyContent, true).GetComponent<SpriteRenderer>();
            _copyItem.transform.localScale = Vector3.one;
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

        public void ChangeColor(Color cuteBlockColor)
        {
            _spriteRenderer.color = cuteBlockColor;
        }

        public void ResetColor()
        {
            _spriteRenderer.color = color;
        }
    }
}