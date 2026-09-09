extends Node2D

const MAX_ROOMS := 10
const RUN_LIMIT := 600.0
const MANA_PER_SECOND := 1.0

const HEROES := {
	"전사": {"hp": 160.0, "attack": 18.0, "speed": 0.9, "color": Color("ef5350"), "skill": "회전 베기"},
	"궁수": {"hp": 105.0, "attack": 15.0, "speed": 1.35, "color": Color("42a5f5"), "skill": "관통 사격"},
	"마법사": {"hp": 90.0, "attack": 22.0, "speed": 1.0, "color": Color("eceff1"), "skill": "마력 폭발"},
}

const ARTIFACTS := [
	{"name": "붉은 핵", "desc": "공격력 +20%, 치명타 확률 +8%", "attack": 0.20, "crit": 0.08},
	{"name": "푸른 깃털", "desc": "공격속도 +18%, 이동속도 +15%, 회피 +5%", "speed": 0.18, "move": 0.15, "dodge": 0.05},
	{"name": "백색 결정", "desc": "순수 공격력 +6, 최대 체력 +20", "flat_attack": 6.0, "hp": 20.0},
]

var hero_name := ""
var room := 0
var elapsed := 0.0
var mana := 0.0
var max_mana := 10.0
var hp := 100.0
var max_hp := 100.0
var attack := 10.0
var attack_speed := 1.0
var move_speed := 250.0
var crit_chance := 0.05
var dodge_chance := 0.0
var enemy_hp := 0.0
var enemy_max_hp := 0.0
var attack_timer := 0.0
var enemy_timer := 0.0
var playing := false
var choosing_artifact := false
var rng := RandomNumberGenerator.new()

var title_label: Label
var info_label: Label
var hp_bar: ProgressBar
var mana_bar: ProgressBar
var enemy_bar: ProgressBar
var room_label: Label
var timer_label: Label
var message_label: Label
var action_button: Button
var choice_box: HBoxContainer

func _ready() -> void:
	rng.randomize()
	build_ui()
	show_hero_select()

func build_ui() -> void:
	var bg := ColorRect.new()
	bg.color = Color("11172a")
	bg.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	add_child(bg)

	var root := VBoxContainer.new()
	root.position = Vector2(110, 55)
	root.size = Vector2(1060, 610)
	root.add_theme_constant_override("separation", 18)
	add_child(root)

	title_label = Label.new()
	title_label.text = "동아리 던전"
	title_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	title_label.add_theme_font_size_override("font_size", 36)
	root.add_child(title_label)

	var top := HBoxContainer.new()
	top.alignment = BoxContainer.ALIGNMENT_CENTER
	top.add_theme_constant_override("separation", 80)
	root.add_child(top)
	room_label = make_label("방 0 / 10", 22)
	timer_label = make_label("10:00", 22)
	top.add_child(room_label)
	top.add_child(timer_label)

	info_label = make_label("", 18)
	info_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	root.add_child(info_label)

	hp_bar = make_bar("체력", Color("ef5350"))
	mana_bar = make_bar("마나", Color("42a5f5"))
	enemy_bar = make_bar("적 체력", Color("ffb74d"))
	root.add_child(hp_bar)
	root.add_child(mana_bar)
	root.add_child(enemy_bar)

	message_label = make_label("", 21)
	message_label.custom_minimum_size.y = 80
	message_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	message_label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	message_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	root.add_child(message_label)

	choice_box = HBoxContainer.new()
	choice_box.alignment = BoxContainer.ALIGNMENT_CENTER
	choice_box.add_theme_constant_override("separation", 18)
	root.add_child(choice_box)

	action_button = Button.new()
	action_button.text = "스킬 사용 (마나 5)"
	action_button.custom_minimum_size = Vector2(0, 60)
	action_button.add_theme_font_size_override("font_size", 20)
	action_button.pressed.connect(use_skill)
	root.add_child(action_button)

