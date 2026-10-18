---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/scenario_properties_editor.cpp"
ref: HEAD
---

#include "scenario_properties_editor.h"

#include <glm/glm.hpp>

#include <QAbstractItemView>
#include <QBrush>
#include <QCheckBox>
#include <QComboBox>
#include <QDialogButtonBox>
#include <QDragMoveEvent>
#include <QDropEvent>
#include <QGraphicsOpacityEffect>
#include <QHBoxLayout>
#include <QHeaderView>
#include <QIcon>
#include <QMessageBox>
#include <QPixmap>
#include <QPushButton>
#include <QTableWidget>
#include <QTreeWidget>
#include <QVBoxLayout>

import std;
import MapGlobal;
import MapInfo;
import RenderManager;

namespace {
constexpr int players_column_number = 0;
constexpr int players_column_name = 1;
constexpr int players_column_color = 2;
constexpr int players_column_race = 3;
constexpr int players_column_controller = 4;
constexpr int players_column_fixed_start = 5;

constexpr int forces_column_name = 0;
constexpr int forces_column_allied = 1;
constexpr int forces_column_allied_victory = 2;
constexpr int forces_column_share_vision = 3;
constexpr int forces_column_share_unit_control = 4;
constexpr int forces_column_share_advanced_unit_control = 5;

QIcon player_color_icon(const int internal_number) {
	const glm::u8vec4 color = RenderManager::player_color(internal_number);
	QPixmap pixmap(12, 12);
	pixmap.fill(QColor(color.r, color.g, color.b));
	return QIcon(pixmap);
}
}

ForceTreeWidget::ForceTreeWidget(QWidget* parent) : QTreeWidget(parent) {
	setDragEnabled(true);
	setAcceptDrops(true);
	setDropIndicatorShown(true);
	// Not InternalMove: we fully hand-roll the reparenting below, and letting Qt's
	// InternalMove machinery also think a Qt::MoveAction happened makes
	// QAbstractItemView::startDrag() additionally delete "the row that was dragged"
	// after drag->exec() returns — but since we already moved the item ourselves,
	// that second deletion lands on whatever item now happens to sit at the old
	// index (a sibling player, or the force itself if it emptied out), which is
	// exactly the "items randomly disappear" symptom this mode caused.
	setDragDropMode(QAbstractItemView::DragDrop);
	setSelectionMode(QAbstractItemView::SingleSelection);
	setHeaderHidden(true);
}

void ForceTreeWidget::dragMoveEvent(QDragMoveEvent* event) {
	event->acceptProposedAction();
}

void ForceTreeWidget::dropEvent(QDropEvent* event) {
	QTreeWidgetItem* target = itemAt(event->position().toPoint());
	QTreeWidgetItem* source = currentItem();

	// Only a player leaf (has a parent) can be dragged; dropping nowhere or
	// dragging a force itself is a no-op.
	if (!target || !source || !source->parent()) {
		event->ignore();
		return;
	}

	// Dropping "on" a player lands in that player's force.
	QTreeWidgetItem* target_force = target->parent() ? target->parent() : target;
	QTreeWidgetItem* source_force = source->parent();

	if (target_force == source_force) {
		event->ignore();
		return;
	}

	const int player_internal_number = source->data(0, Qt::UserRole).toInt();
	const int target_force_row = indexOfTopLevelItem(target_force);

	source_force->removeChild(source);
	target_force->addChild(source);
	target_force->setExpanded(true);
	setCurrentItem(source);

	// Accept the drop, but explicitly as a non-move action: we already relocated
	// the item by hand above, so we must not let Qt's startDrag() think a
	// Qt::MoveAction happened (see setDragDropMode comment) and delete it again.
	event->setDropAction(Qt::IgnoreAction);
	event->accept();

	emit playerDropped(player_internal_number, target_force_row);
}

ScenarioPropertiesEditor::ScenarioPropertiesEditor(QWidget* parent) : QDialog(parent) {
	setAttribute(Qt::WA_DeleteOnClose);
	setWindowTitle("Scenario Properties");
	resize(1200, 720);

	ensure_all_player_slots_exist();
	ensure_every_player_has_a_force();

	auto* root = new QVBoxLayout(this);

	tabs = new QTabWidget(this);
	root->addWidget(tabs, 1);

	auto* players_tab = new QWidget;
	build_players_tab(players_tab);
	tabs->addTab(players_tab, "Players");

	auto* forces_tab = new QWidget;
	build_forces_tab(forces_tab);
	tabs->addTab(forces_tab, "Forces");

	auto* buttons = new QDialogButtonBox(QDialogButtonBox::Close);
	root->addWidget(buttons);
	connect(buttons, &QDialogButtonBox::rejected, this, &QDialog::close);

	show();
}

