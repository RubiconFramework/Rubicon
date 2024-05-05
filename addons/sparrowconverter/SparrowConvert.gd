@tool
extends EditorPlugin

var dockScene
var spritePath
var atlasPath
var fps
var loop
var optimized
var convertBttn

func _enter_tree():
	dockScene = preload("res://addons/sparrowconverter/SparrowConvert.tscn").instantiate()
	
	spritePath = dockScene.get_node("Panel/SpritePath")
	atlasPath = dockScene.get_node("Panel/AtlasPath")
	fps = dockScene.get_node("Panel/VBoxContainer/FPS")
	loop = dockScene.get_node("Panel/VBoxContainer/Loop")
	optimized = dockScene.get_node("Panel/VBoxContainer/Optimize")
	convertBttn = dockScene.get_node("Panel/VBoxContainer/Convert")
	convertBttn.connect("pressed", convert_sprite)
	
	add_control_to_dock(DOCK_SLOT_LEFT_UR, dockScene)

func _exit_tree():
	remove_control_from_docks(dockScene)
	dockScene.queue_free()

func convert_sprite():
	var finalSpritePath = spritePath.text
	var finalAtlasPath = atlasPath.text
	
	if finalAtlasPath == "":
		finalAtlasPath = finalSpritePath.get_base_name() + ".xml"
	
	var texture = load(finalSpritePath)
	if texture == null:
		printerr("Sprite failed loading at path: ", finalSpritePath)
		return
	
	print("Sprite Path: ", finalSpritePath, "\nAtlas Path: ", finalAtlasPath)
	
	var spriteFrame = SpriteFrames.new()
	spriteFrame.remove_animation("default")
	
	var xml = XMLParser.new()
	xml.open(finalAtlasPath)
	
	var previousRect = Rect2()
	var previousAtlas = AtlasTexture.new()
	
	while xml.read() == OK:
		if xml.get_node_type() != XMLParser.NODE_TEXT:
			var nodeName = xml.get_node_name()
			
			if nodeName == "SubTexture":
				var frameData
				
				var animName = xml.get_named_attribute_value("name")
				animName = animName.left(animName.length() - 4)
				
				var frameRect = Rect2(
					Vector2(xml.get_named_attribute_value("x").to_float(), xml.get_named_attribute_value("y").to_float()),
					Vector2(xml.get_named_attribute_value("width").to_float(), xml.get_named_attribute_value("height").to_float())
				)
				
				if optimized.pressed and previousRect == frameRect:
					frameData = previousAtlas
				else:
					frameData = AtlasTexture.new()
					frameData.atlas = texture
					frameData.region = frameRect
					
					if xml.has_attribute("frameX"):
						print("frame data exists")
						
						var frameSizeData = Vector2(xml.get_named_attribute_value("frameWidth").to_int(), xml.get_named_attribute_value("frameHeight").to_int())
						if frameSizeData == Vector2.ZERO:
							frameSizeData = frameRect.size
						
						var margin = Rect2(
							Vector2(-xml.get_named_attribute_value("frameX").to_int(), -xml.get_named_attribute_value("frameY").to_int()),
							Vector2(xml.get_named_attribute_value("frameWidth").to_int() - frameRect.size.x, xml.get_named_attribute_value("frameHeight").to_int() - frameRect.size.y))
						
						if margin.size.x < abs(margin.position.x):
							margin.size.x = abs(margin.position.x)
						if margin.size.y < abs(margin.position.y):
							margin.size.y = abs(margin.position.y)
						
						frameData.margin = margin
					
					frameData.filter_clip = true
					
					previousAtlas = frameData
					previousRect = frameRect
				
				if not spriteFrame.has_animation(animName):
					spriteFrame.add_animation(animName)
					spriteFrame.set_animation_loop(animName, loop.pressed)
					spriteFrame.set_animation_speed(animName, fps.text.to_int())
				
				spriteFrame.add_frame(animName, frameData)
	
	ResourceSaver.save(spriteFrame, finalSpritePath.get_base_name() + ".res", ResourceSaver.FLAG_COMPRESS)
	if ResourceLoader.exists(finalSpritePath.get_base_name() + ".res"):
		print("SpriteFrame succesfully created at path: ", finalSpritePath.get_base_name() + ".res")
