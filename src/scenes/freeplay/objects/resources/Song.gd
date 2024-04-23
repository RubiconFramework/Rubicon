class_name Song
extends Resource

@export_group("Song Data")
@export var song_display_name: String
@export var song_name: String
@export var song_icon: SpriteFrames
@export var song_icon_frame: int = 2
@export var difficulties:Array[String] = ["Hard", "Normal", "Easy"]

@export_group("Freeplay Data")
@export var background_color: Color
