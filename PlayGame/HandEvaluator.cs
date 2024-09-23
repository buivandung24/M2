using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HandRank
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush
}

public class HandEvaluator : MonoBehaviour
{
    private PotData potData;
    private DeckManager deckManager;
    List<PlayerData> player = new List<PlayerData>();
    List<CardData> communityCards = new List<CardData>();
    private void Awake() {
        potData = FindObjectOfType<PotData>();
        deckManager = FindObjectOfType<DeckManager>();
    }

    public PlayerData EvaluateStrongestHand(List<PlayerData> players, List<CardData> communityCards)
    {
        List<PlayerData> strongestPlayers = new List<PlayerData>();
        int highestHandScore = 0;

        List<PlayerData> playersCopy = new List<PlayerData>(players);
        List<CardData> communityCardsCopy = new List<CardData>(communityCards);

        // Kiểm tra từng tay bài của người chơi
        foreach (PlayerData player in playersCopy)
        {
            int handScore = EvaluateHand(player.Hand, communityCardsCopy);
            if (handScore > highestHandScore)
            {
                highestHandScore = handScore;
                strongestPlayers.Clear();
                strongestPlayers.Add(player);
            }
            else if (handScore == highestHandScore)
            {
                strongestPlayers.Add(player);
            }
        }

        // Nếu có nhiều hơn một người chơi có điểm số bài mạnh nhất, so sánh HighCard
        if (strongestPlayers.Count > 1)
        {
            return DetermineHighCardWinner(strongestPlayers, communityCardsCopy);
        }

        return strongestPlayers.FirstOrDefault();
    }

    // Phương thức mới để xác định người chiến thắng dựa trên HighCard
    private PlayerData DetermineHighCardWinner(List<PlayerData> players, List<CardData> communityCards)
    {
        Debug.Log("Determine High Card Winner");
        PlayerData highCardWinner = null;
        int highestCardValue = 0;

        foreach (PlayerData player in players)
        {
            var combinedCards = player.Hand.Concat(communityCards).OrderByDescending(card => card.NumericValue).ToList();
            // List<CardData> combinedCards = player.Hand.Concat(communityCards).OrderByDescending(card => card.NumericValue).ToList();
            int playerHighestCardValue = combinedCards.First().NumericValue;

            if (playerHighestCardValue > highestCardValue)
            {
                highestCardValue = playerHighestCardValue;
                highCardWinner = player;
            }
            else if (playerHighestCardValue == highestCardValue)
            {
                // Xử lý trường hợp khi HighCard giống nhau, so sánh card tiếp theo
                highCardWinner = CompareNextHighestCard(highCardWinner, player, combinedCards, communityCards);
            }
        }

        return highCardWinner;
    }

    private PlayerData CompareNextHighestCard(PlayerData currentWinner, PlayerData challenger, List<CardData> challengerCards, List<CardData> communityCards)
    {
        Debug.Log("Compare Next Highest Card");
        var currentWinnerCards = currentWinner.Hand.Concat(communityCards).OrderByDescending(card => card.NumericValue).ToList();
        // List<CardData> currentWinnerCards = currentWinner.Hand.Concat(communityCards).OrderByDescending(card => card.NumericValue).ToList();
        for (int i = 0; i < currentWinnerCards.Count; i++)
        {
            if (challengerCards[i].NumericValue > currentWinnerCards[i].NumericValue)
            {
                return challenger;
            }
            else if (challengerCards[i].NumericValue < currentWinnerCards[i].NumericValue)
            {
                return currentWinner;
            }
        }
        // Trường hợp hòa, có thể trả về null hoặc xử lý theo quy tắc cụ thể của trò chơi
        return null;
    }



    public PlayerData DetermineWinner()
    {
        Debug.Log("1");
        
        player = new List<PlayerData>(potData.GetEligiblePlayers());
        communityCards = new List<CardData>(deckManager.GetCommunityCards());

        Debug.Log("2");
        PlayerData winner = EvaluateStrongestHand(player, communityCards);
        Debug.Log("3");
        if(winner != null){
            Debug.Log("4");
            Debug.Log(winner.playerName.text + " has the strongest hand.");
            Debug.Log("5");
            return winner;
        }
        Debug.Log("Have no Winner");
        return null;
    }

