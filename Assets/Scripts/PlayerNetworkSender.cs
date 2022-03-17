using Godot;
using System;

public class PlayerNetworkSender : Node 
{
	Client client;

	public override void _Ready()
	{
		client = GetNode<Client>("/root/Client");        
	}

	public override void _Process(float delta)
	{
		GetInput();
	}

	private void GetInput()
	{

	if (Input.IsActionJustPressed("right") ) client.SendData(0x05, new object[] {0,1});
	if (Input.IsActionJustPressed("left")  ) client.SendData(0x05, new object[] {1,1});
	if (Input.IsActionJustPressed("down")  ) client.SendData(0x05, new object[] {2,1});
	if (Input.IsActionJustPressed("up")    ) client.SendData(0x05, new object[] {3,1});

	if (Input.IsActionJustReleased("right")) client.SendData(0x05, new object[] {0,0});
	if (Input.IsActionJustReleased("left") ) client.SendData(0x05, new object[] {1,0});
	if (Input.IsActionJustReleased("down") ) client.SendData(0x05, new object[] {2,0});
	if (Input.IsActionJustReleased("up")   ) client.SendData(0x05, new object[] {3,0});
	}
}
