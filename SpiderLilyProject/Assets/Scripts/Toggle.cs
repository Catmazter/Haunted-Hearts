using UnityEngine;
using UnityEngine.UI;
public class Togglebutton : MonoBehaviour
{
    [SerializeField] private Sprite[] buttonSprites;
    [SerializeField] private Image targetImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
public void ChangeSprite()
    {
        if (targetImage.sprite == buttonSprites[0])
        {
            targetImage.sprite = buttonSprites[1];
        }
        else
        {
            targetImage.sprite = buttonSprites[0];
        }
        return;
    }
}