void ScenarioPropertiesEditor::refresh_all() {
	ensure_all_player_slots_exist();
	ensure_every_player_has_a_force();

	refresh_players_table();
	refresh_forces_tree();
	refresh_forces_flags_table();
	update_force_buttons_enabled();

	updating_ui = true;
	if (use_custom_forces_checkbox) {
		use_custom_forces_checkbox->setChecked(map->info.custom_forces);
	}
	if (fixed_player_settings_checkbox) {
		fixed_player_settings_checkbox->setChecked(map->info.fixed_player_settings);
	}
	updating_ui = false;
}

// --- Players tab ---------------------------------------------------------

namespace {
constexpr int max_player_slots = 24;
}

void ScenarioPropertiesEditor::ensure_all_player_slots_exist() {
	std::array<bool, max_player_slots> has_slot {};
	for (const auto& player : map->info.players) {
		if (player.internal_number >= 0 && player.internal_number < max_player_slots) {
			has_slot[player.internal_number] = true;
		}
	}

	// war3map.w3i technically allows storing fewer than 24 players (some maps/tools
	// trim unused slots to shrink the file), but the World Editor always operates on
	// a fixed 24-slot model and pads missing slots with inactive defaults when it
	// opens Player Properties. Mirror that here so every slot is available to
	// configure, rather than only whichever ones happened to be on disk.
	for (int i = 0; i < max_player_slots; ++i) {
		if (has_slot[i]) {
			continue;
		}

		PlayerData player {};
		player.internal_number = i;
		player.type = PlayerType::neutral;
		player.race = PlayerRace::human;
		player.fixed_start_position = 0;
		player.starting_position = glm::vec2(0.f);
		player.ally_low_priorities_flags = 0;
		player.ally_high_priorities_flags = 0;
		map->trigger_strings.set_string(player.name, "Player " + std::to_string(i + 1));
		map->info.players.push_back(std::move(player));
	}

	std::ranges::sort(map->info.players, {}, &PlayerData::internal_number);
}

void ScenarioPropertiesEditor::build_players_tab(QWidget* tab) {
	auto* layout = new QVBoxLayout(tab);

	players_table = new QTableWidget(0, 6, tab);
	players_table->setHorizontalHeaderLabels({ "#", "Player Name", "Color", "Race", "Controller", "Fixed Start Location" });
	players_table->verticalHeader()->setVisible(false);
	players_table->horizontalHeader()->setSectionResizeMode(QHeaderView::ResizeToContents);
	players_table->horizontalHeader()->setSectionResizeMode(players_column_name, QHeaderView::Stretch);
	players_table->setSelectionMode(QAbstractItemView::NoSelection);
	players_table->setEditTriggers(QAbstractItemView::DoubleClicked | QAbstractItemView::EditKeyPressed);
	layout->addWidget(players_table, 1);

	connect(players_table, &QTableWidget::itemChanged, this, [this](QTableWidgetItem* item) {
		if (updating_ui) {
			return;
		}
		const int row = item->row();
		if (row < 0 || row >= static_cast<int>(map->info.players.size())) {
			return;
		}
		auto& player = map->info.players[row];
		if (item->column() == players_column_name) {
			map->trigger_strings.set_string(player.name, item->text().toStdString());
		} else if (item->column() == players_column_fixed_start) {
			player.fixed_start_position = item->checkState() == Qt::Checked ? 1 : 0;
		}
	});

	auto* reset_row = new QHBoxLayout;
	reset_row->addStretch(1);
	auto* reset_button = new QPushButton("Reset Players to Defaults");
	connect(reset_button, &QPushButton::clicked, this, &ScenarioPropertiesEditor::reset_players_to_defaults);
	reset_row->addWidget(reset_button);
	layout->addLayout(reset_row);

	refresh_players_table();
}

