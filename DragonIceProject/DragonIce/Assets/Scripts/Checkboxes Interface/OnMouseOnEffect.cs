using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnMouseOnEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool selected = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selected) { GetComponentInChildren<Text>().fontStyle = FontStyle.BoldAndItalic; }
        else { GetComponentInChildren<Text>().fontStyle = FontStyle.Normal; }
    }
}
