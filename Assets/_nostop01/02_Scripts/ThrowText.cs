using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThrowText : MonoBehaviour
{
    public int index = 0;

    public bool OpenChoose = false;

    public GetSpreadSheetData getSpreadSheetData;

    public void Awake()
    {
        getSpreadSheetData = GameObject.Find("Test").GetComponent<GetSpreadSheetData>();
    }

    public void Start()
    {
        getSpreadSheetData.WhenSceneLoaded();
        getSpreadSheetData.SetText(ref index);
    }

    private void Update()
    {
        if(!OpenChoose && Input.GetKeyDown(KeyCode.Space))
        {
            getSpreadSheetData.SetText(ref index);
        }
    }

    private IEnumerator WaitSeconds(float count)
    {
        yield return new WaitForSeconds(count);
    }
}