void ScenarioPropertiesEditor::refresh_players_table() {
	updating_ui = true;

	players_table->setRowCount(static_cast<int>(map->info.players.size()));
	players_table->clearContents();

	for (int row = 0; row < static_cast<int>(map->info.players.size()); ++row) {
		const auto& player = map->info.players[row];

		auto* number_item = new QTableWidgetItem(QString::number(player.internal_number + 1));
		number_item->setFlags(number_item->flags() & ~Qt::ItemIsEditable);
		players_table->setItem(row, players_column_number, number_item);

		auto* name_item = new QTableWidgetItem(QString::fromUtf8(map->trigger_strings.string(player.name)));
		players_table->setItem(row, players_column_name, name_item);

		auto* color_item = new QTableWidgetItem();
		color_item->setFlags(color_item->flags() & ~(Qt::ItemIsEditable | Qt::ItemIsSelectable));
		const glm::u8vec4 color = RenderManager::player_color(player.internal_number);
		color_item->setBackground(QColor(color.r, color.g, color.b));
		players_table->setItem(row, players_column_color, color_item);

		auto* race_combo = new QComboBox;
		race_combo->addItem("Human", static_cast<int>(PlayerRace::human));
		race_combo->addItem("Orc", static_cast<int>(PlayerRace::orc));
		race_combo->addItem("Undead", static_cast<int>(PlayerRace::undead));
		race_combo->addItem("Night Elf", static_cast<int>(PlayerRace::night_elf));
		race_combo->addItem("Selectable", static_cast<int>(PlayerRace::selectable));
		race_combo->setCurrentIndex(race_combo->findData(static_cast<int>(player.race)));
		connect(race_combo, QOverload<int>::of(&QComboBox::currentIndexChanged), this, [this, row, race_combo](int) {
			if (updating_ui) {
				return;
			}
			map->info.players[row].race = static_cast<PlayerRace>(race_combo->currentData().toInt());
		});
		players_table->setCellWidget(row, players_column_race, race_combo);

		auto* controller_combo = new QComboBox;
		controller_combo->addItem("User", static_cast<int>(PlayerType::human));
		controller_combo->addItem("Computer", static_cast<int>(PlayerType::computer));
		controller_combo->addItem("None", static_cast<int>(PlayerType::neutral));
		controller_combo->addItem("Rescuable", static_cast<int>(PlayerType::rescuable));
		controller_combo->setCurrentIndex(controller_combo->findData(static_cast<int>(player.type)));
		connect(controller_combo, QOverload<int>::of(&QComboBox::currentIndexChanged), this, [this, row, controller_combo](int) {
			if (updating_ui) {
				return;
			}
			const auto type = static_cast<PlayerType>(controller_combo->currentData().toInt());
			map->info.players[row].type = type;
			apply_player_row_active_style(row, type != PlayerType::neutral);
			sync_forces_after_controller_change();
		});
		players_table->setCellWidget(row, players_column_controller, controller_combo);

		auto* fixed_start_item = new QTableWidgetItem();
		fixed_start_item->setFlags((fixed_start_item->flags() & ~Qt::ItemIsEditable) | Qt::ItemIsUserCheckable);
		fixed_start_item->setCheckState(player.fixed_start_position ? Qt::Checked : Qt::Unchecked);
		players_table->setItem(row, players_column_fixed_start, fixed_start_item);

		apply_player_row_active_style(row, player.type != PlayerType::neutral);
	}

	updating_ui = false;
}

void ScenarioPropertiesEditor::apply_player_row_active_style(const int row, const bool is_active) {
	static const QColor inactive_text_color(140, 140, 140);

	if (auto* number_item = players_table->item(row, players_column_number)) {
		number_item->setForeground(is_active ? QBrush() : QBrush(inactive_text_color));
	}
	if (auto* name_item = players_table->item(row, players_column_name)) {
		name_item->setForeground(is_active ? QBrush() : QBrush(inactive_text_color));
	}
	if (auto* fixed_start_item = players_table->item(row, players_column_fixed_start)) {
		fixed_start_item->setForeground(is_active ? QBrush() : QBrush(inactive_text_color));
	}
	// The Controller combo is left at full visibility since it's the control used to
	// activate the slot; dimming it too would obscure the exact thing to click.
	if (auto* race_combo = players_table->cellWidget(row, players_column_race)) {
		if (is_active) {
			race_combo->setGraphicsEffect(nullptr);
		} else {
			auto* effect = new QGraphicsOpacityEffect(race_combo);
			effect->setOpacity(0.5);
			race_combo->setGraphicsEffect(effect);
		}
	}
}

