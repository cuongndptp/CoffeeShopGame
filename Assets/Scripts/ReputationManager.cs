using System;
using UnityEngine;

public class ReputationManager : MonoBehaviour
{
    public static ReputationManager Instance;

    private float totalRating = 0f;
    private float reputationStars = 3f; // Default: 3 stars
    private const float MinRating = -100000f; // -100,000 = 1 Star
    private const float MaxRating = 100000f;  //  100,000 = 5 Stars
    private const float ScalingFactor = 25000f; // Adjusts curve steepness

    public event EventHandler<OnReputationChangedEventArgs> OnReputationChanged;
    public class OnReputationChangedEventArgs : EventArgs
    {
        public float reputationStars;
    }
    public event EventHandler<OnRatingChangedEventArgs> OnRatingChanged;

    public class OnRatingChangedEventArgs : EventArgs
    {
        public float totalRating;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateReputationLevel();
    }

    /// <summary>
    /// Gets the current Total Rating.
    /// </summary>
    public float GetTotalRating()
    {
        return totalRating;
    }

    /// <summary>
    /// Sets Total Rating directly (clamped to valid range).
    /// </summary>
    public void SetTotalRating(float newRating)
    {
        totalRating = Mathf.Clamp(newRating, MinRating, MaxRating);
        OnRatingChanged?.Invoke(this, new OnRatingChangedEventArgs
        {
            totalRating = totalRating
        });
        UpdateReputationLevel();
    }

    /// <summary>
    /// Adds to the Total Rating (clamped).
    /// </summary>
    public void AddRating(float value)
    {
        SetTotalRating(totalRating + value);
        Debug.Log("Total rating: " + totalRating);
    }

    /// <summary>
    /// Subtracts from the Total Rating (clamped).
    /// </summary>
    public void SubtractRating(float value)
    {
        SetTotalRating(totalRating - value);
    }

    /// <summary>
    /// Updates the reputation level based on TotalRating using an arctangent formula.
    /// </summary>
    private void UpdateReputationLevel()
    {
        float normalizedRating = Mathf.Atan(totalRating / ScalingFactor) / Mathf.Atan(MaxRating / ScalingFactor);
        reputationStars = 3f + 2f * normalizedRating;

        OnReputationChanged?.Invoke(this, new OnReputationChangedEventArgs
        {
            reputationStars = reputationStars
        });
    }

    /// <summary>
    /// Gets the current Reputation Level as an integer (1 to 5 stars).
    /// </summary>
    public int GetReputationLevel()
    {
        return Mathf.RoundToInt(reputationStars);
    }

    /// <summary>
    /// Gets Reputation as a float (1.0 to 5.0 stars, smooth progression).
    /// </summary>
    public float GetReputationFloat()
    {
        return reputationStars;
    }
}
