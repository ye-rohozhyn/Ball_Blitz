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

    public void SetItem(BallData ball)
    {
        ballIcon.sprite = ball.ballSprite;
        valueText.text = ball.amount.ToString();
        valueIcon.sprite = ball.typeValue == TypeValue.Money ? moneySprite : adsSprite;
        valueIcon.color = ball.typeValue == TypeValue.Money ? Color.yellow : Color.white;
    }
}
