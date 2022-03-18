using Godot;
using System;


//! Errors with 0x05 are not solved


public class Client : Node
{
	public WebSocketClient client = new WebSocketClient();
	public int ClientID;
	public string url = "ws://45.141.149.120:3636";
	public Node2D spawn;
	public string displayName = "";
	PackedScene Player;
	PackedScene PlayerNetworkSender;

	public override void _Ready()
	{
		SetProcess(false);
		Player = GD.Load<PackedScene>("res://Assets/GameObjects/Player/Player.tscn");
		PlayerNetworkSender = GD.Load<PackedScene>("res://Assets/GameObjects/Player/PlayerNetworkSender.tscn");
	}

	public void Closed(bool wasClean = false)
	{
		GD.Print("Closed, clean: ", wasClean);
		// TODO : Show error message to player
	}

	public void OnConnected(string protocol = "")
	{
		GD.Print("Connected");
		// TODO : Show a nice message to player

		spawn = GetTree().CurrentScene.GetNode<Node2D>("SpawnPoint");
		SendData(0x07, new object[] {displayName});
	}
	

	public void CreateClient(string uname)
	{
		displayName = uname;

		client.Connect("connection_closed", this, "Closed");
		client.Connect("connection_error", this, "Closed");
		client.Connect("connection_established", this, "OnConnected");
		client.Connect("data_received", this, "OnData");

		Error err = client.ConnectToUrl(url);

		if (err != Error.Ok)
		{
			GD.Print("Cannot connect to server");
			SetProcess(false);
			return;
		}

		SetProcess(true);
	}


	public void OnData()
	{
		byte[] incomingData = client.GetPeer(1).GetPacket();
		Godot.Collections.Array pkt = Utils.ByteArray2Data(incomingData);


		switch (pkt[0])
		{
			// Player is master, add them to the game and get their ClientID
			case 0x00:
				CreatePlayerOnGame((int)pkt[1], true, new Vector2(0, 0));
				break;

			// Regular player connected, add them to game
			case 0x01:
				CreatePlayerOnGame((int)pkt[1], false, Vector2.Zero);
				break;

			// When new player is connected, get the positions of players in room
			case 0x02:
				CreatePlayerOnGame((int)pkt[1], false, new Vector2((float)pkt[2], (float)pkt[3]));
				break;

			// Player disconnected
			case 0x03:
				spawn.GetNode(pkt[1].ToString()).QueueFree();
				break;

			// 
			case 0x04:
				Vector2 newInterpolation = new Vector2((float) pkt[2], (float) pkt[3]);
				spawn.GetNode<Player>(pkt[1].ToString()).newPosition = newInterpolation;
				break;

			// Movement interpolation
			case 0x05:
				GD.Print("New 0x05");
				GD.Print(pkt[2], pkt[3]);
				spawn.GetNode<Player>(pkt[1].ToString()).receiveMovementData((int) pkt[2], Convert.ToBoolean(pkt[3]) );
				break;

			// Pong
			case 0x06:
				GD.Print("pong");
				SendData(0x06, new object[] {});
				break;

			// Set head-top name label
			case 0x07:
				spawn.GetNode<Player>(pkt[1].ToString()).SetDisplayLabel(displayName);
				break;

			// Get other players name and add them on their head-top label
			case 0x08:
				GD.Print(pkt[1].GetType());
				FillPlayerNames(pkt[1] as Godot.Collections.Array);
				break;

			default:
				GD.Print("Unknown header");
				break;
		}
	}

	public void CreatePlayerOnGame(int id, bool isMaster, Vector2 transformOrigin)
	{
		KinematicBody2D player = Player.Instance<KinematicBody2D>();
		Node playerNetworkSender = PlayerNetworkSender.Instance<Node>();

		player.Name = id.ToString();

		spawn.AddChild(player);

		if (isMaster)
		{
			player.AddChild(playerNetworkSender);
			ClientID = id;
			Camera2D camera = new Camera2D();
			camera.Current = true;
			//2450 1425
			camera.LimitLeft = 0;
			camera.LimitRight = 2450;
			camera.LimitTop = 0;
			camera.LimitBottom = 1425;
			player.AddChild(camera);
		}
		else
		{
			Transform2D trans = player.Transform;
			trans.origin = transformOrigin;
			player.Transform = trans;
		}
	}

	public void SendData(int header, object[] data)
	{
		Godot.Collections.Array packet = new Godot.Collections.Array { header };
		
		foreach (var item in data)
		{
			packet.Add(item);
		}

		client.GetPeer(1).PutPacket(Utils.Data2ByteArray(packet));
	}

	public void FillPlayerNames(Godot.Collections.Array Names)
	{
		foreach (Godot.Collections.Dictionary pair in Names)
		{
			spawn.GetNode<Player>(pair["id"].ToString()).SetDisplayLabel(pair["display_name"].ToString());
		}
	}

	public override void _Process(float delta)
	{
		if (client is WebSocketClient)
			client.Poll();
	}


}