func make_label(text_value: String, size_value: int) -> Label:
	var label := Label.new()
	label.text = text_value
	label.add_theme_font_size_override("font_size", size_value)
	return label

func make_bar(label_text: String, color: Color) -> ProgressBar:
	var bar := ProgressBar.new()
	bar.custom_minimum_size.y = 34
	bar.show_percentage = false
	bar.set_meta("label", label_text)
	var fill := StyleBoxFlat.new()
	fill.bg_color = color
	fill.corner_radius_top_left = 8
	fill.corner_radius_top_right = 8
	fill.corner_radius_bottom_left = 8
	fill.corner_radius_bottom_right = 8
	bar.add_theme_stylebox_override("fill", fill)
	return bar

func clear_choices() -> void:
	for child in choice_box.get_children():
		child.queue_free()

func show_hero_select() -> void:
	playing = false
	action_button.hide()
	enemy_bar.hide()
	hp_bar.hide()
	mana_bar.hide()
	room_label.text = "10개의 방"
	timer_label.text = "제한 시간 10:00"
	info_label.text = "고유한 능력치를 가진 캐릭터를 선택하세요."
	message_label.text = "전사: 높은 체력  ·  궁수: 빠른 공격  ·  마법사: 강한 스킬"
	clear_choices()
	for name in HEROES:
		var button := Button.new()
		button.text = name
		button.custom_minimum_size = Vector2(230, 70)
		button.add_theme_font_size_override("font_size", 24)
		button.pressed.connect(start_run.bind(name))
		choice_box.add_child(button)

func start_run(selected: String) -> void:
	clear_choices()
	hero_name = selected
	var data: Dictionary = HEROES[selected]
	max_hp = data.hp
	hp = max_hp
	attack = data.attack
	attack_speed = data.speed
	crit_chance = 0.05
	dodge_chance = 0.0
	move_speed = 250.0
	mana = 0.0
	elapsed = 0.0
	room = 0
	playing = true
	choosing_artifact = false
	hp_bar.show()
	mana_bar.show()
	enemy_bar.show()
	action_button.show()
	info_label.text = "%s  |  공격력 %.0f  |  공격속도 %.2f  |  스킬: %s" % [hero_name, attack, attack_speed, data.skill]
	start_next_room()

func start_next_room() -> void:
	room += 1
	if room > MAX_ROOMS:
		finish_run(true)
		return
	enemy_max_hp = 48.0 + room * 15.0
	if room == MAX_ROOMS:
		enemy_max_hp *= 2.0
	enemy_hp = enemy_max_hp
	attack_timer = 0.0
	enemy_timer = 0.0
	room_label.text = "방 %d / %d" % [room, MAX_ROOMS]
	message_label.text = "최종 보스 등장!" if room == MAX_ROOMS else "%d번째 방의 적이 나타났습니다." % room
	update_bars()

func _process(delta: float) -> void:
	if not playing or choosing_artifact:
		return
	elapsed += delta
	if elapsed >= RUN_LIMIT:
		finish_run(false, "제한 시간이 끝났습니다.")
		return
	mana = min(max_mana, mana + MANA_PER_SECOND * delta)
	attack_timer += delta
	enemy_timer += delta
	if attack_timer >= 1.0 / attack_speed:
		attack_timer = 0.0
		basic_attack()
	if enemy_timer >= max(0.65, 1.65 - room * 0.06):
		enemy_timer = 0.0
		enemy_attack()
	update_bars()
	update_timer()

func basic_attack() -> void:
	var damage := attack
	var critical := rng.randf() < crit_chance
	if critical:
		damage *= 2.0
	enemy_hp -= damage
	message_label.text = "치명타! %.0f 피해" % damage if critical else "자동 공격: %.0f 피해" % damage
	check_enemy_defeated()

func enemy_attack() -> void:
	if rng.randf() < dodge_chance:
		message_label.text = "공격을 회피했습니다!"
		return
	var damage := 6.0 + room * 1.5
	hp -= damage
	message_label.text = "적의 공격: %.0f 피해" % damage
	if hp <= 0.0:
		finish_run(false, "체력이 모두 소진되었습니다.")

