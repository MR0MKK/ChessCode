using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

/// QUẢN LÝ GIAO DIỆN CỦA GAME 

public class Interface : MonoBehaviour
{

    #region Variables

    public static Interface interfaceClass;

    
    /// Menu chính
    [Header("Main Menu")]
    [SerializeField] GameObject mainMenu = null;  
    /// Bảng phụ của Menu chính    
    [SerializeField] GameObject[] panelsMenu = null;    
    /// TEXT thông báo khi bị lỗi kết nối    
    [SerializeField] Text textErrorConnection = null;    
    /// TEXT thông báo đã kết nối    
    [SerializeField] Text serverText = null;    
    /// Text cho phép LOAD SAVE GAME  


    [Header("Panel Load")]
    [SerializeField] Text textLoad0 = null;    
    /// Text cho phép load save slot 1    
    [SerializeField] Text textLoad1 = null;    
    /// Text cho phép load save slot 2      
    [SerializeField] Text textLoad2 = null;    
    /// Text cho phép load save slot 3     
    [SerializeField] Text textLoad3 = null;    
    /// Button cho phép load AutoSave     
    [SerializeField] Button buttonLoad0 = null;    
    /// Button cho phép load save slot 1        
    [SerializeField] Button buttonLoad1 = null;    
    /// Button cho phép load save slot 2        
    [SerializeField] Button buttonLoad2 = null;    
    /// Button cho phép load save slot 3  
    [SerializeField] Button buttonLoad3 = null;

    
    /// Button hình bánh răng => PAUSE GAME
    [Header("Panel Pause")]
    [SerializeField] GameObject buttonPauseObject = null;    
    /// Panel chứa panel con của PAUSE MENU  
    [SerializeField] GameObject panelPause = null;    
    /// Các bảng phụ chứa trong PAUSE MENU  
    [SerializeField] GameObject[] panelsPause = null;    
    /// TEXT cho phép lưu vào vị trí 1  
    [SerializeField] Text textSave1 = null;    
    /// TEXT cho phép lưu vào vị trí 2     
    [SerializeField] Text textSave2 = null;    
    /// TEXT cho phép lưu vào vị trí 3      
    [SerializeField] Text textSave3 = null;    
    /// Button cho phép save slot 1  
    [SerializeField] Button buttonSave1 = null;    
    /// Button cho phép save slot 2  
    [SerializeField] Button buttonSave2 = null;    
    /// Button cho phép save slot 3  
    [SerializeField] Button buttonSave3 = null;    
    /// Button xác nhận Save( trường hơp hợp ghi đè)
    [SerializeField] Button buttonConfirmSave = null;    
    /// Button Save  
    [SerializeField] GameObject buttonSave = null;    
    /// Button Restart    
    [SerializeField] GameObject buttonRestart = null;    
    /// Button xác nhận muốn Restart    
    [SerializeField] Button buttonConfirmRestart = null;

    
    /// Bảng màu nâu => hiện thi bàn cờ  
    [Header("Panel Game")]
    [SerializeField] GameObject panelGame = null;    
    /// Các bảng phụ của Panel Game
    [SerializeField] GameObject[] panelsGame = null;    
    /// Button chơi lại khi kết thúc ván
    [SerializeField] Button buttonRestartEndGame = null;    
    /// Button cho màu Trắng khi chơi AI
    [SerializeField] Button buttonColourWhite = null;    
    /// Button cho màu Đen khi chơi AI    
    [SerializeField] Button buttonColourBlack = null;    
    /// Sound khi di chuyển quân
    [SerializeField] AudioSource moveSound = null;

    
    /// Bảng hiển thị thông báo lượt
    [Header("Panel Notifications")]
    [SerializeField] GameObject panelNotifications = null;    
    /// TEXT thông báo 
    [SerializeField] Text notificationsText = null;    
    /// Vòng tròn hiển thị đến lượt đội nào    
    [SerializeField] Image notificationsImage = null;
    

    /// Bảng hiện thị thông báo chiếu    
    [Header("Panel Check")]
    [SerializeField] GameObject panelCheck = null;    
    /// TEXT thông báo đang chiếu
    [SerializeField] Text checkText = null;    
    /// TEXT thông báo đang đợi người chơi thứ 2
    [Header("Panel Waiting Player 2")]
    [SerializeField] Text textWaitingRoomName = null; 


    /// TEXT cho biết tên phòng
    [Header("Panel Keyboard")]
    [SerializeField] Text textRoomName = null;   


    /// TEXT thông báo có quân tốt cần phong    
    [Header("Panel Promotion")]
    [SerializeField] Text promotionText = null;    
    /// Bảng chọn quân tốt Trắng muốn phong  
    [SerializeField] GameObject panelPiecesWhite = null;
    /// Bảng chọn quân tốt Đen muốn phong .    
    [SerializeField] GameObject panelPiecesBlack = null;

    // Firebase
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public static string userName = "";
    // Login 
    [Space]
    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;
    [SerializeField] Text loginFailText = null; 

    // Registration 
    [Space]
    [Header("Registration")]
    public InputField nameRegisterField;
    public InputField emailRegisterField;
    public InputField passwordRegisterField;
    public InputField confirmPasswordRegisterField;
    [SerializeField] Text registerFailText = null; 

    [Header("Email")]
    [SerializeField] Text emailFailText = null; 
    [SerializeField] Text emailChangePassword = null;

    

    #endregion

    #region Run

    public void Awake()
    {
        StartCoroutine(CheckAndFixDependenciesAsync());

        emailChangePassword.GetComponent<Text>().enabled = false;

        interfaceClass = this;

        Options.LoadOptions();

        LetterBoxer.AddLetterBoxing();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = 60;

        UpdateSaveDates();        
    }
    #endregion

    #region Main Menu

    
    /// Mở Menu chính với bảng phụ được chỉ định
    
