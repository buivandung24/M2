using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Thư viện của Photon
using PlayFab;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping; // Thư viện của PlayFab

public enum GameState
{
    WaitingForPlayers,
    PreFlop,
    Flop,
    Turn,
    River,
    Showdown,
    EndHand
}

public class GamePlayManager : MonoBehaviourPunCallbacks
{
    public static GamePlayManager instance;
    //
    private bool canAction = true;
    public GameObject startGameButton;
    public GameObject myPlayer;
    public PhotonView photonViewGameManager;
    // public GameObject darkScene;
    // public GameObject InfoPersonalScene;

    // Biến lưu trạng thái hiện tại của game
    private GameState currentState;
    private List<PlayerData> players = new List<PlayerData>();
    public List<GameObject> PlayersInOrder = new List<GameObject>();
    public List<Transform> seatPositions; // Assign seat positions in clockwise order in the Inspector
    private bool isEndBettingRoundDone;

    // class
    private UIManager uIManager;
    private DeckManager deckManager;
    private PotManager potManager;
    private PlayerCreator playerCreator;
    private HandEvaluator handEvaluator;
    private PotData potData;

    // Player
    public int currentPlayerIndex = -1;
    private int indexMasterClient;
    public GameObject playerMasterClient;
    private void Awake()
    {
        instance = this;
        // Thiết lập trạng thái khởi đầu của game
        currentState = GameState.WaitingForPlayers;
        
        // Khởi tạo và setup các biến khác
        uIManager = FindObjectOfType<UIManager>();
        deckManager = FindObjectOfType<DeckManager>();
        potManager = FindObjectOfType<PotManager>();
        playerCreator = FindObjectOfType<PlayerCreator>();
        potData = FindObjectOfType<PotData>();
        handEvaluator = FindObjectOfType<HandEvaluator>();
    }

    // Phương thức cập nhật được gọi mỗi khung hình
    private void Update()
    {
        switch (currentState)
        {
            case GameState.WaitingForPlayers:
                // Chờ đủ người chơi
                CheckCanPlayGame();
                WaitingForPlayers();
                break;
            case GameState.PreFlop:
                startGameButton.gameObject.SetActive(false);
                // Quản lý lượt chơi ở giai đoạn PreFlop
                PreFlop();
                // Kiểm tra điều kiện chuyển sang Flop
                if (IsReadyForNextStage())
                {
                    //currentState = GameState.Flop;
                    photonViewGameManager.RPC("UpdateCurrentState", RpcTarget.All, GameState.Flop);
                    Debug.Log("GameState.Flop");
                }
                break;
            case GameState.Flop:
                // Quản lý lượt chơi ở giai đoạn Flop
                Flop();
                // Kiểm tra điều kiện chuyển sang Turn
                if (IsReadyForNextStage())
                {
                    //currentState = GameState.Turn;
                    photonViewGameManager.RPC("UpdateCurrentState", RpcTarget.All, GameState.Turn);
                    Debug.Log("GameState.Turn");
                }
                //Debug.Log("GameState.Flop");
                break;
            case GameState.Turn:
                // Quản lý lượt chơi ở giai đoạn Turn
                Turn();
                // Kiểm tra điều kiện chuyển sang River
                if (IsReadyForNextStage())
                {
                    //currentState = GameState.River;
                    photonViewGameManager.RPC("UpdateCurrentState", RpcTarget.All, GameState.River);
                    Debug.Log("GameState.River");
                }
                //Debug.Log("GameState.Turn");
                break;
            case GameState.River:
                // Quản lý lượt chơi ở giai đoạn River
                River();
                // Kiểm tra điều kiện chuyển sang Showdown
                if (IsReadyForNextStage())
                {
                    //currentState = GameState.Showdown;
                    photonViewGameManager.RPC("UpdateCurrentState", RpcTarget.All, GameState.Showdown);
                    Debug.Log("GameState.Showdown");
                }
                //Debug.Log("GameState.River");
                break;
            case GameState.Showdown:
                //Debug.Log("GameState.Showdown");
                // Quản lý việc so bài và xác định người thắng
                Showdown();
                // Chuyển sang giai đoạn kết thúc ván bài
                // if (IsReadyForNextStage())
                // {
                //     //currentState = GameState.EndHand;
                //     photonViewGameManager.RPC("UpdateCurrentState", RpcTarget.All, GameState.EndHand);
                // }
                //currentState = GameState.EndHand;
                break;
            case GameState.EndHand:
                //Debug.Log("GameState.EndHand");
                // Xử lý kết thúc ván bài
                EndHand();
                // Chuẩn bị cho ván bài tiếp theo hoặc kết thúc game
                break;
            default:
                throw new System.Exception("Unhandled game state!");
        }
    }

