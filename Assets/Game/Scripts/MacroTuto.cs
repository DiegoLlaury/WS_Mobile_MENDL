using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

public class MacroTuto : MonoBehaviour
{
    public List<GameObject> panelList;
    private int tutoStep = -1;
    public bool inMacro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inMacro)
        {
            StartCoroutine(wait(1f));
        }
        else if (GameManager.currentEvent.tuto)
        {
            StartCoroutine(wait(1f));
        }
    }

    public void Next()
    {
        tutoStep++;
        if (tutoStep < panelList.Count)
        {
            panelList[tutoStep].SetActive(true);
        }

    }

    public void waitAndNext(float time)
    {
        if (tutoStep >= 0)
        {
            panelList[tutoStep].SetActive(false);
        }
        StartCoroutine(wait(time));
    }

    public IEnumerator wait(float delay)
    {
        yield return new WaitForSeconds(delay);

        Next();
    }
}
