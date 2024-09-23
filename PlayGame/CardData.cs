using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public string Suit; // e.g., "Hearts", "Diamonds", "Clubs", "Spades"
    public string Value; // e.g., "2", "3", ..., "10", "J", "Q", "K", "A"
    public int NumericValue;

    public CardData(string suit, string value) {
        Suit = suit;
        Value = value;
        SetNumericValue();
    }

    private void SetNumericValue() {
        // Check if the value is a number and convert it. If not, check for face cards
        if (int.TryParse(Value, out int numericValue)) {
            NumericValue = numericValue;
        } else {
            switch (Value) {
                case "J":
                    NumericValue = 11;
                    break;
                case "Q":
                    NumericValue = 12;
                    break;
                case "K":
                    NumericValue = 13;
                    break;
                case "A":
                    // Assuming Ace is high
                    NumericValue = 14;
                    break;
                default:
                    Debug.LogError("Invalid card value: " + Value);
                    break;
            }
        }
    }
}