    // Kiểm tra xem có đủ điều kiện để tiến lên giai đoạn tiếp theo hay không
    private bool IsReadyForNextStage()
    {
        if(isEndBettingRoundDone){
            isEndBettingRoundDone = false;
            canAction = true;
            return true;
        }
        return false;
    }

    private void CheckCanPlayGame(){
        if(PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.IsMasterClient){
            startGameButton.gameObject.SetActive(true);
        } else {
            startGameButton.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void SetCanAction(bool value){
        canAction = value;
    }

    [PunRPC]
    public void StartGame(){
        //currentState = GameState.PreFlop;
        photonViewGameManager.RPC("UpdateCurrentState", RpcTarget.All, GameState.PreFlop);
        UpdatePlayerList();
        UpdatePlayersInOrder();
        canAction = true;
    }

    [PunRPC]
    public void UpdateCurrentState(GameState gameStateUpdate){
        //currentState = GameState.PreFlop;
        currentState = gameStateUpdate;
    }

    void UpdatePlayerList(){
        players.Clear(); // Xóa danh sách cũ trước khi cập nhật
        if (PhotonNetwork.InRoom)
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                PlayerData playerData = new PlayerData();
                players.Add(playerData);
            }
        }
    }

    private void UpdatePlayersInOrder(){
        PlayersInOrder = GetPlayersInOrder();
    }

    [PunRPC]
    private void getPlayerMasterClient(int indexMasterClient){
        playerMasterClient = PlayersInOrder[indexMasterClient];
    }

    private int getindexMasterClient(){
        for(int i=0; i<PlayersInOrder.Count; i++){
            if(PhotonNetwork.IsMasterClient && PlayersInOrder[i] == myPlayer){
                return i;
            }
        }
        return -1;
    }

    private void PreFlop(){
        if(canAction && PhotonNetwork.IsMasterClient){
            canAction = false;
            //get master
            indexMasterClient = getindexMasterClient();
            photonViewGameManager.RPC("getPlayerMasterClient", RpcTarget.All, indexMasterClient);
            //deck
            //myPlayer.GetComponent<PlayerData>().photonViewPlayer.RPC("ResetCardHand", RpcTarget.All);

            deckManager.CreateDeck();
            deckManager.ShuffleDeck();
            deckManager.DealCardsToPlayers(PlayersInOrder,2);
            foreach (GameObject player in PlayersInOrder){
                PhotonView photonViewPlay = player.GetComponent<PlayerData>().photonViewPlayer;
                photonViewPlay.RPC("UpdateCardDisplay", RpcTarget.All);
            }
            //Start Betting Round
            StartBettingRound(PlayersInOrder);
        }
    }

    private void Flop(){
        if(canAction && PhotonNetwork.IsMasterClient){
            canAction = false;
            Debug.Log("Start Deal CommunityCards");
            deckManager.DealCommunityCards(3);
            deckManager.photonViewDeckManager.RPC("UpdateCommunityCards", RpcTarget.All);
            Debug.Log("Start BettingRound");
            StartBettingRound(PlayersInOrder);
            Debug.Log("Done Flop!!!!");
        }
    }

