using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.EventSystems;
using DG.Tweening;


namespace InputDeviceDetection
{
    public class MainMenuUIController : MonoBehaviour
    {
        [SerializeField] Button newGameButton;
        [SerializeField] Button continueGameButton;
        [SerializeField] Button quitGameButton;
        [SerializeField] Button StaffButton;

        PlayableDirector director;
        GameObject eventSystem;

        [Header("===== UI Event System =====")]
        [SerializeField]
        private GameObject mainMenuFirst;
        private bool isMouse;
        private bool isPad;

        [Header("===== Staff =====")]
        [SerializeField]
        private Canvas StaffCanvas;
        [SerializeField]
        private RectTransform StaffPanelRectTransform;
        [SerializeField]
        private Button iKnowButton;
        private bool isStaffPanel;

        [Header("===== NewGame =====")]
        [SerializeField]
        private Canvas newGameCanvas;
        [SerializeField]
        private RectTransform newGamePanelRectTransform;
        [SerializeField]
        private Button createNewGameButton;
        [SerializeField]
        private Button dontCreateButton;
        private bool isNewGame;

        [Header("===== Continue Game =====")]
        [SerializeField]
        private Canvas noSaveCanvas;
        [SerializeField]
        private RectTransform noSavePanelRectTransform;
        [SerializeField]
        private Button noSaveOkButton;
        private bool isNoSave;

        [Header("===== SFX =====")]
        [SerializeField]
        private AudioData nomalClickSfx;
        [SerializeField]
        private AudioData openCanvasSfx;
        [SerializeField]
        private AudioData closeCanvasSfx;

        private void OnEnable()
        {
            //Debug.Log("OnEnable");
            newGameButton.onClick.AddListener(OnNewGameButtonClick);
            continueGameButton.onClick.AddListener(OnContinueGameButtonClick);
            quitGameButton.onClick.AddListener(OnQuitGameButtonClick);
            StaffButton.onClick.AddListener(OnStaffButtonClick);
            iKnowButton.onClick.AddListener(OnIKnowButtonClick);
            createNewGameButton.onClick.AddListener(OnCreateNewGameButtonClick);
            dontCreateButton.onClick.AddListener(OnDontCreateButtonClick);
            noSaveOkButton.onClick.AddListener(OnNoGameOKButtonClick);

            director = FindObjectOfType<PlayableDirector>();
            eventSystem = GameObject.Find("EventSystem");
        }

        private void OnDisable()
        {
            newGameButton.onClick.RemoveAllListeners();
            continueGameButton.onClick.RemoveAllListeners();
            quitGameButton.onClick.RemoveAllListeners();
            StaffButton.onClick.RemoveAllListeners();
            iKnowButton.onClick.RemoveAllListeners();
            createNewGameButton.onClick.RemoveAllListeners();
            dontCreateButton.onClick.RemoveAllListeners();
        }

        private void Start()
        {
            Time.timeScale = 1f;
            InputDeviceDetector.OnSwitchToMouse.AddListener(SwitchToMouse);
            InputDeviceDetector.OnSwitchToKeyboard.AddListener(SwitchToKeyboard);
            InputDeviceDetector.OnSwitchToGamepad.AddListener(SwitchToGamepad);
            EventSystem.current.SetSelectedGameObject(mainMenuFirst);
        }

        void PlayTimeline()
        {
            director.Play();
        }

        void OnNewGameButtonClick()
        {
            AudioManager.Instance.PlaySFX(nomalClickSfx);
            //PlayerPrefs.DeleteAll();
            OpenCreateNewGameCanvas();
        }

        void OnContinueGameButtonClick()
        {
            AudioManager.Instance.PlaySFX(nomalClickSfx);
            PlayerSaveData.Instance.loadSence();
            if (SaveManager.Instance.playerSaveData.SceneName != "")
            {
                eventSystem.SetActive(false);
                SceneController.Instance.TransitionToLoadGame();
            }
            else
            {
                OpenNoSaveCanvas();
            }
        }

        void OnQuitGameButtonClick()
        {
            AudioManager.Instance.PlaySFX(nomalClickSfx);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        void OnStaffButtonClick()
        {
            AudioManager.Instance.PlaySFX(nomalClickSfx);
            AudioManager.Instance.PlaySFX(openCanvasSfx);
            isStaffPanel = true;
            StaffCanvas.enabled = true;
            StaffPanelRectTransform.localScale = Vector2.zero;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(iKnowButton.gameObject);
            }
            StaffPanelRectTransform.DOScale(Vector2.one, 0.15f);
        }

