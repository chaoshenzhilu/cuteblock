using System;
using DG.Tweening;
using UnityEngine;

public class PuzzleDragMove : MonoBehaviour
{
    private Vector3 lastMousePosition = Vector3.zero;
    private Vector3 initialPosition;
    private bool isMouseDown = false;

    [SerializeField]
    private bool isCanMove = true;

    public Action MouseDown, MouseDrag,MouseUp;
    public bool isMoveToBack = true;
    private void Start()
    {
        initialPosition = transform.position;
    }

    public void OnMouseDown()
    {
        if (!isCanMove)
        {
            return;
        }
        Debug.Log("按下");
        transform.DOKill();
        isMouseDown = true;
        lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastMousePosition = new Vector3(lastMousePosition.x, lastMousePosition.y);
        MouseDown?.Invoke();
        transform.localScale=Vector3.one;
        transform.position =lastMousePosition+ Vector3.up*2;
    }

    public void OnMouseDrag()
    {
        if (!isCanMove)
        {
            return;
        }
        //Debug.Log("拖拽");
        Vector3 newMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newMousePosition = new Vector3(newMousePosition.x, newMousePosition.y);
        Vector3 offset = newMousePosition - lastMousePosition;
        transform.position += offset;
        lastMousePosition = newMousePosition;
        MouseDrag?.Invoke();
    }

    public void OnMouseUp()
    {
        if (!isCanMove)
        {
            return;
        }
        Debug.Log("拖拽结束");
        MouseUp?.Invoke();
    }

    public void BackOldLocation(Action action)
    {
        if (!isMoveToBack)
        {
            return;
        }
        isMoveToBack = true;
        transform.localScale = Vector3.one * 0.5f;
        transform.DOMove(initialPosition, 0.1f).OnComplete(action.Invoke);
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
        isCanMove = true;
    }

    public void ResetPos(Transform box)
    {
        initialPosition = box.transform.position;
    }

    public void CloseDrag()
    {
        isCanMove = false;
    }
}