    private void Turn(){
        if(canAction && PhotonNetwork.IsMasterClient){
            canAction = false;
            Debug.Log("Start Deal CommunityCards");
            deckManager.DealCommunityCards(1);
            deckManager.photonViewDeckManager.RPC("UpdateCommunityCards", RpcTarget.All);
            Debug.Log("Start BettingRound");
            StartBettingRound(PlayersInOrder);
            Debug.Log("Done Flop!!!!");
        }
    }

    private void River(){
        if(canAction && PhotonNetwork.IsMasterClient){
            canAction = false;
            Debug.Log("Start Deal CommunityCards");
            deckManager.DealCommunityCards(1);
            deckManager.photonViewDeckManager.RPC("UpdateCommunityCards", RpcTarget.All);
            Debug.Log("Start BettingRound");
            StartBettingRound(PlayersInOrder);
            Debug.Log("Done Flop!!!!");
        }
    }

    private void Showdown(){
        if(canAction && PhotonNetwork.IsMasterClient){
            canAction = false;
            Debug.Log("Start showDown");
            currentPlayerIndex = 0;
            photonViewGameManager.RPC("updateCurrentPlayerIndex", RpcTarget.All, currentPlayerIndex);
            potData.EligiblePlayers[currentPlayerIndex].GetComponent<PlayerData>().photonViewPlayer.RPC("SetTurnDisplayCardHand", RpcTarget.All, true);
            Debug.Log("Done set turn");
        }

        if(myPlayer.GetComponent<PlayerData>().myTurnDisplayCardHand){
            Debug.Log(myPlayer.GetComponent<PlayerData>().playerName.text + " Send card");
            foreach (CardData card in myPlayer.GetComponent<PlayerData>().Hand)
            {
                myPlayer.GetComponent<PlayerData>().photonViewPlayer.RPC("RpcAddCardToHand", RpcTarget.Others, card.Suit, card.Value);
            }

            myPlayer.GetComponent<PlayerData>().photonViewPlayer.RPC("ShowdownCardDisplay", RpcTarget.Others);
            myPlayer.GetComponent<PlayerData>().photonViewPlayer.RPC("SetTurnDisplayCardHand", RpcTarget.All, false);
            

            if(currentPlayerIndex < potData.EligiblePlayers.Count){
                currentPlayerIndex++;
                photonViewGameManager.RPC("updateCurrentPlayerIndex", RpcTarget.All, currentPlayerIndex);
                potData.EligiblePlayers[currentPlayerIndex].photonViewPlayer.RPC("SetTurnDisplayCardHand", RpcTarget.All, true);
            }
        }
        
        // GoToEndHand();

        if(currentPlayerIndex == potData.EligiblePlayers.Count && PhotonNetwork.IsMasterClient){
            Debug.Log("Done Send card");
            GoToEndHand();
        }
    }

    private void GoToEndHand(){
        //if(PhotonNetwork.IsMasterClient && currentPlayerIndex == potData.EligiblePlayers.Count){
            Debug.Log("ready go to EndHand");
            canAction = true;
            photonViewGameManager.RPC("UpdateCurrentState", RpcTarget.All, GameState.EndHand);
            Debug.Log("GameState.EndHand");
        //}
    }

    private void EndHand(){
        // if(!myPlayer.GetComponent<PlayerData>().GameStateIsEndHand)
        // myPlayer.GetComponent<PlayerData>().photonViewPlayer.RPC("SetGameStateIsEndHand", RpcTarget.All, true);
        
        if(canAction && PhotonNetwork.IsMasterClient){
            canAction = false;
            Debug.Log("Start Determine Winner");
            PlayerData winner = handEvaluator.DetermineWinner();
            Debug.Log("1");
            if(winner != null){
                Debug.Log("Done Determine Winner");
                winner.photonViewPlayer.RPC("AddChips", RpcTarget.All, potData.Amount);
                Debug.Log("2");
                StartCoroutine(ResetGame());
            } else Debug.Log("Null Winner");
        }
    }

