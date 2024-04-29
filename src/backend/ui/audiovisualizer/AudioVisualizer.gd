extends Control

@onready var spectrum = AudioServer.get_bus_effect_instance(1, 0)
@onready var colorRectArray = $ColorRects.get_children()

const VU_COUNT = 16
const HEIGHT = 700
const FREQ_MAX = 11050.0
const MIN_DB = 60
const UPDATE_RATE = 0.01

var update_timer: float

func _process(delta):
    update_timer += delta
    if update_timer >= UPDATE_RATE:
        update_spectrum()
        update_timer = 0

func update_spectrum():
    var tween = create_tween().set_parallel(true) 

    var prev_hz = 0

    for colorRect in colorRectArray:
        var i = colorRectArray.find(colorRect) + 1
        var hz = i * FREQ_MAX / VU_COUNT
        var f = spectrum.get_magnitude_for_frequency_range(prev_hz, hz)
        var energy = clamp((MIN_DB + linear_to_db(f.length())) / MIN_DB, 0, 1)
        var height = energy * HEIGHT
        prev_hz = hz

        tween.tween_property(colorRect, "size", Vector2(colorRect.size.x, height), 0.05)
