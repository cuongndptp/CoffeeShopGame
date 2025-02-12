using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public static SeatManager Instance;
    private void Awake()
    {
        Instance = this;
        allSeatPoints = new List<SeatPoint>();
    }

    [SerializeField]
    private List<Seat> seats;
    private List<SeatPoint> allSeatPoints;
    private List<SeatPoint> availableSeatPoints = new List<SeatPoint>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var seat in seats)
        {
            foreach (var seatPoint in seat.GetSeatPointList())
            {
                allSeatPoints.Add(seatPoint);

                // Subscribe to the availability change event
                seatPoint.OnSeatPointAvailable += SeatPoint_OnSeatPointAvailable;
                seatPoint.OnSeatPointUnavailable += SeatPoint_OnSeatPointUnavailable; ;
            }
        }
        AddAvailableSeatPoints();
    }

    private void SeatPoint_OnSeatPointUnavailable(object sender, EventArgs e)
    {
        RemoveUnavailableSeatPoints();
    }

    private void SeatPoint_OnSeatPointAvailable(object sender, EventArgs e)
    {
        AddAvailableSeatPoints();
    }

    private void RemoveUnavailableSeatPoints()
    {
        availableSeatPoints.RemoveAll(seatPoint => seatPoint.IsAvailable() == false);
    }

    private void AddAvailableSeatPoints()
    {
        availableSeatPoints.Clear();
        foreach (var seatPoint in allSeatPoints)
        {
            if (seatPoint.IsAvailable() )
            {
                if (!availableSeatPoints.Contains(seatPoint))
                {
                    availableSeatPoints.Add(seatPoint);
                }
            }
            else
            {
            }
        }
    }

    public SeatPoint GetAvailableSeatPoint()
    {
        if (availableSeatPoints != null && availableSeatPoints.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableSeatPoints.Count);
            SeatPoint seatPoint = null;
            seatPoint = availableSeatPoints[randomIndex];
            return seatPoint;
        }
        return null;
    }

}
