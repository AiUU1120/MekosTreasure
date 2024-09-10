using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace InputDeviceDetection
{
    public class GameplayUIController : Singleton<GameplayUIController>
    {
        [SerializeField]
        private PlayerInput input;
        [SerializeField]
        private Canvas hUDCanvas;

        [Header("===== Player Panel UI")]
        [SerializeField]
        private Canvas playerPanelCanvas;
        [SerializeField]
        private RectTransform playerPanelRectTransform;
        [SerializeField]
        private float transitionTime;

        [Header("===== Player Skill UI")]
        [SerializeField]
        private Canvas dashUI;

        [Header("===== UI Event System =====")]
        [SerializeField]
        private GameObject inventoryBagFirst;
        [SerializeField]
        private GameObject shopAFirst;
        [SerializeField]
        private GameObject shopBFirst;
        [SerializeField]
        private GameObject shopCFirst;
        private bool isMouse;
        public bool isPad;
        public bool isUI;//是否在UI界面的游戏中
        public bool isBag;//是否在背包界面
        public bool isShop;//是否在商店界面
        public bool isShopA;
        public bool isShopB;
        public bool isShopC;
        public bool isSecondaryMenu;

        [Header("===== Shop UI")]
        [SerializeField]
        private Canvas shopACanvas;
        [SerializeField]
        private RectTransform shopAPanelRectTransform;
        [SerializeField]
        private Canvas shopBCanvas;
        [SerializeField]
        private RectTransform shopBPanelRectTransform;
        [SerializeField]
        private Canvas shopCCanvas;
        [SerializeField]
        private RectTransform shopCPanelRectTransform;
        [SerializeField]
        private Canvas doSuccessCanvas;
        [SerializeField]
        private RectTransform doSuccessPanelTransform;
        [SerializeField]
        private GameObject doSuccessButton;
        [SerializeField]
        private GameObject doSuccessText;
        [SerializeField]
        private GameObject doFailText;
        private bool isdoSuccess;

        [Header("===== Cat House =====")]
        [SerializeField]
        private Canvas catHouseCanvas;
        [SerializeField]
        private RectTransform catHousePanelRectTransform;
        [SerializeField]
        private GameObject catHouseFirst;
        public bool isCatHouse;

        [Header("===== Esc Menu")]
        [SerializeField]
        private Canvas escMenuCanvas;
        [SerializeField]
        private RectTransform escMenuPanelRectTransform;
        [SerializeField]
        private GameObject escMenuFirst;
        public bool isEscMenu;

        [Header("===== Boss UI =====")]
        [SerializeField]
        private GameObject felkoHpBar;

        [Header("===== Dialogue Basic Element =====")]
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Text speakerName;
        [SerializeField]
        private Text mainText;

        [SerializeField]
        private Canvas dialogueCanvas;
        [SerializeField]
        private RectTransform standPanelTransform;
        [SerializeField]
        private RectTransform dialogueTransform;
        private bool canContinue;
        private bool canNextText;

        [Header("===== Dialogue Data =====")]
        [SerializeField]
        private DialogueData_SO currentData;

        private int currentIndex = 0;

        [Header("===== UI SFX =====")]
        [SerializeField]
        private AudioData openCanvasSfx;
        [SerializeField]
        private AudioData normalClickSfx;
        [SerializeField]
        private AudioData closeCanvasSfx;
        [SerializeField]
        private AudioData failSfx;

        private void Start()
        {
            input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
            InputDeviceDetector.OnSwitchToMouse.AddListener(SwitchToMouse);
            InputDeviceDetector.OnSwitchToKeyboard.AddListener(SwitchToKeyboard);
            InputDeviceDetector.OnSwitchToGamepad.AddListener(SwitchToGamepad);

        }

        private void Update()
        {
            //开关角色面板（背包）
            if (input.playerPanel && !isUI)
            {
                OpenPlayerPanel();
            }
            if ((input.closePanel || input.goBack) && !isSecondaryMenu && isBag)
            {
                ClosePlayerPanel();
            }
            //打开Esc菜单
            if(input.EscMenu && !isUI)
            {
                OpenEscMenuCanvas();
            }

            if (canContinue && input.dialogueContinue && canNextText)
            {
                if (currentIndex < currentData.dialoguePieces.Count)
                {
                    UpdateMainDialogue(currentData.dialoguePieces[currentIndex]);
                }
                else
                {
                    canContinue = false;
                    CLoseMainDialogue(currentData.dialoguePieces[0]);
                    DialogueFinishEvent(currentData.dialoguePieces[0]);
                }
            }
            //显示冲刺CD 此处未来可优化
            if (GameManager.Instance.playStats.CanDash)
            {
                dashUI.enabled = true;
            }
            else
            {
                dashUI.enabled = false;
            }
        }
        #region PlayerPanel
        private void OpenPlayerPanel()
        {
            //Time.timeScale = 0f;
            AudioManager.Instance.PlaySFX(openCanvasSfx);
            isUI = true;
            isBag = true;
            hUDCanvas.enabled = true;
            playerPanelCanvas.enabled = true;
            playerPanelRectTransform.localScale = Vector3.zero;
            playerPanelRectTransform.DOScale(Vector3.one, transitionTime);
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(inventoryBagFirst);
            }
            input.EnablePlayerPanelInputs();
        }

        private void ClosePlayerPanel()
        {
            AudioManager.Instance.PlaySFX(closeCanvasSfx);
            isUI = false;
            isBag = false;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            StartCoroutine(ClosePanel());
        }

        IEnumerator ClosePanel()
        {
            Time.timeScale = 1f;
            hUDCanvas.enabled = true;
            playerPanelRectTransform.DOScale(Vector3.zero, transitionTime);
            yield return new WaitForSeconds(transitionTime);
            playerPanelCanvas.enabled = false;
            input.EnableGamePlayInputs();
            yield break;
        }
        #endregion

        #region Shop
        public void OpenShop(ShopIndex shopIndex)
        {
            AudioManager.Instance.PlaySFX(openCanvasSfx);
            isUI = true;
            isShop = true;
            hUDCanvas.enabled = true;
            if (shopIndex == ShopIndex.A)
            {
                isShopA = true;
                shopACanvas.enabled = true;
                shopAPanelRectTransform.localScale = Vector3.zero;
                shopAPanelRectTransform.DOScale(Vector3.one, transitionTime);
            }
            else if (shopIndex == ShopIndex.B)
            {
                isShopB = true;
                shopBCanvas.enabled = true;
                shopBPanelRectTransform.localScale = Vector3.zero;
                shopBPanelRectTransform.DOScale(Vector3.one, transitionTime);
            }
            else if (shopIndex == ShopIndex.C)
            {
                isShopC = true;
                shopCCanvas.enabled = true;
                shopCPanelRectTransform.localScale = Vector3.zero;
                shopCPanelRectTransform.DOScale(Vector3.one, transitionTime);
            }
            if (isPad)
            {
                if (isShopA)
                {
                    EventSystem.current.SetSelectedGameObject(shopAFirst);
                }
                else if (isShopB)
                {
                    EventSystem.current.SetSelectedGameObject(shopBFirst);
                }
                else if (isShopC)
                {
                    EventSystem.current.SetSelectedGameObject(shopCFirst);
                }
            }
            input.EnablePlayerPanelInputs();
        }

        public void CloseShop()
        {
            AudioManager.Instance.PlaySFX(closeCanvasSfx);
            isUI = false;
            isShop = false;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            if (isShopA)
            {
                StartCoroutine(CloseShopAPanel());
            }
            else if (isShopB)
            {
                StartCoroutine(CloseShopBPanel());
            }
            else if (isShopC)
            {
                StartCoroutine(CloseShopCPanel());
            }
            isShopA = false;
            isShopB = false;
            isShopC = false;
        }
        #endregion

        #region IEnumerator Colse Shop Canvas
        IEnumerator CloseShopAPanel()
        {
            Time.timeScale = 1f;
            hUDCanvas.enabled = true;
            shopAPanelRectTransform.DOScale(Vector3.zero, transitionTime);
            yield return new WaitForSeconds(transitionTime);
            shopACanvas.enabled = false;
            input.EnableGamePlayInputs();
            yield break;
        }
        IEnumerator CloseShopBPanel()
        {
            Time.timeScale = 1f;
            hUDCanvas.enabled = true;
            shopBPanelRectTransform.DOScale(Vector3.zero, transitionTime);
            yield return new WaitForSeconds(transitionTime);
            shopBCanvas.enabled = false;
            input.EnableGamePlayInputs();
            yield break;
        }
        IEnumerator CloseShopCPanel()
        {
            Time.timeScale = 1f;
            hUDCanvas.enabled = true;
            shopCPanelRectTransform.DOScale(Vector3.zero, transitionTime);
            yield return new WaitForSeconds(transitionTime);
            shopCCanvas.enabled = false;
            input.EnableGamePlayInputs();
            yield break;
        }
        #endregion

        #region Boss UI
        public void ShowFelkoHpBar()
        {
            Instantiate(felkoHpBar);
        }
        #endregion

        #region Dialogue
        public void UpdateDialogueData(DialogueData_SO data)
        {
            AudioManager.Instance.PlaySFX(openCanvasSfx);
            hUDCanvas.enabled = false;
            input.EnableDialogueInputs();
            currentData = data;
            currentIndex = 0;
        }

        public void UpdateMainDialogue(DialoguePiece piece)
        {
            canContinue = true;
            canNextText = false;
            dialogueCanvas.enabled = true;
            if (currentIndex == 0)
            {
                if (!piece.isFromNpc)
                {
                    standPanelTransform.anchoredPosition = new Vector2(-550f, -20f);
                    standPanelTransform.DOAnchorPos(new Vector2(0f, -20f), 0.3f);
                    dialogueTransform.anchoredPosition = new Vector2(0f, -700f);
                    dialogueTransform.DOAnchorPos(new Vector2(0f, -353f), 0.3f);
                }
                else if (piece.isFromNpc)
                {
                    standPanelTransform.anchoredPosition = new Vector2(550f, -20f);
                    standPanelTransform.DOAnchorPos(new Vector2(0f, -20f), 0.3f);
                    dialogueTransform.anchoredPosition = new Vector2(0f, -700f);
                    dialogueTransform.DOAnchorPos(new Vector2(0f, -353f), 0.3f);
                }
            }
            else if(currentIndex > 0)
            {
                AudioManager.Instance.PlaySFX(normalClickSfx);
            }

            if (piece.image != null)
            {
                icon.enabled = true;
                icon.sprite = piece.image;
            }
            else
            {
                icon.enabled = false;
            }
            mainText.text = "";
            speakerName.text = "";
            float textTime = piece.text.Length * 0.03f;
            speakerName.text = piece.speakerName;
            mainText.DOText(piece.text, textTime).SetEase(Ease.Linear);
            Invoke("CanNextText", textTime);
            if (currentData.dialoguePieces.Count > 0)
            {
                currentIndex++;
            }
        }

        private void CanNextText() => canNextText = true;

        private void CLoseMainDialogue(DialoguePiece piece)
        {
            AudioManager.Instance.PlaySFX(closeCanvasSfx);
            hUDCanvas.enabled = true;
            input.EnableGamePlayInputs();
            if (!piece.isEndNpc)
            {
                standPanelTransform.DOAnchorPos(new Vector2(-550f, -20f), 0.2f);
            }
            else if (piece.isEndNpc)
            {
                standPanelTransform.DOAnchorPos(new Vector2(550f, -20f), 0.2f);
            }
            dialogueTransform.DOAnchorPos(new Vector2(0f, -700f), 0.2f);
            StartCoroutine(CloseDialogueCanvas());
        }

        private void DialogueFinishEvent(DialoguePiece piece)
        {
            switch (piece.eventType)
            {
                case DialogueEventType.Normal:
                    {
                        break;
                    }
                case DialogueEventType.ShopA:
                    {
                        OpenShop(ShopIndex.A);
                        break;
                    }
                case DialogueEventType.ShopB:
                    {
                        OpenShop(ShopIndex.B);
                        break;
                    }
                case DialogueEventType.ShopC:
                    {
                        OpenShop(ShopIndex.C);
                        break;
                    }
                case DialogueEventType.FelkoSpawn:
                    {
                        FelkoRespawn.Instance.SpawnFelko();
                        break;
                    }
            }
        }

        IEnumerator CloseDialogueCanvas()
        {
            yield return new WaitForSeconds(0.2f);
            dialogueCanvas.enabled = false;
            yield break;
        }
        #endregion

        #region CatHouse
        public void OpenCatHouseCanvas()
        {
            AudioManager.Instance.PlaySFX(openCanvasSfx);
            isUI = true;
            isCatHouse = true;
            catHouseCanvas.enabled = true;
            catHousePanelRectTransform.localScale = Vector2.zero;
            catHousePanelRectTransform.DOScale(Vector2.one, transitionTime);
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(catHouseFirst);
            }
            input.EnablePlayerPanelInputs();
        }
        public void CloseCatHouseCanvas()
        {
            AudioManager.Instance.PlaySFX(closeCanvasSfx);
            isUI = false;
            isCatHouse = false;
            catHouseCanvas.enabled = true;
            EventSystem.current.SetSelectedGameObject(null);
            StartCoroutine(CloseCatHosuePanel());
        }

        IEnumerator CloseCatHosuePanel()
        {
            catHousePanelRectTransform.DOScale(Vector3.zero, transitionTime);
            yield return new WaitForSeconds(transitionTime);
            catHouseCanvas.enabled = false;
            input.EnableGamePlayInputs();
            yield break;
        }
        #endregion

        #region Esc Menu
        private void OpenEscMenuCanvas()
        {
            //Time.timeScale = 0f;
            AudioManager.Instance.PlaySFX(openCanvasSfx);
            StartCoroutine(PauseGame());
            isUI = true;
            isEscMenu = true;
            hUDCanvas.enabled = false;
            escMenuCanvas.enabled = true;
            escMenuPanelRectTransform.anchoredPosition = new Vector2(escMenuPanelRectTransform.anchoredPosition.x, 855f);
            escMenuPanelRectTransform.DOAnchorPos(new Vector2(escMenuPanelRectTransform.anchoredPosition.x, 213f), 0.15f);
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(escMenuFirst);
            }
            input.EnablePlayerPanelInputs();
        }

        IEnumerator PauseGame()
        {
            yield return new WaitForSeconds(0.17f);
            Time.timeScale = 0f;
            yield break;
        }

        public void CloseEscMenuCanvas()
        {
            Time.timeScale = 1f;
            AudioManager.Instance.PlaySFX(closeCanvasSfx);
            isUI = false;
            isEscMenu = false;
            hUDCanvas.enabled = true;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            StartCoroutine(CloseEscMenu());
        }

        IEnumerator CloseEscMenu()
        {
            escMenuPanelRectTransform.DOAnchorPos(new Vector2(escMenuPanelRectTransform.anchoredPosition.x, 855f), 0.15f);
            yield return new WaitForSeconds(0.15f);
            escMenuCanvas.enabled = false;
            yield return new WaitForSeconds(0.1f);
            input.EnableGamePlayInputs();
            yield break;
        }
        #endregion

        #region Sundry
        public void OpenSuccessCanvas()
        {
            isSecondaryMenu = true;
            AudioManager.Instance.PlaySFX(openCanvasSfx);
            isdoSuccess = true;
            doSuccessText.SetActive(true);
            doFailText.SetActive(false);
            doSuccessCanvas.enabled = true;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(doSuccessButton);
            }
            doSuccessPanelTransform.localScale = Vector2.zero;
            doSuccessPanelTransform.DOScale(Vector2.one, 0.1f);
        }
        public void OpenFailCanvas()
        {
            isSecondaryMenu = true;
            Debug.Log("failsfx");
            isdoSuccess = true;
            doSuccessText.SetActive(false);
            doFailText.SetActive(true);
            doSuccessCanvas.enabled = true;
            if (isPad)
            {
                EventSystem.current.SetSelectedGameObject(doSuccessButton);
            }
            doSuccessPanelTransform.localScale = Vector2.zero;
            doSuccessPanelTransform.DOScale(Vector2.one, 0.1f);
            AudioManager.Instance.PlaySFX(failSfx);
        }

        public void OperationSuccess()
        {
            AudioManager.Instance.PlaySFX(closeCanvasSfx);
            isSecondaryMenu = false;
            isdoSuccess = false;
            doSuccessPanelTransform.DOScale(Vector2.zero, 0.1f);
            StartCoroutine(CloseSuccessPanel());
            if (isPad)
            {
                if (isShop)
                {
                    if (isShopA)
                    {
                        EventSystem.current.SetSelectedGameObject(shopAFirst);
                    }
                    else if (isShopB)
                    {
                        EventSystem.current.SetSelectedGameObject(shopBFirst);
                    }
                    else if (isShopC)
                    {
                        EventSystem.current.SetSelectedGameObject(shopCFirst);
                    }
                }
                if (isCatHouse)
                {
                    EventSystem.current.SetSelectedGameObject(catHouseFirst);
                }
            }
        }

        IEnumerator CloseSuccessPanel()
        {
            yield return new WaitForSeconds(0.1f);
            yield return doSuccessCanvas.enabled = false;
            yield break;
        }
        #endregion
        void SwitchToMouse()
        {
            isPad = false;
            if (!isMouse)
            {
                isMouse = true;
                EventSystem.current.SetSelectedGameObject(null);
            }
            if (isUI)
            {
                InputDeviceDetector.ShowCursor();
            }
        }

        void SwitchToKeyboard()
        {
            if (isUI)
            {
                InputDeviceDetector.ShowCursor();
            }
            isMouse = false;
            if (!isPad)
            {
                isPad = true;
                if (isBag)
                {
                    EventSystem.current.SetSelectedGameObject(inventoryBagFirst);
                }
                else if (isShop)
                {
                    if (isShopA)
                    {
                        EventSystem.current.SetSelectedGameObject(shopAFirst);
                    }
                    else if (isShopB)
                    {
                        EventSystem.current.SetSelectedGameObject(shopBFirst);
                    }
                    else if (isShopC)
                    {
                        EventSystem.current.SetSelectedGameObject(shopCFirst);
                    }
                }
                else if (isdoSuccess)
                {
                    EventSystem.current.SetSelectedGameObject(doSuccessButton);
                }
                else if (isCatHouse)
                {
                    EventSystem.current.SetSelectedGameObject(catHouseFirst);
                }
                else if(isEscMenu)
                {
                    EventSystem.current.SetSelectedGameObject(escMenuFirst);
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
                if (isBag)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(inventoryBagFirst);
                }
                else if (isShop)
                {
                    if (isShopA)
                    {
                        EventSystem.current.SetSelectedGameObject(shopAFirst);
                    }
                    else if (isShopB)
                    {
                        EventSystem.current.SetSelectedGameObject(shopBFirst);
                    }
                    else if (isShopC)
                    {
                        EventSystem.current.SetSelectedGameObject(shopCFirst);
                    }
                }
                else if (isdoSuccess)
                {
                    EventSystem.current.SetSelectedGameObject(doSuccessButton);
                }
                else if (isCatHouse)
                {
                    EventSystem.current.SetSelectedGameObject(catHouseFirst);
                }
                else if (isEscMenu)
                {
                    EventSystem.current.SetSelectedGameObject(escMenuFirst);
                }
            }
        }
    }
}
