using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        CycleCounter.CounterInstance.cycleCount++;
        Debug.Log("Current Cycle Count: " + CycleCounter.CounterInstance.cycleCount);
        SceneManager.LoadScene("Connor Test");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}