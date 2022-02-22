using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplayItem : MonoBehaviour
{
    [SerializeField] private Sprite lifeFull;
    [SerializeField] private Sprite lifeEmpty;

    private int lifeNumber;

    public static LifeDisplayItem Create(GameObject prefab, Transform parent, Vector3 padding, int lifeNumber)
    {
        GameObject lifeDisplayObject = Instantiate(prefab, parent, true);
        lifeDisplayObject.GetComponent<RectTransform>().anchoredPosition = padding;
        //lifeDisplayObject.transform.position = padding;

        LifeDisplayItem lifeDisplayItem = lifeDisplayObject.transform.GetComponent<LifeDisplayItem>();
        lifeDisplayItem.lifeNumber = lifeNumber;
        
        return lifeDisplayItem;
    }

    public void SetSprite(bool isFull)
    {
        GetComponent<Image>().sprite = isFull ? lifeFull : lifeEmpty;
    }

    public int GetLifeNumber() => lifeNumber;
}
