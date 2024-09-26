#this sucks
extends Control

@onready var button_container:VBoxContainer = $BG/Container
@onready var control:Control = $"../.." 

var button_height:int = 96
	
func _physics_process(_delta: float) -> void:
	var new_height = button_container.get_child_count() * button_height
	set_size(Vector2(get_size().x, new_height))
	var screen_height = control.get_size().y
	set_position(Vector2(get_position().x, (screen_height / 2) - (new_height / 2)))