void ScenarioPropertiesEditor::reset_players_to_defaults() {
	if (QMessageBox::question(
			this,
			"Reset Players to Defaults",
			"Reset every player's controller, race, and fixed start location to their default values?\nThis cannot be undone."
		)
		!= QMessageBox::Yes) {
		return;
	}

	const int default_active_player_count = std::min(4, static_cast<int>(map->info.players.size()));
	for (int i = 0; i < static_cast<int>(map->info.players.size()); ++i) {
		auto& player = map->info.players[i];
		player.type = i < default_active_player_count ? PlayerType::human : PlayerType::neutral;
		player.race = PlayerRace::selectable;
		player.fixed_start_position = 1;
	}

	refresh_players_table();
}

// --- Forces tab -----------------------------------------------------------

void ScenarioPropertiesEditor::build_forces_tab(QWidget* tab) {
	auto* layout = new QVBoxLayout(tab);

	auto* checkboxes_row = new QHBoxLayout;
	use_custom_forces_checkbox = new QCheckBox("Use Custom Forces");
	use_custom_forces_checkbox->setToolTip(
		"Apply the forces and alliance settings configured below when the game starts. If unchecked, these "
		"settings are ignored and default team assignment is used instead."
	);
	use_custom_forces_checkbox->setChecked(map->info.custom_forces);
	connect(use_custom_forces_checkbox, &QCheckBox::toggled, this, [this](bool checked) {
		if (updating_ui) {
			return;
		}
		map->info.custom_forces = checked;
	});

	fixed_player_settings_checkbox = new QCheckBox("Fixed Player Settings");
	fixed_player_settings_checkbox->setToolTip(
		"Prevent players from changing team, color, or race in the game lobby. Forces the settings configured "
		"in this dialog."
	);
	fixed_player_settings_checkbox->setChecked(map->info.fixed_player_settings);
	connect(fixed_player_settings_checkbox, &QCheckBox::toggled, this, [this](bool checked) {
		if (updating_ui) {
			return;
		}
		map->info.fixed_player_settings = checked;
	});

	checkboxes_row->addWidget(use_custom_forces_checkbox);
	checkboxes_row->addWidget(fixed_player_settings_checkbox);
	checkboxes_row->addStretch(1);
	layout->addLayout(checkboxes_row);

	auto* content_row = new QHBoxLayout;
	layout->addLayout(content_row, 1);

	auto* tree_column = new QVBoxLayout;
	forces_tree = new ForceTreeWidget(tab);
	tree_column->addWidget(forces_tree, 1);

	auto* tree_buttons = new QHBoxLayout;
	add_force_button = new QPushButton("Add Force");
	remove_force_button = new QPushButton("Remove Force");
	rename_force_button = new QPushButton("Rename Force");
	tree_buttons->addWidget(add_force_button);
	tree_buttons->addWidget(remove_force_button);
	tree_buttons->addWidget(rename_force_button);
	tree_column->addLayout(tree_buttons);

	content_row->addLayout(tree_column, 1);

	forces_flags_table = new QTableWidget(0, 6, tab);
	forces_flags_table->setHorizontalHeaderLabels(
		{ "Force", "Allied", "Allied Victory", "Share Vision", "Share Unit Control", "Share Adv. Unit Control" }
	);
	forces_flags_table->horizontalHeaderItem(forces_column_allied)
		->setToolTip("Players in this force start the game allied with each other instead of as enemies.");
	forces_flags_table->horizontalHeaderItem(forces_column_allied_victory)
		->setToolTip(
			"If one player in this force achieves victory, every other player still in the force wins too "
			"(shared victory)."
		);
	forces_flags_table->horizontalHeaderItem(forces_column_share_vision)
		->setToolTip(
			"Players in this force share vision with each other (see what allies see), without control over "
			"each other's units."
		);
	forces_flags_table->horizontalHeaderItem(forces_column_share_unit_control)
		->setToolTip(
			"Allies in this force can give basic orders (move/attack) to each other's units, but can't spend "
			"resources or cast spells with them."
		);
	forces_flags_table->horizontalHeaderItem(forces_column_share_advanced_unit_control)
		->setToolTip(
			"Allies in this force have full control of each other's units \xe2\x80\x94 including training, "
			"building, spending resources, and casting spells."
		);
	forces_flags_table->verticalHeader()->setVisible(false);
	forces_flags_table->horizontalHeader()->setSectionResizeMode(QHeaderView::ResizeToContents);
	forces_flags_table->horizontalHeader()->setSectionResizeMode(forces_column_name, QHeaderView::Stretch);
	forces_flags_table->setSelectionMode(QAbstractItemView::NoSelection);
	forces_flags_table->setEditTriggers(QAbstractItemView::NoEditTriggers);
	content_row->addWidget(forces_flags_table, 2);

	connect(add_force_button, &QPushButton::clicked, this, &ScenarioPropertiesEditor::add_force);
	connect(remove_force_button, &QPushButton::clicked, this, &ScenarioPropertiesEditor::remove_force);
	connect(rename_force_button, &QPushButton::clicked, this, &ScenarioPropertiesEditor::rename_selected_force);

	connect(forces_tree, &QTreeWidget::currentItemChanged, this, [this](QTreeWidgetItem*, QTreeWidgetItem*) {
		update_force_buttons_enabled();
	});
	connect(forces_tree, &QTreeWidget::itemChanged, this, [this](QTreeWidgetItem* item, int column) {
		if (updating_ui || column != 0 || item->parent() != nullptr) {
			return;
		}
		const int force_row = forces_tree->indexOfTopLevelItem(item);
		if (force_row < 0 || force_row >= static_cast<int>(map->info.forces.size())) {
			return;
		}
		map->trigger_strings.set_string(map->info.forces[force_row].name, item->text(0).toStdString());
		refresh_forces_flags_table();
	});
	connect(forces_tree, &ForceTreeWidget::playerDropped, this, &ScenarioPropertiesEditor::on_player_dropped);

	connect(forces_flags_table, &QTableWidget::itemChanged, this, [this](QTableWidgetItem* item) {
		if (updating_ui) {
			return;
		}
		const int row = item->row();
		if (row < 0 || row >= static_cast<int>(map->info.forces.size())) {
			return;
		}
		auto& force = map->info.forces[row];
		const bool checked = item->checkState() == Qt::Checked;
		switch (item->column()) {
			case forces_column_allied:
				force.allied = checked;
				break;
			case forces_column_allied_victory:
				force.allied_victory = checked;
				break;
			case forces_column_share_vision:
				force.share_vision = checked;
				break;
			case forces_column_share_unit_control:
				force.share_unit_control = checked;
				break;
			case forces_column_share_advanced_unit_control:
				force.share_advanced_unit_control = checked;
				break;
			default:
				break;
		}
	});

	refresh_forces_tree();
	refresh_forces_flags_table();
	update_force_buttons_enabled();
}