    /// <param name="panel">Vị trí của bảng phụ cần mở</param>
    public void OpenPanelMenu(GameObject panel)
    {
        for (int i = 0; i < panelsMenu.Length; i++)
        {
            panelsMenu[i].SetActive(false);
        }

        panel.SetActive(true);
        mainMenu.SetActive(true);
        emailChangePassword.GetComponent<Text>().enabled = false;
        panelPause.SetActive(false);
        panelGame.SetActive(false);
    }

    
    /// Mở Menu chính với bảng phụ được chỉ định
    
    /// <param name="panel">Vị trí của bảng phụ cần mở</param>
    public void OpenPanelMenu(int panel)
    {
        for (int i = 0; i < panelsMenu.Length; i++)
        {
            panelsMenu[i].SetActive(false);
        }

        panelsMenu[panel].SetActive(true);
        mainMenu.SetActive(true);
        panelPause.SetActive(false);
        panelGame.SetActive(false);
       
    }

    
    /// Mở bảng thông báo lỗi kết nối
    
    /// <param name="cause">Nguyên nhân lỗi kết nối</param>
    public void OpenErrorPanel(Photon.Realtime.DisconnectCause cause)
    {
        switch (cause)
        {
            case Photon.Realtime.DisconnectCause.MaxCcuReached:
                textErrorConnection.text = ServerFull();
                break;
            case Photon.Realtime.DisconnectCause.DnsExceptionOnConnect:
                textErrorConnection.text = NoInternetConnection();
                break;
            case Photon.Realtime.DisconnectCause.InvalidRegion:
                textErrorConnection.text = InvalidRegion();
                break;
            default:
                textErrorConnection.text = GenericServerError();
                break;
        }

        Chess.CleanScene();

        OpenPanelMenu(3);
    }

    
    /// Bắt đầu trò chơi mới Player vs AI với quân Trắng
    
    public void NewGameWhite()
    {
        buttonPauseObject.SetActive(true);
        panelCheck.SetActive(false);
        OpenPanelGame(0);

        buttonRestartEndGame.onClick.AddListener(delegate { OpenPanelColours(); });
        buttonConfirmRestart.onClick.AddListener(delegate { OpenPanelColours(); });

        Chess.SelectColor(Enums.Colours.White, null);
    }

    
    /// Bắt đầu trò chơi mới Player vs AI với quân Đen
    
    public void NewGameBlack()
    {
        buttonPauseObject.SetActive(true);
        panelCheck.SetActive(false);
        OpenPanelGame(0);

        buttonRestartEndGame.onClick.AddListener(delegate { OpenPanelColours(); });
        buttonConfirmRestart.onClick.AddListener(delegate { OpenPanelColours(); });

        Chess.SelectColor(Enums.Colours.Black, null);
    }

    
    /// Load trò chơi mới Player vs AI với quân Trắng
    
    /// <param name="saveSlot">Các slot game</param>
    public void LoadGameWhite(int saveSlot)
    {
        buttonPauseObject.SetActive(true);
        panelCheck.SetActive(false);
        OpenPanelGame(0);

        buttonRestartEndGame.onClick.AddListener(delegate { OpenPanelColours(); });
        buttonConfirmRestart.onClick.AddListener(delegate { OpenPanelColours(); });

        Chess.SelectColor(Enums.Colours.White, SaveManager.LoadGame(saveSlot));
    }

    
    /// Load trò chơi mới Player vs AI với quân Đen
    
    /// <param name="saveSlot">Các slot game</param>
    public void LoadGameBlack(int saveSlot)
    {
        buttonPauseObject.SetActive(true);
        panelCheck.SetActive(false);
        OpenPanelGame(0);

        buttonRestartEndGame.onClick.AddListener(delegate { OpenPanelColours(); });
        buttonConfirmRestart.onClick.AddListener(delegate { OpenPanelColours(); });

        Chess.SelectColor(Enums.Colours.Black, SaveManager.LoadGame(saveSlot));
    }

    
    /// Bắt đầu trò chơi mới Multiplayer
    
    public void NewGame()
    {
        panelCheck.SetActive(false);
        OpenPanelGame(0);

        buttonRestartEndGame.onClick.AddListener(delegate { NewGame(); });
        buttonConfirmRestart.onClick.AddListener(delegate { NewGame(); });

        buttonPauseObject.SetActive(true);

        Chess.CleanScene();
        Chess.StartNewGame();
    }

    
    /// Load trò chơi Multiplayer
    
    /// <param name="saveSlot">Các slots game</param>
    public void LoadGame(int saveSlot)
    {
        panelCheck.SetActive(false);
        OpenPanelGame(0);

        buttonRestartEndGame.onClick.AddListener(delegate { NewGame(); });
        buttonConfirmRestart.onClick.AddListener(delegate { NewGame(); });

        buttonPauseObject.SetActive(true);

        SaveData data = SaveManager.LoadGame(saveSlot);

        Chess.CleanScene();
        Chess.StartLoadedGame(data);
    }

    
    /// Cập nhật server muốn kết nối
    
    public void UpdateServerName()
    {
        string server = "";

        switch (Options.ActiveServer)
        {
            case Options.Server.Asia:
                server = "Asia";
                break;
            case Options.Server.Australia:
                server = "Australia";
                break;
            case Options.Server.CanadaEast:
                server = "Canada East";
                break;
            case Options.Server.Europe:
                server = "Europe";
                break;
            case Options.Server.India:
                server = "India";
                break;
            case Options.Server.Japan:
                server = "Japan";
                break;
            case Options.Server.RussiaEast:
                server = "Russia East";
                break;
            case Options.Server.RussiaWest:
                server = "Russia West";
                break;
            case Options.Server.SouthAfrica:
                server = "South Africa";
                break;
            case Options.Server.SouthAmerica:
                server = "South America";
                break;
            case Options.Server.SouthKorea:
                server = "South Korea";
                break;
            case Options.Server.Turkey:
                server = "Turkey";
                break;
            case Options.Server.USAEast:
                server = "USA East";
                break;
            case Options.Server.USAWest:
                server = "USA West";
                break;
        }

        serverText.text = "Server " + server;
    }

    
    /// Kích hoạt nút Multiplayer
    /// Lưu ý: phải ngăn việc lưu trò chơi trước khi bắt đầu
    
