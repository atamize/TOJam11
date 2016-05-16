using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Hint : MonoBehaviour
{
	void Start()
    {
        transform.DOScale(new Vector3(0.4f, 0.4f, 1f), 0.5f).SetLoops(-1, LoopType.Yoyo);
	}
}
