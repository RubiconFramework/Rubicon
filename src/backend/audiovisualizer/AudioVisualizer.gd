extends Control

@onready var spectrum = AudioServer.get_bus_effect_instance(1, 0)
@onready var colorRectArray = $ColorRects.get_children()

const HEIGHT = 700
const FREQ_MAX = 11050.0
const MIN_DB = 60
const UPDATE_RATE = 0.01

var update_timer:float

func _process(delta):
	update_timer += delta
	if update_timer >= UPDATE_RATE:
		update_spectrum()
		update_timer = 0

func update_spectrum():
	var tween = create_tween().set_parallel(true) 
	var prev_hz = 0

	for i in range(colorRectArray.size()):
		var hz = (i + 1) * FREQ_MAX / colorRectArray.size()
		prev_hz = hz
		tween.tween_property(colorRectArray[i], "size", Vector2(colorRectArray[i].size.x, clamp((MIN_DB + linear_to_db(spectrum.get_magnitude_for_frequency_range(prev_hz, hz).length())) / MIN_DB, 0, 1) * HEIGHT), 0.05)
