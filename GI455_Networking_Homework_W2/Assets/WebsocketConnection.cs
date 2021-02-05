using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WebSocketSharp;

namespace ProgramsChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        public InputField IPTextField, PortTextField, ChatTextField;
        
        public Text TextField, RealTextField;
        public Button SendMessageButton;
        public GameObject ChatPanel;

        WebSocket webSocket;

        void Start()
        {   
            ChatPanel.SetActive(false);
        }

        public void OnConect()
        {
            webSocket = new WebSocket("ws://"+IPTextField.text+":"+PortTextField.text+"/");
            webSocket.OnMessage += OnMessage;
            
            webSocket.Connect();
            ChatPanel.SetActive(true);                     
        }   

        public void OnChat()
        {
            webSocket.Send(ChatTextField.text);
   
            RealTextField.text = TextField.text +'\n';
        } 

        private void OnDestroy() {
            if(webSocket != null){
                webSocket.Close();
            }
        }

        public void OnMessage(object sender, MessageEventArgs e)
        {   
            if(e.IsText)
            {
                TextField.text = e.Data;
                return;
            }

            Debug.Log("ข้อความจากเซิฟเวอร์ : " + e.Data);       
        }
    }
}


