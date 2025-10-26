using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject settingsPage;
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    public void OpenSettings()
    {
       settingsPage.SetActive(true);
    }
    public void CloseSettings()
    { settingsPage.SetActive(false); }
    public void ExitGame()
    {
        Debug.Log("Çýkýþ butonu týklandý!");
        Application.Quit();
    }
}