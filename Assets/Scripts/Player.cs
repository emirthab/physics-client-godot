using Godot;

public class Player : KinematicBody2D
{
	[Export] float speed = 200;
	Vector2 velocity = Vector2.Zero;
	Vector2 previousVelocity = Vector2.Zero;

	public Vector2 newPosition { get; set; } = new Vector2(0, 0);

	Vector2 oldPosition = new Vector2(0, 0);

	Spatial playerModel;
	AnimationNodeStateMachinePlayback stateMachine;

	bool isPressingRight = false;
	bool isPressingLeft = false;
	bool isPressingDown = false;
	bool isPressingUp = false;
	public override void _Ready()
	{
		// onready var Player = $ViewportContainer/Viewport/CharacterModel
		// onready var StateMachine = $AnimationTree.get("parameters/playback")
		playerModel = GetNode<Spatial>("ViewportContainer/Viewport/CharacterModel");
		stateMachine = GetNode<AnimationTree>("AnimationTree").Get("parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!HasNode("PlayerNetworkSender"))
			return;
		

		PrepareVelocity();

		velocity = velocity.Normalized() * speed;
		velocity = MoveAndSlide(velocity);

		if (velocity != Vector2.Zero)
		{
			// stateMachine.Travel("Runninig");
			previousVelocity = velocity;
		}
		else
		{
			// stateMachine.Travel("Idle");
		}

		InterpolatePlayer(delta);
	}

	private void PrepareVelocity()
	{
		velocity = new Vector2(0, 0);

		if (Input.IsActionJustPressed("right")) receiveMovementData(0, true);
		if (Input.IsActionJustPressed("left")) receiveMovementData(1, true);
		if (Input.IsActionJustPressed("down")) receiveMovementData(2, true);
		if (Input.IsActionJustPressed("up")) receiveMovementData(3, true);

		if (Input.IsActionJustReleased("right")) receiveMovementData(0, false);
		if (Input.IsActionJustReleased("left")) receiveMovementData(1, false);
		if (Input.IsActionJustReleased("down")) receiveMovementData(2, false);
		if (Input.IsActionJustReleased("up")) receiveMovementData(3, false);

		if (isPressingRight) velocity.x += 1.0f;
		if (isPressingLeft) velocity.x -= 1.0f;
		if (isPressingDown) velocity.y += 1.0f;
		if (isPressingUp) velocity.y -= 1.0f;
	}

	private void InterpolatePlayer(float delta)
	{
		float newAngle = Mathf.Atan2(-previousVelocity.x, -previousVelocity.y);
		Vector3 rotation = playerModel.Rotation;
		rotation.y = Mathf.LerpAngle(rotation.y, newAngle, delta * 10);
		playerModel.Rotation = rotation;


		if (oldPosition != newPosition)
		{
			Transform2D trans = Transform;
			trans.origin = trans.origin.LinearInterpolate(newPosition, delta * 10);
			Transform = trans;
			oldPosition = newPosition;
		}

	}

	public void SetDisplayLabel(string displayName)
	{
		this.GetNode<Label>("Label").Text = displayName;
	}

	public void receiveMovementData(int key, bool value)
	{
		switch (key)
		{
			case 0:
				isPressingRight = value;
				break;
			case 1:
				isPressingLeft = value;
				break;
			case 2:
				isPressingDown = value;
				break;
			case 3:
				isPressingUp = value;
				break;
		}
	}


}