func use_skill() -> void:
	if not playing or choosing_artifact:
		return
	if mana < 5.0:
		message_label.text = "마나가 부족합니다. 마나는 초당 1 회복됩니다."
		return
	mana -= 5.0
	var damage := attack * (2.4 if hero_name == "마법사" else 2.0)
	enemy_hp -= damage
	message_label.text = "%s 사용! %.0f 피해" % [HEROES[hero_name].skill, damage]
	check_enemy_defeated()

func check_enemy_defeated() -> void:
	if enemy_hp > 0.0:
		return
	enemy_hp = 0.0
	update_bars()
	if room == MAX_ROOMS:
		finish_run(true)
	else:
		show_artifact_choices()

func show_artifact_choices() -> void:
	choosing_artifact = true
	action_button.hide()
	message_label.text = "방을 클리어했습니다. 다음 방에 가져갈 아티팩트를 선택하세요."
	clear_choices()
	var choices := ARTIFACTS.duplicate()
	choices.shuffle()
	for artifact in choices.slice(0, 2):
		var button := Button.new()
		button.text = "%s\n%s" % [artifact.name, artifact.desc]
		button.custom_minimum_size = Vector2(430, 78)
		button.pressed.connect(choose_artifact.bind(artifact))
		choice_box.add_child(button)

func choose_artifact(artifact: Dictionary) -> void:
	attack *= 1.0 + artifact.get("attack", 0.0)
	attack += artifact.get("flat_attack", 0.0)
	attack_speed *= 1.0 + artifact.get("speed", 0.0)
	move_speed *= 1.0 + artifact.get("move", 0.0)
	crit_chance += artifact.get("crit", 0.0)
	dodge_chance += artifact.get("dodge", 0.0)
	var added_hp: float = artifact.get("hp", 0.0)
	max_hp += added_hp
	hp = min(max_hp, hp + added_hp + max_hp * 0.18)
	info_label.text = "%s  |  공격력 %.0f  |  공격속도 %.2f  |  치명타 %.0f%%  |  회피 %.0f%%" % [hero_name, attack, attack_speed, crit_chance * 100.0, dodge_chance * 100.0]
	clear_choices()
	choosing_artifact = false
	action_button.show()
	start_next_room()

func update_bars() -> void:
	hp_bar.max_value = max_hp
	hp_bar.value = max(0.0, hp)
	hp_bar.tooltip_text = "체력 %.0f / %.0f" % [max(0.0, hp), max_hp]
	mana_bar.max_value = max_mana
	mana_bar.value = mana
	mana_bar.tooltip_text = "마나 %.1f / %.0f (초당 +1)" % [mana, max_mana]
	enemy_bar.max_value = enemy_max_hp
	enemy_bar.value = max(0.0, enemy_hp)
	enemy_bar.tooltip_text = "적 체력 %.0f / %.0f" % [max(0.0, enemy_hp), enemy_max_hp]

func update_timer() -> void:
	var remaining := int(max(0.0, RUN_LIMIT - elapsed))
	timer_label.text = "%02d:%02d" % [remaining / 60, remaining % 60]

func finish_run(victory: bool, reason := "") -> void:
	playing = false
	choosing_artifact = false
	clear_choices()
	action_button.hide()
	enemy_bar.hide()
	var result := "던전 정복 성공!" if victory else "도전 종료"
	title_label.text = result
	message_label.text = ("10개의 방을 모두 돌파했습니다." if victory else reason) + "\n기록: %d초 · 도달한 방: %d" % [int(elapsed), min(room, MAX_ROOMS)]
	var retry := Button.new()
	retry.text = "다시 시작"
	retry.custom_minimum_size = Vector2(260, 65)
	retry.pressed.connect(reset_game)
	choice_box.add_child(retry)

func reset_game() -> void:
	title_label.text = "동아리 던전"
	show_hero_select()

