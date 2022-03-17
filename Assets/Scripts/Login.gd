extends Control

var demo_level_4 = preload("res://Assets/Levels/DemoLevel4.tscn")

onready var name_input = $Panel/NameInput
onready var login_button = $Panel/Login


func _on_Login_pressed():
	if name_input.text.length() < 3:
		return

	get_tree().change_scene_to(demo_level_4)
	Client.createClient(name_input.text)

