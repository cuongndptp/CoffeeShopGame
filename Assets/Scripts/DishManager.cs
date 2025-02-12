using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    [SerializeField] private List<DishSO> allDishSOList;

    public static DishManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<DishSO> GetAllDishSO()
    {
        return allDishSOList;
    }
}
