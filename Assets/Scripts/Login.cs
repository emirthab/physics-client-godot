using Godot;
using System;

public class Login : Control {
	LineEdit nameInput;
	Button loginButton;
	PackedScene demoLevel3 = GD.Load<PackedScene>("res://Assets/Levels/DemoLevel3.tscn");

	public override void _Ready()
	{
		nameInput = GetNode<LineEdit>("Panel/NameInput");
		loginButton = GetNode<Button>("Panel/Login");
		loginButton.Connect("pressed", this, "LoginGame"); 
	}


	public void LoginGame()
	{
		if ( nameInput.Text.Length < 3 )
			return;
		
		GetNode<Client>("/root/Client").CreateClient(nameInput.Text);
		GetTree().ChangeSceneTo(demoLevel3);
	}


}