    private int EvaluateHand(List<CardData> hand, List<CardData> communityCards)
    {
        var combinedCards = hand.Concat(communityCards).ToList();
        // Sort cards by numeric value to make it easier to check for straights
        combinedCards = combinedCards.OrderBy(card => card.NumericValue).ToList();

        // Check for each type of hand from highest to lowest
        Debug.Log("IsRoyalFlush");
        if (IsRoyalFlush(combinedCards)) return (int)HandRank.RoyalFlush;
        Debug.Log("IsStraightFlush");
        if (IsStraightFlush(combinedCards)) return (int)HandRank.StraightFlush;
        Debug.Log("IsFourOfAKind");
        if (IsFourOfAKind(combinedCards)) return (int)HandRank.FourOfAKind;
        Debug.Log("IsFullHouse");
        if (IsFullHouse(combinedCards)) return (int)HandRank.FullHouse;
        Debug.Log("IsFlush");
        if (IsFlush(combinedCards)) return (int)HandRank.Flush;
        Debug.Log("IsStraight");
        if (IsStraight(combinedCards)) return (int)HandRank.Straight;
        Debug.Log("IsThreeOfAKind");
        if (IsThreeOfAKind(combinedCards)) return (int)HandRank.ThreeOfAKind;
        Debug.Log("IsTwoPair");
        if (IsTwoPair(combinedCards)) return (int)HandRank.TwoPair;
        Debug.Log("IsOnePair");
        if (IsOnePair(combinedCards)) return (int)HandRank.OnePair;
        
        // If none of the above, return High Card
        return (int)HandRank.HighCard;
    }

    private bool IsRoyalFlush(List<CardData> cards)
    {
        return IsStraightFlush(cards) && cards.Any(card => card.NumericValue == 10) && cards.Any(card => card.NumericValue == 14);
    }

    private bool IsStraightFlush(List<CardData> cards)
    {
        return IsFlush(cards) && IsStraight(cards);
    }

    private bool IsFourOfAKind(List<CardData> cards)
    {
        return cards.GroupBy(card => card.NumericValue).Any(group => group.Count() == 4);
    }

    private bool IsFullHouse(List<CardData> cards)
    {
        var groupedCards = cards.GroupBy(card => card.NumericValue).ToList();
        return groupedCards.Count(grp => grp.Count() == 3) == 1 && groupedCards.Count(grp => grp.Count() == 2) == 1;
    }

    private bool IsFlush(List<CardData> cards)
    {
        return cards.GroupBy(card => card.Suit).Any(group => group.Count() >= 5);
    }

    private bool IsStraight(List<CardData> cards)
    {
        int countConsecutive = 0;
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if (cards[i].NumericValue + 1 == cards[i + 1].NumericValue)
            {
                countConsecutive++;
                if (countConsecutive >= 4) return true;
            }
            else if (cards[i].NumericValue != cards[i + 1].NumericValue)
            {
                countConsecutive = 0;
            }
        }
        // Check for Ace-low straight (A-2-3-4-5)
        if (cards.Any(card => card.NumericValue == 14))
        {
            List<CardData> lowAceHand = new List<CardData>(cards);
            foreach (var ace in lowAceHand.Where(card => card.NumericValue == 14))
            {
                //lowAceHand.Add(new CardData(ace.Suit, "A") { Suit = ace.Suit, NumericValue = 1 }); // Represent Ace as 1
                ace.NumericValue = 1;
            }
            lowAceHand = lowAceHand.OrderBy(card => card.NumericValue).ToList();
            return IsStraight(lowAceHand);
        }
        return false;
    }

    private bool IsThreeOfAKind(List<CardData> cards)
    {
        return cards.GroupBy(card => card.NumericValue).Any(group => group.Count() == 3);
    }

    private bool IsTwoPair(List<CardData> cards)
    {
        return cards.GroupBy(card => card.NumericValue).Count(group => group.Count() == 2) == 2;
    }

    private bool IsOnePair(List<CardData> cards)
    {
        return cards.GroupBy(card => card.NumericValue).Any(group => group.Count() == 2);
    }
}
