extends Node

export var Url = "tcp://2.tcp.ngrok.io:12608"

var ClientID : int
var client = WebSocketClient.new()
onready var spawn = get_tree().current_scene.get_node("SpawnPoint")

func _ready():
	Client.createClient()
	
func createClient():
	client.connect("connection_closed", self, "_closed")
	client.connect("connection_error", self, "_closed")
	client.connect("connection_established", self, "_connected")
	client.connect("data_received", self, "_on_data")

	var err = client.connect_to_url(Url)
	if err != OK:
		print("Unable to connect server.")
		set_process(false)

func _closed(was_clean = false):
	print("Closed, clean", was_clean)
	set_process(false)

func _connected(proto = ""):
	print("Connected with protocol: ", proto)

func _on_data():
	var pkt = client.get_peer(1).get_packet().get_string_from_utf8()
	pkt = str2var(pkt) as Array
	print("Got data from server : ", pkt)
	match pkt[0]:
		0x05 : var err = get_tree().current_scene.get_node( "SpawnPoint/"+str(pkt[1]) ).receiveMovementData(pkt[2],pkt[3]) #movement interpolation
		0x00 : createPlayerOnGame(pkt[1],true) #player master add and get their ClientID
		0x01 : createPlayerOnGame(pkt[1],false) #new player connected
		0x02 : createPlayerOnGame(pkt[1],false,Vector2( pkt[2],pkt[3] )) #when I connected, get other players data
		0x03 : spawn.get_node(str(pkt[1])).queue_free() #disconnected
		0x04 : playerPositionsInterpolation(pkt)
			
func sendData(data):
	data = var2str(data) as String
	client.get_peer(1).put_packet(data.to_utf8())

func createPlayerOnGame(id, isMaster : bool, trans : Vector2 = Vector2(0,0)):
	var player = preload("res://Assets/GameObjects/Player/Player.tscn").instance()
	player.name = str(id)
	if isMaster:
		ClientID = id
		player.add_child(preload("res://Assets/GameObjects/Player/PlayerNetworkSender.tscn").instance())
	else:
		player.transform.origin = trans
	spawn.add_child(player)
	
func playerPositionsInterpolation(data : Array):
	data.remove(0)
	for p in data:
		var player = spawn.get_node(str(p[0]))
		player.transform.origin.x = p[1]
		player.transform.origin.y = p[2]
		
func _process(delta):
	client.poll()
	
