using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Wobble : MonoBehaviour
{
    public float end = 0.5f;
    public float duration = 0.5f;
    public float rot = 10f;

	void Start() {
        transform.DOLocalMoveY(end, duration).SetLoops(-1, LoopType.Yoyo);
	}
}
