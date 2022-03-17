extends Node

export var Url = "ws://45.141.149.120:3636"
#export var Url = "ws://127.0.0.1:3636"

var ClientID : int
var client = WebSocketClient.new()
var spawn
var displayName = ""
	
func createClient(dn):
	displayName = dn
	
	print("Connecting to game server...")
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
	spawn = get_tree().current_scene.get_node("SpawnPoint")
	Client.sendData([0x07, displayName])

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
		0x04 : spawn.get_node(str(pkt[1])).playerPositionsInterpolation = Vector2(pkt[2],pkt[3])
		0x06 : sendData([0x06]) #pong
		0x07 : spawn.get_node(str(pkt[1])).setDisplayLabel(pkt[2])
		0x08 : fillNameLabels(pkt[1])
		
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


func fillNameLabels(players):
	print(players)
	for player in players:
		spawn.get_node(str(player["id"])).setDisplayLabel(player["display_name"])


func _process(delta):
	client.poll()
	
