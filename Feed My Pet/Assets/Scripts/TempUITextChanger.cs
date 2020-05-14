using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TempUITextChanger : MonoBehaviour
{
    public TextMeshPro text;

    public void Start() {
        text = GetComponent<TextMeshPro>();
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText() {
        int x = 1;

        while (true) {
            text.text = "TYPE <b>!FEED " + x + "</b>";
            x++;
            if (x > 10) x = 1;

            yield return new WaitForSeconds(1f);
        }
    }
}
