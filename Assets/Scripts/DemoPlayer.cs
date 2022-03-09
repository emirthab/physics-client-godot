using Godot;

//! This file is incomplete


public class DemoPlayer : KinematicBody2D
{
    [Export] int speed = 200;
    Vector2 velocity = Vector2.Zero;
    Vector2 lastVelocity = Vector2.Zero;
    Vector2 playerPositionsInterpolation = new Vector2(0,0);
    Vector2 oldInterpolation = new Vector2(0,0);

    Spatial playerModel;
    AnimationNodeStateMachinePlayback stateMachine;

    public override void _Ready()
    {
        // onready var Player = $ViewportContainer/Viewport/CharacterModel
        // onready var StateMachine = $AnimationTree.get("parameters/playback")
        playerModel = GetNode<Spatial>("/ViewportContainer/Viewport/CharacterModel");
        stateMachine = GetNode<AnimationNodeStateMachinePlayback>("/AnimationTree");

    
    }

}