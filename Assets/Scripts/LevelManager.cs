using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Broadcast;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject textPanel;
    public Transform chatPanel;
    public InputField chatBox;

    void Start()
    {
        // Cualquier mensaje que sea del tipo "MessageBroadcast", procesarlo en la funcion 'ServerMessageRecieved'
        // Para que el servidor procese cualquier tipo de mensaje desde un cliente
        InstanceFinder.ServerManager.RegisterBroadcast<MessageText>(ServerMessageReceived);

        //Si recibe un mensaje como cliente procesar en la funcion de "ClientMessageRecieved"
        InstanceFinder.ClientManager.RegisterBroadcast<MessageText>(ClientMessageRecieved);
    }

    void OnDestroy()
    {
        // Buena practica es desuscribir al destruir el gameObject
        if (InstanceFinder.ServerManager)
            InstanceFinder.ServerManager.UnregisterBroadcast<MessageText>(ServerMessageReceived);

        if (InstanceFinder.ClientManager)
            InstanceFinder.ClientManager.UnregisterBroadcast<MessageText>(ClientMessageRecieved);
    }

    void ServerMessageReceived(NetworkConnection conn, MessageText message, Channel channel)
    {
        //print($"This is the server and we've recieved the following message from {conn.ClientId}: {message.color} - {message.num}");
        print($"Player {conn.ClientId} has sent the following message: {message.message}");
        message.playerId = conn.ClientId;

        if(InstanceFinder.IsServerOnlyStarted)
        {
            GameObject sentMessage = Instantiate(textPanel, chatPanel);
            sentMessage.GetComponent<TextMeshProUGUI>().text = $"Player {message.playerId}: {message.message}";
        }

        InstanceFinder.ServerManager.Broadcast(message); //para enviar mensaje a todos los jugadores
    }

    void ClientMessageRecieved(MessageText message, Channel channel)
    {
        //print($"This is the client and the message from {message.playerId} is: {message.color} - {message.num}");
        GameObject sentMessage = Instantiate(textPanel, chatPanel);
        sentMessage.GetComponent<TextMeshProUGUI>().text = $"Player {message.playerId}: {message.message}";
        print($"Player {message.playerId}: {message.message}");
    }

    private void SendMessage()
    {
        MessageText newMessage = new MessageText()
        {
            message = chatBox.text
        };

        chatBox.text = "";

        InstanceBroadcast(newMessage);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }   
    }
    
    void InstanceBroadcast(MessageText message)
    {
        // if (InstanceFinder.IsServerStarted) InstanceFinder.ServerManager.Broadcast(message);
        if (InstanceFinder.IsClientStarted) InstanceFinder.ClientManager.Broadcast(message);
    }

    public struct MessageText : IBroadcast
    {
        public int playerId;
        public string message;
    }

    public struct MessageBroadcast : IBroadcast
    {
        public int playerId;
        public Color color;
        public int num;
    }
}
