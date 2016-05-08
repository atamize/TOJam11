using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TextWobble : MonoBehaviour {

	// Use this for initialization
	void Start() {
        var text = GetComponent<Text>();

        DOTween.To(() => text.color, x => text.color = x, new Color(1,1,1,0.5f), 0.5f).SetLoops(-1, LoopType.Yoyo);
	}
}
