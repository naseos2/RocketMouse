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
        // �÷��̾� �����ս��� �����ϱ� >  ������� �Ҹ� ���� 
        PlayerPrefs.SetFloat("bgVolume", volume);
        PlayerPrefs.Save();
        // ���� for������ �����ٸ� Save�� ������ ���� �ʿ䰡 ����. for�� �ۿ��� ���������� �ѹ� �������ָ� ��, ���Ͽ� �����ϴ� ���̶� ������ ���� ����Ʈ���� �۾��̱� ����
    }
 
    private void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat("bgVolume", 1f);
        volumeSdr.value = volume;
    }
}
