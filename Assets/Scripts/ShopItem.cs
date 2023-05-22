using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image ballIcon;
    public TMP_Text valueText;
    public Image valueIcon;
    public Sprite moneySprite;
    public Sprite adsSprite;
    public GameObject priceBlock;

    public void SetItem(BallData ball, int index)
    {
        ballIcon.sprite = ball.ballSprite;
        valueText.text = ball.amount.ToString();
        valueIcon.sprite = ball.typeValue == TypeValue.Money ? moneySprite : adsSprite;
        valueIcon.color = ball.typeValue == TypeValue.Money ? Color.yellow : Color.white;

        if (PlayerPrefs.GetInt($"Ball {index} {(int)ball.typeValue} {ball.amount}", 0) == ball.amount)
        {
            priceBlock.SetActive(false);
        }
    }

    public void UpdateCount(int value)
    {
        valueText.text = value.ToString();
    }

    public void DisablePriceBlock()
    {
        priceBlock.SetActive(false);
    }
}
