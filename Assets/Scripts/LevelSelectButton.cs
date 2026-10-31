using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private string levelToLoad;
    [SerializeField] private string previousLevel;   // màn phải hoàn thành trước; để trống = màn đầu, luôn mở
    [SerializeField] private GameObject star1, star2, star3;

    private bool isUnlocked;

    private void Start()
    {
        star1.SetActive(PlayerPrefs.HasKey(levelToLoad + "_Star1"));
        star2.SetActive(PlayerPrefs.HasKey(levelToLoad + "_Star2"));
        star3.SetActive(PlayerPrefs.HasKey(levelToLoad + "_Star3"));

        // hoàn thành màn trước = đạt ít nhất 1 sao
        isUnlocked = string.IsNullOrEmpty(previousLevel) || PlayerPrefs.HasKey(previousLevel + "_Star1");

        GetComponentInChildren<Button>().interactable = isUnlocked;

        // bật/tắt mọi object con có tên chứa "lock" (ví dụ Lock-icon)
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name.ToLower().Contains("lock"))
            {
                child.gameObject.SetActive(!isUnlocked);
            }
        }
    }

    public void LoadLevel()
    {
        if (isUnlocked)
        {
            SceneManager.LoadScene(levelToLoad);
        }
    }
}