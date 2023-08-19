using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Transform controlsPanel;

    private void Awake()
    {
        playButton.onClick.AddListener(() => {
            SceneManager.LoadScene(1);
        });

        controlsButton.onClick.AddListener(() => {
            ShowControlsPanel();
        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });

        HideControlsPanel();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            HideControlsPanel();
        }
    }

    private void ShowControlsPanel()
    {
        controlsPanel.gameObject.SetActive(true);
    }

    private void HideControlsPanel()
    {
        controlsPanel.gameObject.SetActive(false);
    }
}
