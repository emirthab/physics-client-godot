extends AnimationTree

onready var StateMachine = get("parameters/playback")

func _ready():
	active = true
func _physics_process(delta):
	if get_parent().velocity != Vector2(0,0):
		StateMachine.travel("Running")
		print("running")
	else:
		StateMachine.travel("Idle")