        void OnIKnowButtonClick()
        {
            AudioManager.Instance.PlaySFX(nomalClickSfx);
            AudioManager.Instance.PlaySFX(closeCanvasSfx);
            StartCoroutine(CloseStaffPanel());
        }

        void OnCreateNewGameButtonClick()
        {
            AudioManager.Instance.PlaySFX(nomalClickSfx);
            eventSystem.SetActive(false);
            SceneController.Instance.TransitionToFirstLevel();
        }

        void OnDontCreateButtonClick()
        {
            AudioManager.Instance.PlaySFX(nomalClickSfx);
            CloseNewGameCanvas();
        }

        void OnNoGameOKButtonClick()
        {
            AudioManager.Instance.PlaySFX(nomalClickSfx);
            CloseNoSaveCanvas();
        }

        private void OpenCreateNewGameCanvas()
        {
            AudioManager.Instance.PlaySFX(openCanvasSfx);
            isNewGame = true;
            newGameCanvas.enabled = true;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(createNewGameButton.gameObject);
            }
            newGamePanelRectTransform.localScale = Vector2.zero;
            newGamePanelRectTransform.DOScale(Vector2.one, 0.1f);
        }

        private void CloseNewGameCanvas()
        {
            AudioManager.Instance.PlaySFX(closeCanvasSfx);
            isNewGame = false;
            newGamePanelRectTransform.DOScale(Vector2.zero, 0.1f);
            StartCoroutine(CloseNewGamePanel());
        }

        IEnumerator CloseStaffPanel()
        {
            isStaffPanel = false;
            StaffPanelRectTransform.DOScale(Vector2.zero, 0.15f);
            yield return new WaitForSeconds(0.15f);
            StaffCanvas.enabled = false;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuFirst);
            }
            yield break;
        }

        IEnumerator CloseNewGamePanel()
        {
            yield return new WaitForSeconds(0.1f);
            yield return newGameCanvas.enabled = false;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuFirst);
            }
            yield break;
        }

        private void OpenNoSaveCanvas()
        {
            AudioManager.Instance.PlaySFX(openCanvasSfx);
            isNoSave = true;
            noSaveCanvas.enabled = true;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(noSaveOkButton.gameObject);
            }
            noSavePanelRectTransform.localScale = Vector2.zero;
            noSavePanelRectTransform.DOScale(Vector2.one, 0.1f);
        }

        private void CloseNoSaveCanvas()
        {
            AudioManager.Instance.PlaySFX(closeCanvasSfx);
            isNoSave = false;
            noSavePanelRectTransform.DOScale(Vector2.zero, 0.1f);
            StartCoroutine(CloseNoSavePanel());
        }

        IEnumerator CloseNoSavePanel()
        {
            yield return new WaitForSeconds(0.1f);
            yield return noSaveCanvas.enabled = false;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuFirst);
            }
            yield break;
        }

        void SwitchToMouse()
        {
            isPad = false;
            if (!isMouse)
            {
                isMouse = true;
                EventSystem.current.SetSelectedGameObject(null);
            }
            InputDeviceDetector.ShowCursor();
        }

        void SwitchToKeyboard()
        {
            InputDeviceDetector.ShowCursor();
            isMouse = false;
            if (!isPad)
            {
                isPad = true;
                if (isStaffPanel)
                {
                    EventSystem.current.SetSelectedGameObject(iKnowButton.gameObject);
                }
                else if (isNewGame)
                {
                    EventSystem.current.SetSelectedGameObject(createNewGameButton.gameObject);
                }
                else if (isNoSave)
                {
                    EventSystem.current.SetSelectedGameObject(noSaveOkButton.gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(mainMenuFirst);
                }
            }
        }

        void SwitchToGamepad()
        {
            InputDeviceDetector.HideCursor();
            isMouse = false;
            if (!isPad)
            {
                isPad = true;
                EventSystem.current.SetSelectedGameObject(null);
                if (isStaffPanel)
                {
                    EventSystem.current.SetSelectedGameObject(iKnowButton.gameObject);
                }
                else if (isNewGame)
                {
                    EventSystem.current.SetSelectedGameObject(createNewGameButton.gameObject);
                }
                else if (isNoSave)
                {
                    EventSystem.current.SetSelectedGameObject(noSaveOkButton.gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(mainMenuFirst);
                }
            }
        }
    }
}
