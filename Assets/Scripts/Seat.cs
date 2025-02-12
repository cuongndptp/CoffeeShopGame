using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField] private List<SeatPoint> seatPointList;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<SeatPoint> GetSeatPointList()
    {
        return seatPointList;
    }
}