void ScenarioPropertiesEditor::refresh_forces_tree() {
	updating_ui = true;
	forces_tree->clear();

	for (const auto& force : map->info.forces) {
		auto* force_item = new QTreeWidgetItem(forces_tree);
		force_item->setText(0, QString::fromUtf8(map->trigger_strings.string(force.name)));
		force_item->setFlags(Qt::ItemIsEnabled | Qt::ItemIsSelectable | Qt::ItemIsEditable);

		for (const auto& player : map->info.players) {
			// Slots with no controller aren't in play, so — matching the original World
			// Editor — they don't appear as assignable force members at all.
			if (player.type == PlayerType::neutral) {
				continue;
			}
			if (force.player_masks & (1 << player.internal_number)) {
				auto* player_item = new QTreeWidgetItem(force_item);
				player_item->setText(0, QString::fromUtf8(map->trigger_strings.string(player.name)));
				player_item->setIcon(0, player_color_icon(player.internal_number));
				player_item->setFlags(Qt::ItemIsEnabled | Qt::ItemIsSelectable | Qt::ItemIsDragEnabled);
				player_item->setData(0, Qt::UserRole, player.internal_number);
			}
		}
		force_item->setExpanded(true);
	}

	updating_ui = false;
}

void ScenarioPropertiesEditor::refresh_forces_flags_table() {
	updating_ui = true;

	forces_flags_table->setRowCount(static_cast<int>(map->info.forces.size()));
	forces_flags_table->clearContents();

	for (int row = 0; row < static_cast<int>(map->info.forces.size()); ++row) {
		const auto& force = map->info.forces[row];

		auto* name_item = new QTableWidgetItem(QString::fromUtf8(map->trigger_strings.string(force.name)));
		name_item->setFlags(name_item->flags() & ~Qt::ItemIsEditable);
		forces_flags_table->setItem(row, forces_column_name, name_item);

		const std::array<std::pair<int, bool>, 5> flags = { {
			{ forces_column_allied, force.allied },
			{ forces_column_allied_victory, force.allied_victory },
			{ forces_column_share_vision, force.share_vision },
			{ forces_column_share_unit_control, force.share_unit_control },
			{ forces_column_share_advanced_unit_control, force.share_advanced_unit_control },
		} };
		for (const auto& [column, checked] : flags) {
			auto* item = new QTableWidgetItem();
			item->setFlags((item->flags() & ~Qt::ItemIsEditable) | Qt::ItemIsUserCheckable);
			item->setCheckState(checked ? Qt::Checked : Qt::Unchecked);
			forces_flags_table->setItem(row, column, item);
		}
	}

	updating_ui = false;
}

