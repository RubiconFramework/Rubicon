extends Node

static var settings_data: Dictionary = {}

func _ready() -> void:
	Settings.SettingsLoaded.connect(settings_loaded)
	
func settings_loaded(settings_path: String):
	var config = ConfigFile.new()
	var err = config.load(settings_path)
	if err != OK:
		print("Failed to load settings file!")
		return

	settings_data.clear()

	for section in config.get_sections():
		settings_data[section] = {}
		for key in config.get_section_keys(section):
			var value = config.get_value(section, key)
			settings_data[section][key] = value
	
	#if settings_data["Misc/Debug"]["PrintSettingsOnConsole"] == true:
	#	print("[SettingsAccessor] Settings data loaded:")
	#	print(settings_data)

func get_setting(section: String, key: String, default_value = null):
	if section in settings_data and key in settings_data[section]:
		return settings_data[section][key]
	return default_value
