using Godot;

public class PlayerNetworkSender : Node 
{
	Client Client;

	public override void _Ready()
	{
		Client = GetNode<Client>("/root/Client");        
	}

	private void GetInput()
	{

	if (Input.IsActionJustPressed("right") ) Client.SendData(new int[] {0x05,0,0});
	if (Input.IsActionJustPressed("left")  ) Client.SendData(new int[] {0x05,1,0});
	if (Input.IsActionJustPressed("down")  ) Client.SendData(new int[] {0x05,2,0});
	if (Input.IsActionJustPressed("up")    ) Client.SendData(new int[] {0x05,3,0});

	if (Input.IsActionJustReleased("right")) Client.SendData(new int[] {0x05,0,1});
	if (Input.IsActionJustReleased("left") ) Client.SendData(new int[] {0x05,1,1});
	if (Input.IsActionJustReleased("down") ) Client.SendData(new int[] {0x05,2,1});
	if (Input.IsActionJustReleased("up")   ) Client.SendData(new int[] {0x05,3,1});
	}
}