    public void EnableOnlineSave()
    {
        buttonSave.SetActive(true);
    }

    
    /// Lưu trò chơi vào slot
    
    /// <param name="saveSlot">Slot game</param>
    public void SaveGame(int saveSlot)
    {
        Chess.SaveGame(saveSlot);

        UpdateSaveDates();
        OpenPanelPause(1);
    }

 
    /// Mở link liên kết bên ngoài
    public void OpenLink(int link)
    {
        if (link == 0)
        {
            Application.OpenURL("https://www.facebook.com/profile.php?id=100009756049363");
        }

        else
        {
            Application.OpenURL("https://github.com/MR0MKK/Chess.git");
        }
    }

    
    /// Thay đổi độ phân giải màn hình
    
    /// <param name="resolution">Chỉ số độ phân giải</param>
    public void ChangeResolution(int resolution)
    {
        switch (resolution)
        {
            case 0:
                Options.ActiveResolution = Options.Resolution.Fullscreen;
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
                break;

            case 1:
                Options.ActiveResolution = Options.Resolution.Windowed720;
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                break;

            case 2:
                Options.ActiveResolution = Options.Resolution.Windowed480;
                Screen.SetResolution(854, 480, FullScreenMode.Windowed);
                break;
        }

        Options.SaveOptions();
    }

    
    /// Thay đổi ngôn ngữ
    
    /// <param name="language">Ngôn ngữ được họn</param>
    public void ChangeLanguage(int language)
    {
        switch (language)
        {
            case 0:
                Options.ActiveLanguage = Options.Language.EN;
                break;

            case 1:
                Options.ActiveLanguage = Options.Language.VI;
                break;

            case 2:
                Options.ActiveLanguage = Options.Language.CA;
                break;

            case 3:
                Options.ActiveLanguage = Options.Language.IT;
                break;
        }

        Options.SaveOptions();
        UpdateSaveDates();

        OpenPanelMenu(10);
    }

    
    /// Thay đổi máy chủ Photon thành máy chủ đã chón
    
    /// <param name="server">Chỉ máy chủ muốn kích hoạt</param>
    public void ChangeServer(int server)
    {
        Options.ActiveServer = (Options.Server)server;

        Options.SaveOptions();

        OpenPanelMenu(10);
    }

    
    /// Đóng ứng dụng
    
    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Pause Panel

    /// Mở Pause Menu => ẩn game
    
    public void OpenPause()
    {
        if (!panelPause.activeSelf)
        {
            OpenPanelPause(0);
            Chess.PauseGame(true);
        }

        else
        {
            panelPause.SetActive(false);
            panelGame.SetActive(true);
            Chess.PauseGame(false);
        }
    }

    
    /// Mở Pause Menu + các bảng phụ
    
    /// <param name="panel">Bảng phụ</param>
    public void OpenPanelPause(GameObject panel)
    {
        for (int i = 0; i < panelsPause.Length; i++)
        {
            panelsPause[i].SetActive(false);
        }

        panel.SetActive(true);
        panelGame.SetActive(false);
        panelPause.SetActive(true);
    }

    
    /// Mở Pause Menu + các bảng phụ
    
    /// <param name="panel">Bảng phụ cần mở</param>
    public void OpenPanelPause(int panel)
    {
        for (int i = 0; i < panelsPause.Length; i++)
        {
            panelsPause[i].SetActive(false);
        }

        panelsPause[panel].SetActive(true);
        panelGame.SetActive(false);
        panelPause.SetActive(true);
    }

    
    /// TEXT của Save Game và Load Game
    /// Nếu Slot Game đó trống => không thể Load Save Game
    
    public void UpdateSaveDates()
    {
        string[] dates = SaveManager.GetDates();

        textLoad0.text = dates[0] != "0" ? ("AutoSave:  " + dates[0]) : ("AutoSave:  " + EmptyText());
        textLoad1.text = dates[1] != "0" ? ("Save 01:  " + dates[1]) : ("Save 01:  " + EmptyText());
        textLoad2.text = dates[2] != "0" ? ("Save 02:  " + dates[2]) : ("Save 02:  " + EmptyText());
        textLoad3.text = dates[3] != "0" ? ("Save 03:  " + dates[3]) : ("Save 03:  " + EmptyText());

        buttonLoad0.enabled = dates[0] != "0";
        buttonLoad1.enabled = dates[1] != "0";
        buttonLoad2.enabled = dates[2] != "0";
        buttonLoad3.enabled = dates[3] != "0";

        textSave1.text = dates[1] != "0" ? ("Save 01:  " + dates[1]) : ("Save 01:  " + EmptyText());
        textSave2.text = dates[2] != "0" ? ("Save 02:  " + dates[2]) : ("Save 02:  " + EmptyText());
        textSave3.text = dates[3] != "0" ? ("Save 03:  " + dates[3]) : ("Save 03:  " + EmptyText());

        buttonSave1.onClick.AddListener(delegate { ButtonSave(1, dates[1] == "0"); });
        buttonSave2.onClick.AddListener(delegate { ButtonSave(2, dates[2] == "0"); });
        buttonSave3.onClick.AddListener(delegate { ButtonSave(3, dates[3] == "0"); });
    }

    
    /// Cập nhật nút Save và Load cho chế độ người chơi
    
