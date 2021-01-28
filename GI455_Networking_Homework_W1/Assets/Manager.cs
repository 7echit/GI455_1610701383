using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    List<string> NameLists = new List<string>() {"ส้มตำ", "ไก่ย่าง", "ข้าวเหนียว", "ขนมจีน", "ไข่ปิ้ง", "ลาบเป็ด", "ปลาดุกปิ้ง", "ต้มยำ", "ปลาทอด", "ตับไก่", "หอยดอง"};
    public List<Text> nameText = new List<Text>();
 
    public InputField inputField;
    public GameObject textDisplay;

    void Start()
    {
        inputField.onEndEdit.AddListener(SubmitName);

        for(int i = 0; i <= NameLists.Count - 1; i++)
        {
            nameText[i].text = NameLists[i].ToString();
        }
    }

    public void SubmitName(string NameText)
    {
        if (NameLists.Contains(inputField.text))
        {
            textDisplay.GetComponent<Text>().text = "[ " + "<color=green>" + NameText + "</color>" + " ]" + " is found.";
        }
        else
        {
            textDisplay.GetComponent<Text>().text = "[ " + "<color=red>" + NameText + "</color>" + " ]" + " is not found.";
        }        
    }
}
