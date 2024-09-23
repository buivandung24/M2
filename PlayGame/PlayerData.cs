using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerData : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public TMP_Text playerName;
    public TMP_Text playerMoney;
    public Button buttonTurnOnInfoPersonal;
    
    public PhotonView photonViewPlayer;
    //card
    public List<CardData> Hand = new List<CardData>();
    public List<Image> playerCardsImages; // Images UI for displaying player's cards
    public Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>(); // Dictionary để lưu các sprite với key là "Value_Suit"
    
    //check can play
    public bool myTurnDisplayCardHand = false;
    //public bool GameStateIsEndHand = false;
    public bool canPlay;
    public bool HasChips { get; set; }
    public bool HasFolded { get; set; }
    public Photon.Realtime.Player photonPlayer { get; set; }
    // class
    private PlayerCreator playerCreator;
    public UIManager uIManager;
    public PotData potData;
    public PotManager potManager;
    public GamePlayManager gamePlayManager;
    public ButtonController buttonController;
 
    private void Awake() {
        //GameStateIsEndHand = false;
        canPlay = false;
        HasFolded = false;
        playerCreator = FindObjectOfType<PlayerCreator>();
        uIManager = FindObjectOfType<UIManager>();
        potData = FindObjectOfType<PotData>();
        potManager = FindObjectOfType<PotManager>();
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        LoadCardSprites();
        buttonTurnOnInfoPersonal.onClick.AddListener(TurnOnInfoPersonal);
    }

    private void Update(){
        playerMoney.text = Chips + "";

        if(Chips > 0){
            HasChips = true;
        }else{
            HasChips = false;
        }

        if(HasChips == false || HasFolded == true){
            canPlay = false;
        }
    }
    //button
    private void TurnOnInfoPersonal(){
        buttonController.TurnOnScene(uIManager.darkScene);
        buttonController.TurnOnScene(uIManager.InfoPersonalScene);
    }

    //photon

    public void OnSetCanPlay() {
        photonViewPlayer.RPC("SetCanPlay", RpcTarget.All, true);
    }

    [PunRPC]
    public void SetCanPlay(bool value)
    {
        canPlay = value;
    }

    [PunRPC]
    public void SetTurnDisplayCardHand(bool value)
    {
        myTurnDisplayCardHand = value;
    }

    protected virtual void LoadNickName(){
        this.playerName.text = this.photonView.Owner.NickName;
    }
    
    //card
    [PunRPC]
    public void ResetCardHand(){
        Hand.Clear();
        for (int i = 0; i < playerCardsImages.Count; i++){
            playerCardsImages[i].gameObject.SetActive(false);
        }
    }

    public List<CardData> GetCardHand(){
        return new List<CardData>(Hand);
    }

    // Function to add a card to the player's hand

    public void AddCardToHand(CardData card) {
        Hand.Add(card);
        Debug.Log("get card done!!!!");
    }

    [PunRPC]
    public void RpcAddCardToHand(string suit, string value) {
        CardData getCard = new CardData(suit, value); // Giả sử bạn có một cơ sở dữ liệu bài
        AddCardToHand(getCard);
        Debug.Log("card received: " + suit + "_" + value);
    }

    [PunRPC]
    public void UpdateCardDisplay()
    {
        if(photonViewPlayer.IsMine){
            for (int i = 0; i < Hand.Count; i++)
            {
                string key = Hand[i].Suit + "_" + Hand[i].Value; // Tạo key để truy cập dictionary
                if (cardSprites.TryGetValue(key, out Sprite cardSprite))
                {
                    playerCardsImages[i].sprite = cardSprite; // Cập nhật sprite cho Image component
                    playerCardsImages[i].gameObject.SetActive(true); // Đảm bảo rằng Image component được hiển thị
                    Debug.Log("display card");
                }
                else
                {
                    Debug.LogError("Sprite not found for card: " + key);
                }
            }
        }
    }

    [PunRPC]
    public void ShowdownCardDisplay()
    {
        for (int i = 0; i < Hand.Count; i++)
        {
            string key = Hand[i].Suit + "_" + Hand[i].Value; // Tạo key để truy cập dictionary
            if (cardSprites.TryGetValue(key, out Sprite cardSprite))
            {
                playerCardsImages[i].sprite = cardSprite; // Cập nhật sprite cho Image component
                playerCardsImages[i].gameObject.SetActive(true); // Đảm bảo rằng Image component được hiển thị
                Debug.Log("display card");
            }
            else
            {
                Debug.LogError("Sprite not found for card: " + key);
            }
        }
    }

    private void LoadCardSprites()
    {
        // Load tất cả các sprite từ file "Deck_1" trong thư mục "Resources"
        Sprite[] sprites = Resources.LoadAll<Sprite>("Deck_1");

        // Thêm các sprite vào dictionary
        foreach (Sprite sprite in sprites)
        {
            cardSprites[sprite.name] = sprite;
        }
    }

    //add chips
    public int Chips = 100000;

    [PunRPC]
    public void AddChips(int amount)
    {
        Chips += amount;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        StartCoroutine(createPlayer(info));
    }

    IEnumerator createPlayer(PhotonMessageInfo info)
    {
        yield return new WaitForSeconds(0.1f);
        //base.OnPhotonInstantiate(info);
        Debug.Log("OnPhotonInstantiate hoạt động");
        GameObject player = info.photonView.gameObject;
        //int spawnIndex = (int)info.photonView.InstantiationData[0]; // Ensure this matches the index in the InstantiationData you set earlier.
        int spawnIndex = playerCreator.FindAvailableSeat();
        RectTransform rectTransform = player.GetComponent<RectTransform>();

        player.transform.SetParent(playerCreator.spawnPoints[spawnIndex]);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition = Vector3.zero;

        LoadNickName();
        Debug.Log("Prefab đã được tạo");
    }

    [PunRPC]
    public void OnsetHasFolded(bool value){
        HasFolded = value;
    }
}
