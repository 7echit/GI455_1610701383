using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace ProgramsChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        struct MessageData
        {
            public string UserName;
            public string UserMessage;

            public MessageData(string username, string message)
            {
                this.UserName = username;
                this.UserMessage = message;
            }
        }

        public struct SocketEvent
        {
            public string eventName;
            public string data; 

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }

        public GameObject rootConnection, rootCreateAndJoin, rootMessenger;
        public InputField inputUsername, inputText, RoomNameText, JoinRoomNameText;
        public Text sendText, recieveText;       

        public string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;

        WebSocket webSocket;
        SocketEvent socketEvent;

        void Start()
        {   
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(false);
        }

        private void Update() 
        {
            UpdateNotifyMessage();
        }

        public void Connect()
        {
            string url = "ws://127.0.0.1:8080/";

            webSocket = new WebSocket(url);

            webSocket.OnMessage += OnMessage;
            
            webSocket.Connect();

            //CreateRoom("TestRoom01"); //RoomNameText

            rootConnection.SetActive(false);
            rootCreateAndJoin.SetActive(true);                     
        }  

        public void CreateRoom()
        {
            if(webSocket.ReadyState == WebSocketState.Open)
            {    
                SocketEvent socketEvent = new SocketEvent("CreateRoom", RoomNameText.text);   
               
                string jsonStr = JsonUtility.ToJson(socketEvent);

                webSocket.Send(jsonStr);
                rootMessenger.SetActive(true);
                rootCreateAndJoin.SetActive(false);     
            }          
        } 

        public void JoinRoom()
        {
            if(webSocket.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("JoinRoom", JoinRoomNameText.text);

                string jsonStr = JsonUtility.ToJson(socketEvent);

                webSocket.Send(jsonStr);

                rootMessenger.SetActive(true);
                rootCreateAndJoin.SetActive(false);
            }
        }

        public void LeaveRoom()
        {
            if(webSocket.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

                string jsonStr = JsonUtility.ToJson(socketEvent);

                webSocket.Send(jsonStr);

                rootMessenger.SetActive(false);
                rootCreateAndJoin.SetActive(true);
            }
        }

        /*IEnumerator CreateAndLeave(string roomName){

                SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

                string jsonStr = JsonUtility.ToJson(socketEvent);

                webSocket.Send(jsonStr);

                yield return new WaitForSeconds(3.0f);

                socketEvent.eventName = "LeaveRoom";
                socketEvent.data = "";

                jsonStr = JsonUtility.ToJson(socketEvent);

                webSocket.Send(jsonStr);
        }*/

        public void Disconnect()
        {
            if(webSocket != null)
            {
                webSocket.Close();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        } 

        public void SendMessage() 
        {
            if(string.IsNullOrEmpty(inputText.text) || webSocket.ReadyState != WebSocketState.Open)
                return;

            MessageData messageData = new MessageData(inputUsername.text, inputText.text);

            string toJsonStr = JsonUtility.ToJson(messageData);

            webSocket.Send(toJsonStr);
            inputText.text = "";    
        }

        private void UpdateNotifyMessage()
        {
            if(string.IsNullOrWhiteSpace(tempMessageString) == false){

                SocketEvent recieveData = JsonUtility.FromJson<SocketEvent>(tempMessageString);
                Debug.Log(recieveData);
                if(recieveData.eventName == "CreateRoom"){
                    if(OnCreateRoom != null)
                    {
                        OnCreateRoom(recieveData);
                    }
                }
                else if(recieveData.eventName == "JoinRoom"){
                    if(OnJoinRoom != null){
                        OnJoinRoom(recieveData);
                    }
                }
                else if(recieveData.eventName == "LeaveRoom"){
                    if(OnLeaveRoom != null){
                        OnLeaveRoom(recieveData);
                    }
                }   

                tempMessageString = "";

                /*MessageData recieveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);

                if(recieveMessageData.UserName == inputUsername.text)
                {
                    sendText.text += "<color=red>" + recieveMessageData.UserName + "</color> : " + recieveMessageData.UserMessage + "\n";
                    recieveText.text += "\n";
                }
                else
                {
                    sendText.text += "\n";
                    recieveText.text += recieveMessageData.UserName + " : " + recieveMessageData.UserMessage + "\n";
                }*/

                
            }
        }

        public void OnMessage(object sender, MessageEventArgs e)
        {  
            tempMessageString = e.Data;
            Debug.Log("ข้อความ : " + e.Data);       
        }
    }
}


