using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

public class RaiseSliderController : MonoBehaviour
{
    [SerializeField] private Slider raiseSlider; // Attach the raise slider UI component
    [SerializeField] private TMP_Text raiseText; // Attach the raise amount TMP_Text UI component
    [SerializeField] private Button btnDouble, btnQuadruple, btnOctuple, btnHexadecuple, btnAllIn; // Attach the respective buttons

    private float minRaiseAmount = 1; // Minimum raise amount, should be set according to game rules
    private float maxRaiseAmount = 5; // Maximum raise amount, should be set according to player's chips

    private void Start()
    {
        // Initialize the slider values
        raiseSlider.minValue = minRaiseAmount;
        raiseSlider.maxValue = maxRaiseAmount;
        raiseSlider.value = minRaiseAmount;

        UpdateRaiseText();

        // Subscribe button click events to their respective handlers
        btnDouble.onClick.AddListener(() => SetRaiseMultiplier(1));
        btnQuadruple.onClick.AddListener(() => SetRaiseMultiplier(2));
        btnOctuple.onClick.AddListener(() => SetRaiseMultiplier(3));
        btnHexadecuple.onClick.AddListener(() => SetRaiseMultiplier(4));
        btnAllIn.onClick.AddListener(SetAllIn);

        // Subscribe to the slider's value changed event
        raiseSlider.onValueChanged.AddListener(delegate { UpdateRaiseText(); });
    }

    // This method updates the TMP_Text to show the current raise amount
    private void UpdateRaiseText()
    {
        if(raiseSlider.value != maxRaiseAmount) raiseText.text = math.pow(2, raiseSlider.value).ToString() + "X";
        else raiseText.text = "ALL IN";
    }
    // This method calculates and sets the slider value based on the chosen multiplier
    private void SetRaiseMultiplier(int multiplier)
    {
        float newRaiseAmount = minRaiseAmount * multiplier;
        newRaiseAmount = Mathf.Min(newRaiseAmount, maxRaiseAmount); // Ensure it doesn't exceed the max raise amount
        raiseSlider.value = newRaiseAmount;
    }

    // This method sets the slider value to the all-in amount
    private void SetAllIn()
    {
        raiseSlider.value = maxRaiseAmount;
    }

    // Call this method to update the min and max raise amounts when needed
    public void UpdateRaiseLimits(float minAmount, float maxAmount)
    {
        minRaiseAmount = minAmount;
        maxRaiseAmount = maxAmount;
        raiseSlider.minValue = minAmount;
        raiseSlider.maxValue = maxAmount;

        // Reset the slider and text to min raise amount
        raiseSlider.value = minAmount;
        UpdateRaiseText();
    }

    public int getValueRaise(){
        return (int)math.pow(2, raiseSlider.value);
    }
}