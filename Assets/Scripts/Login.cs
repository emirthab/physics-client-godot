using Godot;
using System;

public class Login : Control {
	LineEdit nameInput;
	Button loginButton;
	PackedScene demoLevel3 = GD.Load<PackedScene>("res://Assets/Levels/DemoLevel.tscn");

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
		
		GetTree().ChangeSceneTo(demoLevel3);		
		GetNode<Client>("/root/Client").CreateClient(nameInput.Text);	
	
	}


}
