using Godot;
using System;


//! This file is haven't tested yet


public class Client : Node
{
    private WebSocketClient client = new WebSocketClient();
    private int ClientID;
    private string url = "ws://127.0.0.1:3636";
    private Node2D spawn;
    private string displayName = "";
    PackedScene Player;
    PackedScene PlayerNetworkSender;


    public override void _Ready()
    {
        Player = GD.Load<PackedScene>("res://Assets/GameObjects/Player/Player.tscn");
        PlayerNetworkSender = GD.Load<PackedScene>("res://Assets/GameObjects/Player/PlayerNetworkSender.tscn");
    }


    public override void _Process(float delta)
    {
        base._Process(delta);

        client.Poll();
    }

    public void CreateClient(string uname)
    {
        displayName = uname;

        GD.Print("Welcome to saniyelika, ", displayName);

        client.Connect("connection_closed", this, "Closed");
        client.Connect("connection_error", this, "Closed");
        client.Connect("connection_established", this, "Connected");
        client.Connect("data_received", this, "OnData");

        Error err = client.ConnectToUrl(url);

        if (err != Error.Ok)
        {
            GD.Print("Cannot connect to server");
            SetProcess(false);
        }

    }

    public void Closed(bool wasClean = false)
    {
        GD.Print("Closed, clean: ", wasClean);
        // TODO : Show error message to player
    }

    public void Connected()
    {
        GD.Print("Connected");
        // TODO : Show a nice message to player

        spawn = GetTree().CurrentScene.GetNode<Node2D>("SpawnPoint");
        //* SendData([0x07, displayName])
    }

    private void OnData()
    {

    }

    public void SendData(Array data)
    {

    }

    private void CreatePlayerOnGame(int id, bool isMaster, Vector2 trans)
    {
        KinematicBody2D player = Player.Instance<KinematicBody2D>();
        Node playerNetworkSender = PlayerNetworkSender.Instance<Node>();

        player.Name = id.ToString();

        spawn.AddChild(player);

        if (isMaster)
        {
            player.AddChild(playerNetworkSender);
            ClientID = id;
        }
        else
        {
            // player.Transform.x = trans.x;
            // player.Transform.y = trans.y;
        }
    }



}
