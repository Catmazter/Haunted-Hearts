using UnityEngine;
using UnityEngine.UI;

public class TransitionSceneController : MonoBehaviour
{
    [Header("Story Panels")]
    [SerializeField] GameObject level1Panel;
    [SerializeField] GameObject level2Panel;
    [SerializeField] GameObject level3Panel;
    [SerializeField] GameObject winPanel;

    [SerializeField] Button continueButton;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetupUI();
    }

    void SetupUI()
    {
        var sm = SceneManagerScript.instance;

        // Hide all panels first
        level1Panel.SetActive(false);
        level2Panel.SetActive(false);
        level3Panel.SetActive(false);
        winPanel.SetActive(false);

        if (sm.isFinalLevel)
        {
            winPanel.SetActive(true);
            continueButton.gameObject.SetActive(false);
        }
        else
        {
            switch (sm.nextLevelName)
            {
                case "Level 1": level1Panel.SetActive(true); break;
                case "Level 2": level2Panel.SetActive(true); break;
                case "Level 3": level3Panel.SetActive(true); break;
                default: winPanel.SetActive(true); break;
            }

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => sm.LoadNextLevel());
        }
    }
}