    /// <param name="gameMode">1.AI 2.LAN 3.Multiplayer</param>
    public void UpdateLoadButton(int gameMode)
    {
        switch (gameMode)
        {
            case 1:
                buttonLoad0.onClick.AddListener(delegate { OpenPanelColoursLoad(0); });
                buttonLoad1.onClick.AddListener(delegate { OpenPanelColoursLoad(1); });
                buttonLoad2.onClick.AddListener(delegate { OpenPanelColoursLoad(2); });
                buttonLoad3.onClick.AddListener(delegate { OpenPanelColoursLoad(3); });
                break;

            case 2:
                buttonLoad0.onClick.AddListener(delegate { LoadGame(0); });
                buttonLoad1.onClick.AddListener(delegate { LoadGame(1); });
                buttonLoad2.onClick.AddListener(delegate { LoadGame(2); });
                buttonLoad3.onClick.AddListener(delegate { LoadGame(3); });
                break;

            case 3:
                buttonLoad0.onClick.AddListener(delegate { NetworkManager.manager.CreateLoadedRoom(0); });
                buttonLoad1.onClick.AddListener(delegate { NetworkManager.manager.CreateLoadedRoom(1); });
                buttonLoad2.onClick.AddListener(delegate { NetworkManager.manager.CreateLoadedRoom(2); });
                buttonLoad3.onClick.AddListener(delegate { NetworkManager.manager.CreateLoadedRoom(3); });
                break;
        }
    }

    
    /// Đóng trò chơi và trở lại Menu
    
    public void BackToMenuEndGame()
    {
        
        Chess.CleanScene();
        buttonRestart.SetActive(true);

        buttonPauseObject.SetActive(true);
        OpenPanelMenu(0);
        NetworkManager.manager.DisconnectFromServer();
    }

    
    /// Lưu trò chơi hiện tại
    
    /// <param name="saveSlot">Slot game</param>
    /// <param name="emptySlot">Các slot còn trống</param>
    public void ButtonSave(int saveSlot, bool emptySlot)
    {
        // Nếu còn trống => lưu 

        if (emptySlot)
        {
            SaveGame(saveSlot);
        }

        // Nếu không trống => muốn ghi đè lên không

        else
        {
            buttonConfirmSave.onClick.AddListener(delegate { SaveGame(saveSlot); });
            OpenPanelPause(4);
        }
    }

    
    /// Bật hoặc tắt nút tạm dừng
    
    /// <param name="enable">Bật hay Tắt</param>
    public void EnableButtonPause(bool enable)
    {
        if (enable)
        {
            buttonPauseObject.SetActive(true);
        }

        else
        {
            buttonPauseObject.SetActive(false);
        }
    }

    #endregion

    #region Game Panel

    
    /// Mở bảng hiện thị bàn cờ
    
    /// <param name="panel">Bảng phụ</param>
    public void OpenPanelGame(GameObject panel)
    {
        for (int i = 0; i < panelsGame.Length; i++)
        {
            panelsGame[i].SetActive(false);
        }

        panel.SetActive(true);
        mainMenu.SetActive(false);
        panelPause.SetActive(false);
        panelGame.SetActive(true);
        
    }

    
    /// Mở bảng hiện thị bàn cờ
    
    /// <param name="panel">Bảng phụ</param>
    public void OpenPanelGame(int panel)
    {
        for (int i = 0; i < panelsGame.Length; i++)
        {
            panelsGame[i].SetActive(false);
        }

        panelsGame[panel].SetActive(true);
        mainMenu.SetActive(false);
        panelPause.SetActive(false);
        panelGame.SetActive(true);
    }

    
    /// Mở bảng hiện thị khi người chơi mất kết nôi
    
    public void ErrorPlayerLeftRoom()
    {
        Chess.CleanScene();

        buttonPauseObject.SetActive(true);
        OpenPanelMenu(8);
    }

    
    /// Chọn màu khi bắt đầu trận mới với AI
    
    public void OpenPanelColours()
    {
        Chess.CleanScene();
        buttonPauseObject.SetActive(false);

        buttonColourWhite.onClick.AddListener(delegate { NewGameWhite(); });
        buttonColourBlack.onClick.AddListener(delegate { NewGameBlack(); });
        
        OpenPanelGame(7);
    }

    
    ///  Chọn màu khi Load trận mới với AI
    
    /// <param name="saveSlot">Slot game</param>
    public void OpenPanelColoursLoad(int saveSlot)
    {
        Chess.CleanScene();
        buttonPauseObject.SetActive(false);

        buttonColourWhite.onClick.AddListener(delegate { LoadGameWhite(saveSlot); });
        buttonColourBlack.onClick.AddListener(delegate { LoadGameBlack(saveSlot); });
        
        OpenPanelGame(7);
    }

    
    /// Phát âm thanh khi di chuyển cờ
    
    public void PlayMoveSound()
    {
        moveSound.Play();
    }

    #endregion

    #region Waiting Panel

    
    /// Thông báo khi đến lượt
    
    /// <param name="colour">Màu sắc của người đến lượt chơi</param>
    public void SetWaitingMessage(Enums.Colours colour)
    {
        OpenPanelGame(0);

        notificationsText.text = (colour == Enums.Colours.White) ? WaitingMessageWhite() : WaitingMessageBlack();
        notificationsImage.color = (colour == Enums.Colours.White) ? Color.white : Color.black;
    }

    #endregion

    #region Waiting Player 2 Panel

    
    /// Thông báo đợi người chơi thứ 2
    
    public void OpenPanelWaitingPlayer()
    {
        OpenPanelGame(2);
        textWaitingRoomName.text = RoomName();
        buttonSave.SetActive(false);
        buttonRestart.SetActive(false);
    }

    #endregion

    #region Keyboard Panel

    
    /// Tên phòng
    
    string roomName;

    
    /// Kích hoạt bảng cho nhập tên phòng
    
    public void OpenPanelKeyboard()
    {
        OpenPanelGame(3);
        roomName = "";
        textRoomName.text = roomName;
        buttonSave.SetActive(false);
        buttonRestart.SetActive(false);
    }

    
    /// Thêm ký tự đã nhập vào tên phòng
    
