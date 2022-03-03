extends KinematicBody2D

var speed = 200
var velocity = Vector2.ZERO
var LastVelocity = Vector2.ZERO

onready var Player = $ViewportContainer/Viewport/CharacterModel
onready var StateMachine = $AnimationTree.get("parameters/playback")

var is_pressing_right = false
var is_pressing_left = false
var is_pressing_down = false
var is_pressing_up = false

func _ready():
	#$ViewportContainer/Viewport.set_use_own_world(true)
	#$ViewportContainer/Viewport.world = World.new()
	pass
func receiveMovementData(key,value):
	match key:
		0 : is_pressing_right = false if value else true
		1 : is_pressing_left = false if value else true
		2 : is_pressing_down = false if value else true
		3 : is_pressing_up = false if value else true

func _physics_process(delta):
	velocity = Vector2.ZERO
	if has_node("PlayerNetworkSender"):
		if Input.is_action_just_pressed('right') : receiveMovementData(0,0)
		if Input.is_action_just_pressed('left'): receiveMovementData(1,0)
		if Input.is_action_just_pressed('down'): receiveMovementData(2,0)
		if Input.is_action_just_pressed('up'): receiveMovementData(3,0)
		
		if Input.is_action_just_released('right') : receiveMovementData(0,1)
		if Input.is_action_just_released('left'): receiveMovementData(1,1)
		if Input.is_action_just_released('down'): receiveMovementData(2,1)
		if Input.is_action_just_released('up'): receiveMovementData(3,1)
		
	if is_pressing_right: velocity.x += 1
	if is_pressing_left: velocity.x -= 1
	if is_pressing_down: velocity.y += 1
	if is_pressing_up: velocity.y -= 1

	velocity = velocity.normalized() * speed
	velocity = move_and_slide(velocity)
	
	if velocity != Vector2(0,0):
		StateMachine.travel("Running")
		LastVelocity = velocity
	else:
		StateMachine.travel("Idle")
		
	Player.rotation.y = lerp_angle(Player.rotation.y, atan2(-LastVelocity.x,-LastVelocity.y), delta * 10)
	
