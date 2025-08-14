using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider BGMSlider;
    public AudioSource BGM;
    public Animator dialog;
    
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void SetBGMSound()
    {
        BGM.volume = BGMSlider.value;
    }

    public void OpenSettings()
    {
        dialog.SetBool("ishidden", false);
    }
    public void closeSettings()
    {
        dialog.SetBool("ishidden", true);
    }
}