void ScenarioPropertiesEditor::ensure_every_player_has_a_force() {
	if (map->info.forces.empty()) {
		ForceData& force = map->info.forces.emplace_back();
		map->trigger_strings.set_string(force.name, "Force 1");
	}

	for (const auto& player : map->info.players) {
		// Inactive (controller "None") slots aren't part of the team system at all,
		// matching refresh_forces_tree()'s filtering.
		if (player.type == PlayerType::neutral) {
			continue;
		}
		const bool has_force = std::ranges::any_of(map->info.forces, [&](const ForceData& force) {
			return (force.player_masks & (1 << player.internal_number)) != 0;
		});
		if (!has_force) {
			map->info.forces.front().player_masks |= (1 << player.internal_number);
		}
	}
}

void ScenarioPropertiesEditor::sync_forces_after_controller_change() {
	// A player's controller can flip between None and active on the Players tab
	// while the Forces tab is already built; keep it in sync live rather than only
	// on dialog open, so newly-activated players immediately gain a force and
	// deactivated ones immediately drop out of the tree.
	ensure_every_player_has_a_force();
	refresh_forces_tree();
	refresh_forces_flags_table();
	update_force_buttons_enabled();
}

void ScenarioPropertiesEditor::add_force() {
	map->info.forces.emplace_back();
	ForceData& force = map->info.forces.back();
	map->trigger_strings.set_string(force.name, "Force " + std::to_string(map->info.forces.size()));

	refresh_forces_tree();
	refresh_forces_flags_table();
	update_force_buttons_enabled();

	if (forces_tree->topLevelItemCount() > 0) {
		forces_tree->setCurrentItem(forces_tree->topLevelItem(forces_tree->topLevelItemCount() - 1));
	}
}

void ScenarioPropertiesEditor::remove_force() {
	QTreeWidgetItem* current = forces_tree->currentItem();
	if (!current || current->parent()) {
		return;
	}

	if (map->info.forces.size() <= 1) {
		QMessageBox::warning(this, "Cannot Remove Force", "At least one Force is required.");
		return;
	}

	const int removed_row = forces_tree->indexOfTopLevelItem(current);
	if (removed_row < 0 || removed_row >= static_cast<int>(map->info.forces.size())) {
		return;
	}

	// Reassign members of the removed force rather than leaving them orphaned;
	// every player must belong to exactly one force.
	const int fallback_row = removed_row == 0 ? 1 : 0;
	map->info.forces[fallback_row].player_masks |= map->info.forces[removed_row].player_masks;
	map->info.forces.erase(map->info.forces.begin() + removed_row);

	refresh_forces_tree();
	refresh_forces_flags_table();
	update_force_buttons_enabled();
}

void ScenarioPropertiesEditor::rename_selected_force() {
	QTreeWidgetItem* current = forces_tree->currentItem();
	if (!current || current->parent()) {
		return;
	}
	forces_tree->editItem(current, 0);
}

void ScenarioPropertiesEditor::on_player_dropped(const int player_internal_number, const int target_force_row) {
	if (target_force_row < 0 || target_force_row >= static_cast<int>(map->info.forces.size())) {
		return;
	}

	for (auto& force : map->info.forces) {
		force.player_masks &= ~(1 << player_internal_number);
	}
	map->info.forces[target_force_row].player_masks |= (1 << player_internal_number);
}

void ScenarioPropertiesEditor::update_force_buttons_enabled() {
	QTreeWidgetItem* current = forces_tree->currentItem();
	const bool is_force_selected = current && !current->parent();
	remove_force_button->setEnabled(is_force_selected && map->info.forces.size() > 1);
	rename_force_button->setEnabled(is_force_selected);
}
