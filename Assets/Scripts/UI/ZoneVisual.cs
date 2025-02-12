using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneVisual : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        Player.Instance.OnPlayerModeChanged += Instance_OnPlayerModeChanged;
    }

    private void Instance_OnPlayerModeChanged(object sender, Player.OnPlayerModeChangedEventArgs e)
    {
        if(e.mode == Player.Mode.Put)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