    /// <param name="character">Tên đã nhập</param>
    public void EnterRoomLetter(string character)
    {
        if (roomName.Length < 3)
        {
            roomName += character;

            textRoomName.text = roomName;
        }
    }

    
    /// Loại bỏ kí tự cuối của tên phòng khi nhập
    
    public void DeleteRoomLetter()
    {
        if (roomName.Length > 0)
        {
            string newRoomName = "";

            for (int i = 0; i < roomName.Length - 1; i++)
            {
                newRoomName += roomName[i];
            }

            roomName = newRoomName;
            textRoomName.text = roomName;
        }
    }

    
    /// Vào phòng
    
    public void JoinRoom()
    {
        if (roomName.Length == 3)
        {
            NetworkManager.manager.JoinRoom(roomName);
        }
    }

    #endregion

    #region Check Message

    
    /// Bảng thông báo khi người chơi bị chiếu
    
    /// <param name="colour">Màu của đội bị chiếu</param>
    public void ActivatePanelCheck(Enums.Colours colour)
    {
        checkText.text = (colour == Enums.Colours.White) ? CheckMessageWhite() : CheckMessageBlack();

        panelCheck.SetActive(true);
    }

    
    /// Hủy bảng thông báo khi người chơi bị chiếu
    
    public void DeactivatePanelCheck()
    {
        panelCheck.SetActive(false);
    }

    #endregion

    #region Promotion Message

    
    /// Bảng thông báo chọn 1 quân để phong tốt Trắng 
    
    /// <param name="inTurn">True nếu là người chơi sở hữu tốt phong, False nếu ko( Multiplayer)</param>
    public void ActivatePromotionWhite(bool inTurn)
    {
        OpenPanelGame(1);

        // Hiện bảng cho phép chọn 1 trong 4 quân Trắng
        if (inTurn)
        {
            panelPiecesWhite.SetActive(true);
            promotionText.text = PromotionMessageWhite();
        }

        // Hiện TEXT thông báo quân đã phong

        else
        {
            promotionText.text = WaitPromotionMessageWhite();
        }
    }

    
    /// Bảng thông báo chọn 1 quân để phong tốt Trắng 
    
    /// <param name="inTurn">True nếu là người chơi sở hữu tốt phong, False nếu ko( Multiplayer)</param>
    public void ActivatePromotionBlack(bool inTurn)
    {
        OpenPanelGame(1);

        // Hiện bảng cho phép chọn 1 trong 4 quân Đen

        if (inTurn)
        {
            panelPiecesBlack.SetActive(true);
            promotionText.text = PromotionMessageBlack();
        }

        // Hiện TEXT thông báo quân đã phong

        else
        {
            promotionText.text = WaitPromotionMessageBlack();
        }
    }

    
    /// Phong tốt đã chọn
    
    /// <param name="piece">Quân cờ được chọn</param>
    public void PromotePieceWhite(string piece)
    {
        if (!NetworkManager.manager.IsConnected)
        {
            switch (piece)
            {
                case "Rook":
                    Chess.PieceSelectedToPromotion(Enums.PromotablePieces.Rook, Pieces.Colour.White);
                    break;
                case "Knight":
                    Chess.PieceSelectedToPromotion(Enums.PromotablePieces.Knight, Pieces.Colour.White);
                    break;
                case "Bishop":
                    Chess.PieceSelectedToPromotion(Enums.PromotablePieces.Bishop, Pieces.Colour.White);
                    break;
                case "Queen":
                    Chess.PieceSelectedToPromotion(Enums.PromotablePieces.Queen, Pieces.Colour.White);
                    break;
            }
        }

        else
        {
            switch (piece)
            {
                case "Rook":
                    NetworkManager.manager.PromotePiece(Enums.PromotablePieces.Rook, Pieces.Colour.White);
                    break;
                case "Knight":
                    NetworkManager.manager.PromotePiece(Enums.PromotablePieces.Knight, Pieces.Colour.White);
                    break;
                case "Bishop":
                    NetworkManager.manager.PromotePiece(Enums.PromotablePieces.Bishop, Pieces.Colour.White);
                    break;
                case "Queen":
                    NetworkManager.manager.PromotePiece(Enums.PromotablePieces.Queen, Pieces.Colour.White);
                    break;
            }
        }
    }

    
    /// Phong tốt đã chọn
    
    /// <param name="piece">Quân cờ được chọn</param>
    public void PromotePieceBlack(string piece)
    {
        if (!NetworkManager.manager.IsConnected)
        {
            switch (piece)
            {
                case "Rook":
                    Chess.PieceSelectedToPromotion(Enums.PromotablePieces.Rook, Pieces.Colour.Black);
                    break;
                case "Knight":
                    Chess.PieceSelectedToPromotion(Enums.PromotablePieces.Knight, Pieces.Colour.Black);
                    break;
                case "Bishop":
                    Chess.PieceSelectedToPromotion(Enums.PromotablePieces.Bishop, Pieces.Colour.Black);
                    break;
                case "Queen":
                    Chess.PieceSelectedToPromotion(Enums.PromotablePieces.Queen, Pieces.Colour.Black);
                    break;
            }
        }

        else
        {
            switch (piece)
            {
                case "Rook":
                    NetworkManager.manager.PromotePiece(Enums.PromotablePieces.Rook, Pieces.Colour.Black);
                    break;
                case "Knight":
                    NetworkManager.manager.PromotePiece(Enums.PromotablePieces.Knight, Pieces.Colour.Black);
                    break;
                case "Bishop":
                    NetworkManager.manager.PromotePiece(Enums.PromotablePieces.Bishop, Pieces.Colour.Black);
                    break;
                case "Queen":
                    NetworkManager.manager.PromotePiece(Enums.PromotablePieces.Queen, Pieces.Colour.Black);
                    break;
            }
        }
    }

    
    /// Hủy bảng phong tốt
    
    public void DisablePromotions()
    {
        panelsGame[1].SetActive(false);
        panelPiecesBlack.SetActive(false);
        panelPiecesWhite.SetActive(false);
    }

