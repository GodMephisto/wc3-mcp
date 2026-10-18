---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/palette.cpp"
ref: HEAD
---

#include "palette.h"

#include <QEvent>

Palette::Palette(QWidget* parent) : QDialog(parent) {
	setWindowFlag(Qt::Widget, true);
	setWindowFlag(Qt::Dialog, false);
}

Palette::~Palette()
{
}

void Palette::monitor_activation(QWidget* widget) {
	QWidget* target = widget ? widget : this;
	target->installEventFilter(this);

	const auto children = target->findChildren<QWidget*>();
	for (auto* child : children) {
		child->installEventFilter(this);
	}
}

bool Palette::eventFilter(QObject* watched, QEvent* event) {
	if (event->type() == QEvent::MouseButtonPress || event->type() == QEvent::FocusIn) {
		emit activated();
	}

	return QDialog::eventFilter(watched, event);
}

//void Palette::addShortcut(const QKeySequence sequence, const std::vector<QWidget*>& attach_to) {
//	for (auto&& i : attach_to) {
//		shortcuts.push_back(new QShortcut(sequence, i));
//	}
//}
