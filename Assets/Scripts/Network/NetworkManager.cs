using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Chức năng cần thiết cho trò chơi
[RequireComponent(typeof(PhotonView))]
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager manager;

    
    // Trò chơi đã tải
    SaveData loadData = null;

    #region Properties

    
    // Cho biết có đang kết nối với máy chủ Photon không
    public bool IsConnected { get; private set; }

    
    // Tên phòng ( 3 kí tự)
    public string ActiveRoom { get; private set; }

    
    // Mã định danh của máy chủ( đưa ra bới Photon PUN)
    string Token
    {
        get
        {
            switch (Options.ActiveServer)
            {
                case Options.Server.Asia:
                    return "asia";
                case Options.Server.Australia:
                    return "au";
                case Options.Server.CanadaEast:
                    return "cae";
                case Options.Server.Europe:
                    return "eu";
                case Options.Server.India:
                    return "in";
                case Options.Server.Japan:
                    return "jp";
                case Options.Server.RussiaEast:
                    return "rue";
                case Options.Server.RussiaWest:
                    return "ru";
                case Options.Server.SouthAfrica:
                    return "za";
                case Options.Server.SouthAmerica:
                    return "sa";
                case Options.Server.SouthKorea:
                    return "kr";
                case Options.Server.Turkey:
                    return "tr";
                case Options.Server.USAEast:
                    return "us";
                case Options.Server.USAWest:
                    return "usw";
                default:
                    return "eu";
            }
        }
    }

    string RandomRoom
    {
        get
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            char char1 = characters[Random.Range(0, characters.Length)];
            char char2 = characters[Random.Range(0, characters.Length)];
            char char3 = characters[Random.Range(0, characters.Length)];

            return char1.ToString() + char2.ToString() + char3.ToString();
        }
    }

    #endregion

    private void Awake()
    {
        manager = this;
    }

    #region Conection

    
    // Kết nối trò chơi với máy chủ Photon.
    
    public void ConnectToServer()
    {
        // Thông báo trên màn hình đã kết nối

        Interface.interfaceClass.OpenPanelMenu(2);

        // Đã kết nối

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = Token;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // Chỉ ra đã kết nối Photon

        IsConnected = true;

        // Cái cần làm để tải 1 trò chơi trực tuyến

        PhotonPeer.RegisterType(typeof(SaveData), (byte)'S', SaveManager.Serialize, SaveManager.Deserialize);

        // Thông báo kết nối biến mất

        Interface.interfaceClass.UpdateServerName();
        Interface.interfaceClass.OpenPanelMenu(6);
    }

    
    // Ngừng kết nối máy chủ Photon
    
    public void DisconnectFromServer()
    {
        // Ngắt kết nối và gọi phương thức ngắt

        IsConnected = false;
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // Có nhiều lý do để ngắt kết nối với máy chủ
        // Thông báo giải thích

        switch (cause)
        {
            // Máy chủ đã đầy
            case DisconnectCause.MaxCcuReached:
                Interface.interfaceClass.OpenErrorPanel(cause);
                break;

            // Thiết bị không kết nối mạng
            case DisconnectCause.DnsExceptionOnConnect:
                Interface.interfaceClass.OpenErrorPanel(cause);
                break;

            // Máy chủ ngừng hoạt động
            case DisconnectCause.InvalidRegion:
                Interface.interfaceClass.OpenErrorPanel(cause);
                break;

            // Tắt game thủ công hoặc mất kết nối( chỉ thông báo cái thứ 2)
            case DisconnectCause.DisconnectByClientLogic:
                if (Chess.IsPlaying)
                    Interface.interfaceClass.ErrorPlayerLeftRoom();
                break;

            // Các lỗi còn lại
            default:
                Interface.interfaceClass.OpenErrorPanel(cause);
                break;
        }

        Debug.Log(cause);
    }

    
    // Tạo phòng và đợi người chơi
    
    public void CreateRoom()
    {
        // Khong có trò chơi hiện tại => bắt đầu từ đầu

        loadData = null;

        // Tạo tên ngẫu nhiên

        ActiveRoom = RandomRoom;

        // Tạo phòng. Max = 2 

        PhotonNetwork.CreateRoom(ActiveRoom, new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    
    // Đợi người thứ 2 để bắt đầu
    public void CreateLoadedRoom(int saveSlot)
    {
        // Tải trò chơi từ vị trí đã chọn

        loadData = SaveManager.LoadGame(saveSlot);

        // nhận được một tên ngẫu nhiên cho phòng

        ActiveRoom = RandomRoom;

        // Tạo phòng. Max = 2 

        PhotonNetwork.CreateRoom(ActiveRoom, new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Interface.interfaceClass.OpenPanelWaitingPlayer();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // Nếu tạo phòng không thành công (có phòng trùng tên) thì ta lặp lại chức năng với tên khác.

        ActiveRoom = "";

        CreateRoom();
    }

    
    // Bắt đầu kết nối với phòng do người chơi khác tạo trước đó
    public void JoinRoom(string roomName)
    {
        ActiveRoom = roomName;
        PhotonNetwork.JoinRoom(ActiveRoom);
    }

    public override void OnJoinedRoom()
    {
        // Người vào phòng trước => team Trắng

        if (PhotonNetwork.PlayerList.Length == 1)
        {
            Chess.PlayerColour = Enums.Colours.White;
        }

        // Người vào phòng sau => team Đen

        else if (PhotonNetwork.PlayerList.Length == 2)
        {
            Chess.PlayerColour = Enums.Colours.Black;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // Phòng ko tồn tại hay đủ người => báo lỗi

        Interface.interfaceClass.OpenPanelGame(5);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Người thứ 2 vào => bắt đầu
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            if (loadData == null)
            {
                StartGame();
            }

            // Nếu tải Load game lên => tiếp tục

            else
            {
                StartLoadedGame();
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DisconnectFromServer();
    }

    #endregion

    #region Game RPCs

    
    // Bắt đầu game mới khi có 2 thiết bị kết nối
    
    public void StartGame()
    {
        photonView.RPC("StartGameRPC", RpcTarget.All);
    }
    
    [PunRPC]
    void StartGameRPC()
    {
        Chess.StartNewGame();
    }
    
    public void StartLoadedGame()
    {
        photonView.RPC("StartLoadedGameRPC", RpcTarget.All, loadData);
    }

    
    // Người 1 gửi Save Game cho người 2 tải xuống
    
    [PunRPC]
    void StartLoadedGameRPC(SaveData data)
    {
        Chess.StartLoadedGame(data);
    }

    // Di chuyển quân cờ trên cả 2 thiết bị
    public void MovePiece(Vector2 piecePosition, Vector2 movePosition)
    {
        photonView.RPC("MovePieceRPC", RpcTarget.All, piecePosition, movePosition);
    }

    
    // Phần được chỉ định được di chuyển trên thiết bị này và RPC được khởi chạy để điều tương tự xảy ra trên thiết bị kia.
    
    [PunRPC]
    void MovePieceRPC(Vector2 piecePosition, Vector2 movePosition)
    {
        // Quân có vị trí được chỉ định

        Chess.SelectPiece(piecePosition);

        // Quân được di chuyển

        Chess.MovePiece(movePosition);
    }

    
    // Phong cấp con tốt trên các thiết bị
    public void PromotePiece(Enums.PromotablePieces piece, Pieces.Colour colour)
    {
        photonView.RPC("PromotePieceRPC", RpcTarget.All, piece, colour);
    }

    [PunRPC]
    void PromotePieceRPC(Enums.PromotablePieces piece, Pieces.Colour colour)
    {
        Chess.PieceSelectedToPromotion(piece, colour);
    }

    
    // Ngắt kết nối khi kết thúc
    
    public void DisconnectAll()
    {
        photonView.RPC("DisconnectAllRPC", RpcTarget.AllViaServer);
    }

    
    // Khởi chạy RPC tới máy chủ để nó ngắt kết nối tất cả người chơi cùng một lúc
    [PunRPC]
    void DisconnectAllRPC()
    {
        Chess.IsPlaying = false;
        DisconnectFromServer();
    }

    #endregion
}