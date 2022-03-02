extends Node
	
func get_input():
	if Input.is_action_just_pressed('right') : Client.sendData([0x05,0,0])
	if Input.is_action_just_pressed('left'): Client.sendData([0x05,1,0])
	if Input.is_action_just_pressed('down'): Client.sendData([0x05,2,0])
	if Input.is_action_just_pressed('up'): Client.sendData([0x05,3,0])
	
	if Input.is_action_just_released('right') : Client.sendData([0x05,0,1])
	if Input.is_action_just_released('left'): Client.sendData([0x05,1,1])
	if Input.is_action_just_released('down'): Client.sendData([0x05,2,1])
	if Input.is_action_just_released('up'): Client.sendData([0x05,3,1])

func _process(delta):
	get_input()
