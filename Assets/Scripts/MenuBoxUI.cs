using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MenuBoxUI : MonoBehaviour
{
    [Header("References")]
    public MenuBox menuBox;
    public TextMeshProUGUI cursorText;
    public List<TextMeshProUGUI> optionTexts;

    private int lastIndex = -1;

    void Start()
    {
        for (int i = 0; i < optionTexts.Count; i++)
        {
            if (i < menuBox.options.Count)
                optionTexts[i].text = menuBox.options[i].optionName;
            else
                optionTexts[i].text = "";
        }
    }

    void Update()
    {
        int currentIndex = menuBox.GetCurrentIndex();

        if (currentIndex != lastIndex)
        {
            lastIndex = currentIndex;

            cursorText.rectTransform.anchoredPosition = new Vector2(
                optionTexts[currentIndex].rectTransform.anchoredPosition.x - 80f,
                optionTexts[currentIndex].rectTransform.anchoredPosition.y
);
        }
    }
}