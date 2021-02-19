const app = require('express')();
const server = require('http').Server(app);
const { json } = require('express');
const websocket = require("ws");

const wss = new websocket.Server({server});

var wsList = [];
var roomList = [];
/*{
    roomName: "xxxx",
    wsList: []
}*/

wss.on("connection", (ws)=>{
    {
        //LobbyZone
        ws.on("message", (data)=>{

            console.log(data);

            var toJson = JSON.parse(data);

            console.log(toJson.eventName);
            //console.log(toJson["eventName"]);
        
            if(toJson.eventName == "CreateRoom")//CreateRoom
            {
                console.log("client request CreateRoom" + toJson.data);
                var isFoundRoom = false;
                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJson.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }

                if(isFoundRoom)
                {
                    //Callback to client : roomName is exist;
                    console.log("Create Room : room is found");

                    var resultData = {
                        eventName: toJson.eventName,
                        data: "fail"
                    }

                    var toJsonStr = JSON.stringify(resultData);
                    ws.send(toJsonStr);
                }
                else
                {
                    //Create Room here.
                    console.log("Create Room : room is not found");

                    var newRoom = {
                        roomName: toJson.data,
                        wsList: []
                    }

                    newRoom.wsList.push(ws);

                    roomList.push(newRoom);

                    var resultData = {
                        eventName: toJson.eventName,
                        data: toJson.data
                    }

                    var toJsonStr = JSON.stringify(resultData);
                    ws.send(toJsonStr);         
                }

            }
            else if(toJson.eventName == "JoinRoom")//JoinRoom
            {
                console.log("client request JoinRoom");

                var isRoomExist, isClientJoined = false;

                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJson.data)
                    {
                        isRoomExist = true;
                        break;
                    }
                }
                
                if(isRoomExist)
                {
                    for(var i = 0; i < roomList.length; i++)
                    {
                        for(var j = 0; j < roomList[i].wsList.length; j++)
                        {
                            if(ws == roomList[i].wsList[j])
                            {
                                roomList[i].wsList.push(ws);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    console.log("Can't Join Room");
                }            
            }
            else if(toJson.eventName == "LeaveRoom")
            {
                var isLeaveSuccess = false;

                for(var i = 0; i < roomList.length; i++)
                {
                    for(var j = 0; j < roomList[i].wsList.length; j++)
                    {
                        if(ws == roomList[i].wsList[j])
                        {
                            roomList[i].wsList.splice(j, 1);

                            if(roomList[i].wsList.length <= 0)
                            {
                                roomList.splice(i,1);
                            }

                            isLeaveSuccess = true;
                            break;
                        }
                    }
                }

                if(isLeaveSuccess)
                {
                    ws.send("LeaveRoomSuccess");

                    console.log("leave room success");
                }
                else
                {
                    ws.send("LeaveRoomFail");

                    console.log("leave room fail");
                }
            }          
        });    
    }

    console.log("client connected.");
    wsList.push(ws);

    ws.on("close", ()=>{
        console.log("client disconnected.");
        /*for(var i = 0; i < wsList.length; i++)
        {
            if(wsList[i] == ws)
            {
                wsList.splice(i, 1);
                break;
            }
        }*/

        for(var i = 0; i < roomList.length; i++)
        {
            for(var j = 0; j < roomList[i].wsList.length; j++)
            {
                if(roomList[i].wsList[j] == ws)
                {
                    roomList.wsList.splice(j, 1);

                    if(roomList[i].wsList.length <= 0)
                    {
                        roomList.splice(i,1);
                    }
                    
                    break;
                }
            }
        }
    });
});

server.listen(process.env.PORT || 8080, ()=>{
    console.log("Sever start at prot " + server.address().port);
});

function Boardcast(data)
{
    for(var i = 0; i < wsList.length; i++)
    {
        wsList[i].send(data);
    }
}

// var Test2 = ()=>{
//     console.log("eiei");
// }

// function Test(a)
// {
//     a();
// }

// Test(()=>{
//     console.log("eiei");
// });