    #endregion

    #region Checkmate Message

    
    /// Hiển thị bảng "chiếu"
    
    /// <param name="colour">Màu của người chơi chiến thắng</param>
    public void ActivateCheckmateMessage(Enums.Colours colour)
    {
        if (!NetworkManager.manager.IsConnected)
        {
            OpenPanelGame(4);
        }

        // Bảng điều khiển sẽ khác nhau tùy thuộc vào chúng ta có đang trực tuyến không
        // Nếu đang chơi trực tuyến => ngắt kết nối với máy chủ

        else
        {
            OpenPanelGame(6);
            NetworkManager.manager.DisconnectAll();
        }

        panelNotifications.SetActive(true);
        buttonPauseObject.SetActive(false);
        panelCheck.SetActive(false);

        notificationsText.text = (colour == Enums.Colours.White) ? CheckmateMessageWhite() : CheckmateMessageBlack();
        notificationsImage.color = (colour == Enums.Colours.White) ? Color.white : Color.black;
    }

    #endregion

    #region Draw Message

    
    /// Kích hoạt bảng khi kết thúc kết quả hòa
    
    /// <param name="drawType">Lý do kết thúc kết quả hòa</param>
    public void ActivateDrawMessage(Enums.DrawModes drawType)
    {
        if (!NetworkManager.manager.IsConnected)
        {
            OpenPanelGame(4);
        }

        else
        {
            OpenPanelGame(6);
            NetworkManager.manager.DisconnectAll();
        }

        panelNotifications.SetActive(true);
        notificationsImage.color = Color.blue;

        switch (drawType)
        {
            case Enums.DrawModes.Stalemate:
                notificationsText.text = DrawStalemateMessage();
                break;
            case Enums.DrawModes.Impossibility:
                notificationsText.text = DrawImpossibilityMessage();
                break;
            case Enums.DrawModes.Move75:
                notificationsText.text = Draw75MovesMessage();
                break;
            case Enums.DrawModes.ThreefoldRepetition:
                notificationsText.text = DrawRepetitionMessage();
                break;
        }
    }

    #endregion

