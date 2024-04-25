using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Animator startButton;
    public Animator header;
    public Animator settingButton;
    public Animator dialog;
    public Animator contentPanel;
    public Animator gearImage;

    public Slider volumeSdr;

    private void Start()
    {
        LoadVolume();
    }

    public void OpenCloseSettings(bool isOpen)
    {
        startButton.SetBool("isHidden", isOpen);
        header.SetBool("isHidden", isOpen);
        settingButton.SetBool("isHidden", isOpen);
        dialog.SetBool("isHidden", !isOpen);
    }

    public void ToggleMenu()
    {
        bool isHidden = contentPanel.GetBool("isHidden");
        contentPanel.SetBool("isHidden", !isHidden);
        gearImage.SetBool("isHidden", !isHidden);
    }

    public void SoundSetting()
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        AudioSource audi = mainCamera.GetComponent<AudioSource>();
        if (audi.mute)
            audi.mute = false;
        else
            audi.mute = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void SaveVolume()
    {
        float volume = volumeSdr.value;
        // 플레이어 프리팹스로 저장하기 >  배경음악 소리 저장 
        PlayerPrefs.SetFloat("bgVolume", volume);
        PlayerPrefs.Save();
        // 만약 for문으로 돌린다면 Save를 여러번 해줄 필요가 없다. for문 밖에서 마지막으로 한번 저장해주면 됨, 파일에 접근하는 것이라서 성능을 많이 떨어트리는 작업이기 때문
    }
 
    private void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat("bgVolume", 1f);
        volumeSdr.value = volume;
    }
}
