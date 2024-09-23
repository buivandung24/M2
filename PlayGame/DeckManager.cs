using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class DeckManager : MonoBehaviour {
    private List<CardData> deck = new List<CardData>();
    public Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();
    public List<Image> communityCardsImage;
    public List<CardData> communityCards = new List<CardData>();
    public PhotonView photonViewDeckManager;
    private string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
    private string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

    void Start() {
        // CreateDeck();
        // ShuffleDeck();
        LoadCardSprites();
    }

    // Function to create a new deck of 52 cards
    public void CreateDeck() {
        if(PhotonNetwork.IsMasterClient){
            deck.Clear();
            foreach (string suit in suits) {
                foreach (string value in values) {
                    deck.Add(new CardData(suit, value));
                }
            }
        }
    }

    // Function to shuffle the deck
    public void ShuffleDeck() {
        if(PhotonNetwork.IsMasterClient){
            for (int i = 0; i < deck.Count; i++) {
                CardData temp = deck[i];
                int randomIndex = Random.Range(i, deck.Count);
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
            }
        }
    }

    // Function to deal a single card from the deck
    public CardData DealCard() {
        if (deck.Count > 0) {
            CardData cardToDeal = deck[0];
            deck.RemoveAt(0);
            return cardToDeal;
        } else {
            Debug.LogWarning("Deck is empty!");
            return null;
        }
    }

    // Function to deal cards to players
    public void DealCardsToPlayers(List<GameObject> players, int cardsPerPlayer) {
        if(PhotonNetwork.IsMasterClient){
            for (int i = 0; i < cardsPerPlayer; i++) {
                foreach (GameObject player in players) {
                    PhotonView photonViewPlayer = player.GetComponent<PhotonView>();
                    CardData card = DealCard();
                    if (card != null && photonViewPlayer != null) {
                        photonViewPlayer.RPC("RpcAddCardToHand", photonViewPlayer.Owner, card.Suit, card.Value);
                    }
                }
            }
        }
    }
    //CommunityCards
    public void DealCommunityCards(int NumberOfCards){
        if(PhotonNetwork.IsMasterClient){
            for (int i = 0; i < NumberOfCards; i++) {
                CardData card = DealCard();
                if (card != null) {
                    photonViewDeckManager.RPC("RpcAddCommunityCards", RpcTarget.All, card.Suit, card.Value);
                }
            }
        }
    }

    [PunRPC]
    public void UpdateCommunityCards()
    {
        Debug.Log("start Update CommunityCards");
        //if(photonViewDeckManager.IsMine){
            for (int i = 0; i < communityCards.Count; i++)
            {
                string key = communityCards[i].Suit + "_" + communityCards[i].Value; // Tạo key để truy cập dictionary
                if (cardSprites.TryGetValue(key, out Sprite cardSprite))
                {
                    communityCardsImage[i].sprite = cardSprite; // Cập nhật sprite cho Image component
                    communityCardsImage[i].gameObject.SetActive(true); // Đảm bảo rằng Image component được hiển thị
                    Debug.Log("display card");
                }
                else
                {
                    Debug.LogError("Sprite not found for card: " + key);
                }
            }
        //}
        Debug.Log("Done Update CommunityCards");
    }

    [PunRPC]
    public void RpcAddCommunityCards(string suit, string value) {
        CardData getCard = new CardData(suit, value); // Giả sử bạn có một cơ sở dữ liệu bài
        communityCards.Add(getCard);
        Debug.Log("CommunityCards received: " + suit + "_" + value);
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

    public void resetCommunityCards(){
        foreach (Image card in communityCardsImage)
        {
            card.gameObject.SetActive(false);
        }
        communityCards.Clear();
    }

    public List<CardData> GetCommunityCards(){
        return new List<CardData>(communityCards);
    }
}
