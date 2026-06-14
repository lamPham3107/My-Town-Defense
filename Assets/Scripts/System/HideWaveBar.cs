using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWaveBar : MonoBehaviour
{
    public void HideBar()
    {
        WaveManager.instance._wave_Bar.SetActive(false);
    }
}