    private IEnumerator ResetGame(){
        yield return new WaitForSeconds(5f);
        canAction = true;
        photonViewGameManager.RPC("UpdateCurrentState", RpcTarget.All, GameState.WaitingForPlayers);
        photonViewGameManager.RPC("SetCanAction", RpcTarget.All, true);
        
        Debug.Log("GameState.WaitingForPlayers");
    }

    private void WaitingForPlayers(){
        if(canAction){
            canAction = false;
            potManager.CreatePot();
            uIManager.resetBet();
            myPlayer.GetComponent<PlayerData>().photonViewPlayer.RPC("ResetCardHand", RpcTarget.All);
            uIManager.potTextMain.text = "0";
            deckManager.resetCommunityCards();
        }
    }

    // Call this to get the list of players in clockwise order
    public List<GameObject> GetPlayersInOrder()
    {
        List<GameObject> playersInOrder = new List<GameObject>();

        // Loop through each seat position
        foreach (Transform seat in seatPositions)
        {
            // Check if the seat has a player (child object)
            if (seat.childCount > 0)
            {
                // Add the player object to the list
                playersInOrder.Add(seat.GetChild(0).gameObject);
            }
        }

        return playersInOrder;
    }

    [PunRPC]
    private void updateCurrentPlayerIndex(int value){
        currentPlayerIndex = value;
    }

    // Gọi hàm này để bắt đầu lượt cược
    public void StartBettingRound(List<GameObject> PlayersInOrder)
    {
        Debug.Log("Start Betting Round");
        isEndBettingRoundDone = false;
        //currentPlayerIndex = 0; // Bắt đầu từ người chơi đầu tiên
        photonViewGameManager.RPC("updateCurrentPlayerIndex", RpcTarget.All, 0);
        ProceedToNextPlayer(PlayersInOrder);
    }

    // Chuyển đến người chơi tiếp theo
    private void ProceedToNextPlayer(List<GameObject> PlayersInOrder)
    {
        // Kiểm tra xem đã đến cuối danh sách chưa
        if (currentPlayerIndex >= PlayersInOrder.Count)
        {
            EndBettingRound();
            return;
        }

        // Lấy người chơi tiếp theo
        PlayerData currentPlayer = PlayersInOrder[currentPlayerIndex].GetComponent<PlayerData>();

        // Kiểm tra xem người chơi có thể cược hay không (ví dụ: không phải là người chơi đã bỏ bài)
        if (CanPlayerBet(currentPlayer))
        {
            PhotonView playerPhoton = PlayersInOrder[currentPlayerIndex].GetComponent<PhotonView>();
            //GameObject playerObject = PlayersInOrder[currentPlayerIndex];
            PlayerTurn(playerPhoton);
            StartTurn(playerPhoton, PlayersInOrder, currentPlayerIndex);
            //OnPlayerBetCompleted(playerPhoton, PlayersInOrder);
        }
        else
        {
            // Nếu người chơi không thể cược, chuyển đến người tiếp theo
            Debug.Log("can't play player in " + currentPlayerIndex);
            currentPlayerIndex++;
            photonViewGameManager.RPC("updateCurrentPlayerIndex", RpcTarget.All, currentPlayerIndex);
            ProceedToNextPlayer(PlayersInOrder);
        }
    }


    // Kiểm tra xem người chơi có thể cược hay không
    private bool CanPlayerBet(PlayerData player)
    {
        return player.HasChips && !player.HasFolded;
    }

    public void OnPlayerBetCompleted(PhotonView playerPhoton, List<GameObject> PlayersInOrder)
    {
        playerPhoton.RPC("SetCanPlay", RpcTarget.All, false);
        currentPlayerIndex++;
        photonViewGameManager.RPC("updateCurrentPlayerIndex", RpcTarget.All, currentPlayerIndex);
        ProceedToNextPlayer(PlayersInOrder);
        Debug.Log("Bet Completed");
    }