    #region Texts

    
    /// Thông báo máy chủ hiện tại đang đầy( max 20)
    string ServerFull()
    {
        return Resources.Load<TranslateText>("Texts/ServerFull").GetText(Options.ActiveLanguage);
    }

    
    /// Thông báo thiết bị không được kết nối Internet
    string NoInternetConnection()
    {
        return Resources.Load<TranslateText>("Texts/NoInternetConnection").GetText(Options.ActiveLanguage);
    }

    
    /// Thông báo khu vực hiện tại không khả dụng
    /// Ko phải lỗi do Photon Servive mà là do khu vực đã chọn
    string InvalidRegion()
    {
        return Resources.Load<TranslateText>("Texts/InvalidRegion").GetText(Options.ActiveLanguage);
    }

    
    /// Cho biết đã xảy ra lỗi chung trên máy chủ
    string GenericServerError()
    {
        return Resources.Load<TranslateText>("Texts/GenericServerError").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo vị trí lưu đang trống
    string EmptyText()
    {
        return Resources.Load<TranslateText>("Texts/EmptyText").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo đang đợi màu trắng để chơi.
    string WaitingMessageWhite()
    {
        return Resources.Load<TranslateText>("Texts/WaitingMessageWhite").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo đang đợi màu đen để chơi.
    string WaitingMessageBlack()
    {
        return Resources.Load<TranslateText>("Texts/WaitingMessageBlack").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo vua Trắng đang chiếu
    string CheckMessageWhite()
    {
        return Resources.Load<TranslateText>("Texts/CheckMessageWhite").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo vua Đen đang chiếu    
    string CheckMessageBlack()
    {
        return Resources.Load<TranslateText>("Texts/CheckMessageBlack").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo phong tốt Trắng
    string PromotionMessageWhite()
    {
        return Resources.Load<TranslateText>("Texts/PromotionMessageWhite").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo phong tốt Đen
    string PromotionMessageBlack()
    {
        return Resources.Load<TranslateText>("Texts/PromotionMessageBlack").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo đợi cho team Trắng phong tốt
    string WaitPromotionMessageWhite()
    {
        return Resources.Load<TranslateText>("Texts/WaitPromotionMessageWhite").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo đợi cho team Đen phong tốt
    string WaitPromotionMessageBlack()
    {
        return Resources.Load<TranslateText>("Texts/WaitPromotionMessageBlack").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo Trắng wins
    string CheckmateMessageWhite()
    {
        return Resources.Load<TranslateText>("Texts/CheckmateMessageWhite").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo Đen wins
    
    
    string CheckmateMessageBlack()
    {
        return Resources.Load<TranslateText>("Texts/CheckmateMessageBlack").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo hòa do ko còn nước đi
    string DrawStalemateMessage()
    {
        return Resources.Load<TranslateText>("Texts/DrawStalemateMessage").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo hòa do ko thể chiếu hết với số quân còn lại
    string DrawImpossibilityMessage()
    {
        return Resources.Load<TranslateText>("Texts/DrawImpossibilityMessage").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo hòa do ko thể chiếu hết với số quân còn lại
    string Draw75MovesMessage()
    {
        return Resources.Load<TranslateText>("Texts/Draw75MovesMessage").GetText(Options.ActiveLanguage);
    }

    
    /// TEXT thông báo hòa do đi quá 150 bước 
    string DrawRepetitionMessage()
    {
        return Resources.Load<TranslateText>("Texts/DrawRepetitionMessage").GetText(Options.ActiveLanguage);
    }

    
    /// Tên phòng    
    string RoomName()
    {
        return Resources.Load<TranslateText>("Texts/RoomName").GetText(Options.ActiveLanguage) + "" + NetworkManager.manager.ActiveRoom;
    }

    // Login
    string LoginFailEmailWrong()
    {
        return Resources.Load<TranslateText>("Texts/LoginFailEmailWrong").GetText(Options.ActiveLanguage);
    }
    string LoginFailPasswordWrong()
    {
        return Resources.Load<TranslateText>("Texts/LoginFailPasswordWrong").GetText(Options.ActiveLanguage);
    }
    string LoginFailEmailMissing()
    {
        return Resources.Load<TranslateText>("Texts/LoginFailEmailMissing").GetText(Options.ActiveLanguage);
    }
    string LoginFailPasswordMissing()
    {
        return Resources.Load<TranslateText>("Texts/LoginFailPasswordMissing").GetText(Options.ActiveLanguage);
    }
    string LoginFailMessage()
    {
        return Resources.Load<TranslateText>("Texts/LoginFailMessage").GetText(Options.ActiveLanguage);
    }
    string LoginFailUserNotFound()
    {
        return Resources.Load<TranslateText>("Texts/LoginFailUserNotFound").GetText(Options.ActiveLanguage);
    }

    // Register
    string RegisterFailMessage()
    {
        return Resources.Load<TranslateText>("Texts/RegisterFailMessage").GetText(Options.ActiveLanguage);
    }
    string RegisterFailEmailWrong()
    {
        return Resources.Load<TranslateText>("Texts/RegisterFailEmailWrong").GetText(Options.ActiveLanguage);
    }
    string RegisterFailPasswordWrong()
    {
        return Resources.Load<TranslateText>("Texts/RegisterFailPasswordWrong").GetText(Options.ActiveLanguage);
    }
    string RegisterFailEmailMissing()
    {
        return Resources.Load<TranslateText>("Texts/RegisterFailEmailMissing").GetText(Options.ActiveLanguage);
    }
    string RegisterFailPasswordMissing()
    {
        return Resources.Load<TranslateText>("Texts/RegisterFailPasswordMissing").GetText(Options.ActiveLanguage);
    }
    string RegisterFailDifferentPassword()
    {
        return Resources.Load<TranslateText>("Texts/RegisterFailDifferentPassword").GetText(Options.ActiveLanguage);
    }


    // Verification Email
    string VerifyCancel()
    {
        return Resources.Load<TranslateText>("Texts/VerifyCancel").GetText(Options.ActiveLanguage);
    }
    string VerifyTooManyRequests()
    {
        return Resources.Load<TranslateText>("Texts/VerifyTooManyRequests").GetText(Options.ActiveLanguage);
    }
    string VerifyInvalidRecipientEmail()
    {
        return Resources.Load<TranslateText>("Texts/VerifyInvalidRecipientEmail").GetText(Options.ActiveLanguage);
    }
    string VerifyEmail()
    {
        return Resources.Load<TranslateText>("Texts/VerifyEmail").GetText(Options.ActiveLanguage);
    }
    string NoVerifyEmail()
    {
        return Resources.Load<TranslateText>("Texts/NoVerifyEmail").GetText(Options.ActiveLanguage);
    }
    string VerifyTryAgain()
    {
        return Resources.Load<TranslateText>("Texts/VerifyTryAgain").GetText(Options.ActiveLanguage);
    }
    string EmailChangePassword()
    {
        return Resources.Load<TranslateText>("Texts/EmailChangePassword").GetText(Options.ActiveLanguage);
    }
    #endregion

    #region Login Change Screen
    public void OpenLoginPanel()
    {
        ClearLoginInput();
        OpenPanelMenu(14);
        loginFailText.GetComponent<Text>().enabled = false;
        
    }

    public void OpenRegistrationPanel()
    {
        
        ClearRegisterInput();
        OpenPanelMenu(15);
        emailFailText.GetComponent<Text>().enabled = false;
    }

    private void ClearLoginInput()
    {
        emailLoginField.text="";
        passwordLoginField.text="";
    }
    private void ClearRegisterInput()
    {
        nameRegisterField.text="";
        emailRegisterField.text="";
        passwordRegisterField.text="";
        confirmPasswordRegisterField.text="";
    }
    #endregion

    #region Logout

    public void Logout()
    {
        if(auth != null && user != null)
        {
            auth.SignOut();
        }
    }
    #endregion

    #region Firebase Setup
    private IEnumerator CheckAndFixDependenciesAsync()
    {
        var dependencyTask =FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => dependencyTask.IsCompleted);
    
        dependencyStatus = dependencyTask.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
        }
        else
        {
            Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
        }
    }


    void InitializeFirebase()
    {
        // Tạo đối tượng mặc định
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }


    // Theo dõi thay đổi của đối tượng Auth
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                OpenLoginPanel();
                ClearLoginInput();
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }
    #endregion

    #region Login

    public void Login()
    {
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }
    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => loginTask.IsCompleted);
        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);
            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;
            switch (authError)
            {
                case AuthError.InvalidEmail:
                    loginFailText.text = LoginFailEmailWrong(); 
                    break;
                case AuthError.WrongPassword:
                    loginFailText.text = LoginFailPasswordWrong();
                    break;
                case AuthError.MissingEmail:
                    loginFailText.text = LoginFailEmailMissing();
                    break;
                case AuthError.MissingPassword:
                    loginFailText.text = LoginFailPasswordMissing();
                    break;
                case AuthError.UserNotFound:
                    loginFailText.text = LoginFailUserNotFound();
                    break;
                default:
                    loginFailText.text = LoginFailMessage();  
                    break;
            }
            loginFailText.GetComponent<Text>().enabled = true;
        }
        else
        {
            user = loginTask.Result;
            Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);
            if(user.IsEmailVerified)
            {
                userName = user.DisplayName;
                OpenPanelMenu(0);
            }
            else
            {
                SendEmailForVerification();
            }
            
        }
    }

    private IEnumerator CheckForAutoLogin()
    {
        if(user != null)
        {
            var reloadUserTask = user.ReloadAsync();
            yield return new WaitUntil(() => reloadUserTask.IsCompleted);
            AutoLogin();
        }
        else
        {
            OpenLoginPanel();
        }
    }

    private void AutoLogin()
    {
        if(user != null)
        {
            if(user.IsEmailVerified)
            {
                userName = user.DisplayName;
                OpenPanelMenu(0);
            }
            else
            {
                SendEmailForVerification();
            }
        }
        else
        {
            OpenLoginPanel();
        }
    }
    #endregion

    #region Register

    public void Register()
    {
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
       
        // registerFailText.GetComponent<Text>().enabled = true;
        emailFailText.GetComponent<Text>().enabled = false;
        if (name == "")
        {
            registerFailText.text = RegisterFailMessage();
            registerFailText.GetComponent<Text>().enabled = true;
            ;
        }
        else if (email == "")
        {
            registerFailText.text = RegisterFailEmailMissing();
            registerFailText.GetComponent<Text>().enabled = true;
        }
        
        else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            registerFailText.text = RegisterFailDifferentPassword();
            registerFailText.GetComponent<Text>().enabled = true;
            Debug.LogError("Passwrod incorrect");
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => registerTask.IsCompleted);
            if (registerTask.Exception != null)
            {
                Debug.LogError(email);
                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;
                
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        registerFailText.text = RegisterFailEmailWrong(); 
                        break;
                    case AuthError.WrongPassword:
                        registerFailText.text = RegisterFailPasswordWrong();
                        break;
                    case AuthError.MissingEmail:
                        registerFailText.text = RegisterFailEmailMissing();
                        break;
                    case AuthError.MissingPassword:
                        registerFailText.text = RegisterFailPasswordMissing();
                        break;
                    default:
                        registerFailText.text = RegisterFailMessage();
                        break;
                }
                if(!(email.EndsWith("@gmail.com") || email.EndsWith("@yahoo.com")))
                {
                    registerFailText.text = RegisterFailEmailWrong();
                }
                emailFailText.GetComponent<Text>().enabled = false;
                registerFailText.GetComponent<Text>().enabled = true;

            }
            else
            {
                // Lưu người dùng sau khi đăng kí thành công
                user = registerTask.Result;
                UserProfile userProfile = new UserProfile { DisplayName = name };
                var updateProfileTask = user.UpdateUserProfileAsync(userProfile);
                yield return new WaitUntil(() => updateProfileTask.IsCompleted);
                if (updateProfileTask.Exception != null)
                {
                    // Xóa người dùng nếu đăng kí không thành công
                    user.DeleteAsync();
                    Debug.LogError(updateProfileTask.Exception);
                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            registerFailText.text = RegisterFailEmailWrong(); 
                            break;
                        case AuthError.WrongPassword:
                            registerFailText.text = RegisterFailPasswordWrong();
                            break;
                        case AuthError.MissingEmail:
                            registerFailText.text = RegisterFailEmailMissing();
                            break;
                        case AuthError.MissingPassword:
                            registerFailText.text = RegisterFailPasswordMissing();
                            break;
                        default:
                            registerFailText.text = RegisterFailMessage();
                            break;
                    }
                    emailFailText.GetComponent<Text>().enabled = false;
                    registerFailText.GetComponent<Text>().enabled = true;
                }
                else
                {
                    Debug.Log("Registration Sucessful Welcome " + user.DisplayName);
                    if(user.IsEmailVerified)
                    {
                        OpenLoginPanel();
                    }
                    else
                    {
                        emailFailText.GetComponent<Text>().enabled = true;
                        SendEmailForVerification();
                        
                    }
                }
            }
        }
    }
    #endregion

    #region VerifyEmail

    public void SendEmailForVerification()
    {
        StartCoroutine(SendEmailVerificationAsync());
    }

    private IEnumerator SendEmailVerificationAsync()
    {
        if(user != null)
        {
            var sendEmailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(() => sendEmailTask.IsCompleted);
            if(sendEmailTask.Exception!= null)
            {
                FirebaseException firebaseException=sendEmailTask.Exception.GetBaseException() as FirebaseException;
                AuthError error =(AuthError)firebaseException.ErrorCode;
                switch(error)
                {
                    case AuthError.Cancelled:
                        emailFailText.text = VerifyCancel();
                        break;
                    case AuthError.TooManyRequests:
                        emailFailText.text = VerifyTooManyRequests();
                        break;
                    case AuthError.InvalidRecipientEmail:
                        emailFailText.text = VerifyInvalidRecipientEmail();
                        break;
                    default:
                        emailFailText.text = VerifyTryAgain();
                        break;
                }
                ShowVerificantionResponse(false,emailFailText.text );
            }
            else
            {
                ShowVerificantionResponse(true,null );
                Debug.Log("Success");
            }
        }
    }

    public void ShowVerificantionResponse(bool isEmailSent, string errorMessage)
    {
        if(isEmailSent)
        {
            emailFailText.text =VerifyEmail();
            emailFailText.text +=user.Email;
            registerFailText.GetComponent<Text>().enabled = false;
            emailFailText.GetComponent<Text>().enabled = true;
            loginFailText.text = emailFailText.text;
            loginFailText.GetComponent<Text>().enabled = true;
        }
        else
        {
            emailFailText.text =NoVerifyEmail();
            emailFailText.text +=user.Email;
            registerFailText.GetComponent<Text>().enabled = false;
            emailFailText.GetComponent<Text>().enabled = true;
            loginFailText.text = emailFailText.text;
            loginFailText.GetComponent<Text>().enabled = true;
        }
    }
    #endregion

    #region ChangePassword
    public void ForgotPassword()
    {
        
        emailChangePassword.text=EmailChangePassword();
        emailChangePassword.text +=user.Email;
        emailChangePassword.GetComponent<Text>().enabled = true;
        // emailChangePassword.SetActive(true);
        
        auth.SendPasswordResetEmailAsync(user.Email).ContinueWith(task=>{
            if (task.IsCanceled) 
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                return;
            }
            Debug.Log("Password reset email sent successfully.");
        });
    }

    #endregion
}