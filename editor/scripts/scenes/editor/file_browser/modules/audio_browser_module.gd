extends PanelContainer

const PLAY_ICON: Texture2D = preload("res://assets/icons/play/play.svg")
const PAUSE_ICON: Texture2D = preload("res://assets/icons/paused/paused.svg")

@onready var play_pause_button: TextureButton = %PlayPauseButton
@onready var progress_slider: HSlider = %ProgressSlider
@onready var time_label: Label = %TimeLabel
@onready var audio_player: AudioStreamPlayer = %AudioPlayer

var _duration: float = 0.0
var _is_slider_dragging: bool = false
var _is_slider_updating: bool = false


func _ready() -> void:
	if not play_pause_button.pressed.is_connected(_on_play_pause_button_pressed):
		play_pause_button.pressed.connect(_on_play_pause_button_pressed)
	if not progress_slider.value_changed.is_connected(_on_progress_slider_value_changed):
		progress_slider.value_changed.connect(_on_progress_slider_value_changed)
	if not progress_slider.drag_started.is_connected(_on_progress_slider_drag_started):
		progress_slider.drag_started.connect(_on_progress_slider_drag_started)
	if not progress_slider.drag_ended.is_connected(_on_progress_slider_drag_ended):
		progress_slider.drag_ended.connect(_on_progress_slider_drag_ended)
	if not audio_player.finished.is_connected(_on_audio_player_finished):
		audio_player.finished.connect(_on_audio_player_finished)
	_reset_progress_ui()
	_sync_play_pause_button()


func open_file(path: String) -> bool:
	audio_player.stop()
	audio_player.stream_paused = false

	var stream: AudioStream = _load_audio_stream(path)
	if stream == null:
		audio_player.stream = null
		_duration = 0.0
		_reset_progress_ui()
		_sync_play_pause_button()
		return false

	audio_player.stream = stream
	_duration = _get_stream_length(stream)
	_reset_progress_ui()
	_sync_play_pause_button()
	return true


func _load_audio_stream(path: String) -> AudioStream:
	var loaded: Resource = GlobalEditorResourceLoader.load_resource(path)
	var stream: AudioStream = loaded as AudioStream
	if stream != null:
		return stream

	if path.get_extension().to_lower() == "ogg":
		return AudioStreamOggVorbis.load_from_file(path)
	return null


func _process(_delta: float) -> void:
	_update_progress_ui()
	_sync_play_pause_button()


func _notification(what: int) -> void:
	if what == NOTIFICATION_VISIBILITY_CHANGED and not is_visible_in_tree():
		if not is_node_ready():
			return
		_stop_audio_playback()


func _on_play_pause_button_pressed() -> void:
	if audio_player.stream == null:
		return
	if audio_player.playing and not audio_player.stream_paused:
		audio_player.stream_paused = true
	else:
		if audio_player.playing and audio_player.stream_paused:
			audio_player.stream_paused = false
		else:
			audio_player.play(progress_slider.value)
			audio_player.stream_paused = false
	_sync_play_pause_button()


func _on_progress_slider_value_changed(value: float) -> void:
	if _is_slider_updating:
		return
	if audio_player.stream == null:
		return
	if _is_slider_dragging:
		audio_player.seek(value)
		_update_time_label(value)


func _on_progress_slider_drag_started() -> void:
	_is_slider_dragging = true


func _on_progress_slider_drag_ended(value_changed: bool) -> void:
	_is_slider_dragging = false
	if audio_player.stream == null:
		return
	if value_changed:
		var target_time: float = progress_slider.value
		audio_player.seek(target_time)
		_update_time_label(target_time)


func _update_progress_ui() -> void:
	if audio_player.stream == null:
		return
	if _is_slider_dragging:
		return

	var duration: float = _duration
	if duration <= 0.0:
		duration = _get_stream_length(audio_player.stream)
		_duration = duration

	if duration <= 0.0:
		_update_time_label(audio_player.get_playback_position())
		return

	if progress_slider.max_value != duration:
		progress_slider.max_value = duration

	var pos: float = audio_player.get_playback_position()
	if pos < 0.0:
		pos = 0.0
	elif pos > duration:
		pos = duration

	_set_slider_value(pos)
	_update_time_label(pos)


func _reset_progress_ui() -> void:
	progress_slider.min_value = 0.0
	progress_slider.max_value = _duration if _duration > 0.0 else 1.0
	_set_slider_value(0.0)
	_update_time_label(0.0)


func _set_slider_value(value: float) -> void:
	_is_slider_updating = true
	progress_slider.value = value
	_is_slider_updating = false


func _update_time_label(current_time: float) -> void:
	var duration: float = _duration
	if duration < 0.0:
		duration = 0.0
	time_label.text = "%s / %s" % [_format_seconds(current_time), _format_seconds(duration)]


func _format_seconds(seconds_value: float) -> String:
	var clamped_seconds: float = seconds_value
	if clamped_seconds < 0.0:
		clamped_seconds = 0.0
	var total_seconds: int = int(clamped_seconds)
	@warning_ignore("integer_division")
	var minutes: int = int(total_seconds / 60)
	var seconds: int = total_seconds % 60
	return "%02d:%02d" % [minutes, seconds]


func _get_stream_length(stream: AudioStream) -> float:
	if stream == null:
		return 0.0
	var length: float = stream.get_length()
	if length < 0.0:
		return 0.0
	return length


func _on_audio_player_finished() -> void:
	_sync_play_pause_button()


func _stop_audio_playback() -> void:
	audio_player.stop()
	audio_player.stream_paused = false
	_set_slider_value(0.0)
	_update_time_label(0.0)
	_sync_play_pause_button()


func _sync_play_pause_button() -> void:
	var has_stream: bool = audio_player.stream != null
	play_pause_button.disabled = not has_stream

	var playing_active: bool = has_stream and audio_player.playing and not audio_player.stream_paused
	var icon: Texture2D = PAUSE_ICON if playing_active else PLAY_ICON
	play_pause_button.texture_normal = icon
	play_pause_button.texture_pressed = icon
	play_pause_button.texture_hover = icon
