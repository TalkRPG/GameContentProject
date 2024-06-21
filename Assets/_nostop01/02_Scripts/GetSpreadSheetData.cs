using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetSpreadSheetData : MonoBehaviour
{
    public readonly string Address = "https://docs.google.com/spreadsheets/d/14Iee2wQQEMyhReFx9pXmo22C6_MzNjm8fhjobiroycI";
    public readonly string Range = "A2:E";
    public readonly long Sheet_ID = 0;

    public readonly string Event_Range = "A2:D";
    public readonly long Event_Sheet_ID = 1989135604;

    public int buttonCount;

    public List<NormalSheetList> normalSheet;
    public List<EventSheetList> eventSheets;

    public TMP_Text content_text;
    public TMP_Text[] button_text;

    public Button[] buttons;

    public ThrowText throwtext;
 
    private void Awake()
    {
        LoadSheet();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public static string GetTSVAddress(string address, string range, long sheet_id) //시트 주소 받아오기
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheet_id}";
    }

    private void LoadSheet() //시트 불러오기
    {
        StartCoroutine(LoadIDSheet());
        StartCoroutine(LoadEventSheet());
    }

    private IEnumerator LoadIDSheet() //대화문 시트 받아오기
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(Address, Range, Sheet_ID));
        yield return www.SendWebRequest();

        normalSheet = GetDatas<NormalSheetList>(www.downloadHandler.text);
    }

    private IEnumerator LoadEventSheet() //이벤트 시트 받아오기(선택지)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(Address, Event_Range, Event_Sheet_ID));
        yield return www.SendWebRequest();

        eventSheets = GetDatas<EventSheetList>(www.downloadHandler.text);
    }

    T GetData<T>(string[] datas) //데이터 받아와서 파싱(형변환)
    {
        object data = Activator.CreateInstance(typeof(T));

        // 클래스에 있는 변수들을 순서대로 저장한 배열
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < datas.Length; i++)
        {
            try
            {
                // string > parse
                Type type = fields[i].FieldType;

                if (string.IsNullOrEmpty(datas[i])) continue;

                // 변수에 맞는 자료형으로 파싱해서 넣는다
                if (type == typeof(int))
                    fields[i].SetValue(data, int.Parse(datas[i]));

                else if (type == typeof(string))
                    fields[i].SetValue(data, datas[i]);

                else if(type == typeof(float))
                    fields[i].SetValue(data, float.Parse(datas[i]));
            }

            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"SpreadSheet Error : {e.Message}");
            }
        }

        return (T)data;
    }

    List<T> GetDatas<T>(string data) //받아온 데이터 저장
    {
        List<T> returnList = new List<T>();
        string[] spliteData = data.Split('\n');

        foreach(string elements in spliteData)
        {
            string[] datas = elements.Split("\t");
            returnList.Add(GetData<T>(datas));
        }

        return returnList;
    }

    public void SetText(ref int index)
    {
        if (buttonCount != 0)
        {
            index = eventSheets[buttonCount].Move_ID;

            buttonCount = 0;
        }

        content_text.text = normalSheet[index].Talk.Replace("\\n", "\n");

        if (normalSheet[index].Skip_ID != 0)
        {
            index = normalSheet[index].Skip_ID;
        }

        int i = 0;

        if (normalSheet[index].Event_ID == eventSheets[i].ID)
        {
            SetChooseEvent();
        }

        if (normalSheet.Count > index)
        {
            index++;
        }
    }

    public void SetChooseEvent()
    {
        throwtext.OpenChoose = true;

        for (int i = 0; i < eventSheets.Count; i++)
        {
            button_text[i].text = eventSheets[i].Choose;
        }

        for(int i = 0; i < 4; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(index));

            if (button_text[i].text == null)
            {
                buttons[i].interactable = false;
            }

            else
            {
                buttons[i].interactable = true;
            }
        }
    }

    public void OnButtonClick(int buttonindex)
    {
        buttonCount = buttonindex;

        throwtext.OpenChoose = false;

        for(int i = 0; i < 4; i++)
        {
            button_text[i].text = null;

            if (button_text[i].text == null)
            {
                buttons[i].interactable = false;
            }
        }
    }

    public void WhenSceneLoaded()
    {
        content_text = GameObject.Find("ContentText").GetComponent<TMP_Text>();
        throwtext = GameObject.Find("ThrowText").GetComponent<ThrowText>();

        for (int i = 0; i < 4; i++)
        {
            button_text[i] = GameObject.Find("Button Text " + i).GetComponent<TMP_Text>();
            buttons[i] = GameObject.Find("Button " + i).GetComponent<Button>();
            button_text[i].text = null;
            buttons[i].interactable = false;
        }
    }
}