    // Gọi hàm này để kết thúc vòng cược
    private void EndBettingRound()
    {
        uIManager.photonViewUI.RPC("UpdatePot", RpcTarget.All);
        isEndBettingRoundDone = true;
        Debug.Log("done betting round");
    }

    public void PlayerTurn(PhotonView playerPhoton)
    {
        playerPhoton.RPC("SetCanPlay", RpcTarget.All, true);
    }

    // Hàm được gọi để bắt đầu lượt của người chơi
    public void StartTurn(PhotonView playerPhoton, List<GameObject> PlayersInOrder, int currentPlayerIndex)
    {
        uIManager.turnTimer = uIManager.turnDuration; // Đặt lại đồng hồ đếm ngược
        //PlayerTurn(playerPhoton);
        StartCoroutine(TurnTimer(playerPhoton, PlayersInOrder, currentPlayerIndex)); // Bắt đầu đếm thời gian
        //OnPlayerBetCompleted(playerPhoton, PlayersInOrder);
    }

    // Coroutine để đếm ngược thời gian lượt chơi
    private IEnumerator TurnTimer(PhotonView playerPhoton, List<GameObject> PlayersInOrder, int currentPlayerIndex)
    {
        //PlayerTurn(playerPhoton);
        while (uIManager.turnTimer > 0)
        {
            uIManager.photonViewUI.RPC("UpdateTimerDisplay", RpcTarget.All, uIManager.turnTimer);
            yield return new WaitForSeconds(1f);
            uIManager.turnTimer -= 1f;
            //UpdateTimerDisplay(uIManager.turnTimer);
        }
        // Hết thời gian, mặc định là người chơi bỏ bài        
        if(uIManager.turnTimer == 0){
            playerMasterClient.GetComponent<PlayerData>().uIManager.photonViewUI.RPC("PlayerFold", RpcTarget.All, currentPlayerIndex);
        }
        OnPlayerBetCompleted(playerPhoton, PlayersInOrder);
    }

    // Cập nhật giao diện thời gian còn lại
    [PunRPC]
    private void UpdateTimerDisplay(float timeLeft)
    {
        if (uIManager.timeCountDown != null)
        {
            uIManager.timeCountDown.text = timeLeft.ToString("0");
        }
    }
    [PunRPC]
    public void OnStopAllCoroutines(){
        //StopAllCoroutines();
        //OnPlayerBetCompleted(playerPhoton, PlayersInOrder);
        uIManager.turnTimer = -1;
    }

    //LeaveRoom

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            // Hủy nhân vật người chơi nếu tồn tại
            if (myPlayer != null && myPlayer.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(myPlayer);
            }

            // Rời phòng
            PhotonNetwork.LeaveRoom();
        }
    }

    // Override OnLeftRoom của Photon để xử lý khi người chơi đã rời khỏi phòng
    public override void OnLeftRoom()
    {
        // Chuyển người chơi về menu chính hoặc màn hình chờ khác
        PhotonNetwork.LoadLevel("Lobby");
    }

    // Được gọi khi một người chơi khác đã rời khỏi phòng. Giúp cập nhật trạng thái của trò chơi.
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // Cập nhật danh sách người chơi hoặc trạng thái trò chơi nếu cần
        UpdatePlayerList();
        UpdatePlayersInOrder();
        
        // Kiểm tra và xử lý nếu số lượng người chơi dưới mức tối thiểu cần thiết để tiếp tục trò chơi
        // Ví dụ: nếu người chơi cần ít nhất 2 người để tiếp tục
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            // Xử lý việc không đủ người chơi, ví dụ như thông báo cho người chơi còn lại hoặc tự động kết thúc trò chơi
        }
    }
    
}
