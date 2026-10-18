---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/main_window/playtest_launcher.cpp"
ref: HEAD
---

#include "playtest_launcher.h"

#include <QProcess>

#include <system_error>

namespace fs = std::filesystem;

namespace hivewe {

QString canonical_existing_path(const fs::path& path) {
	std::error_code error;
	const fs::path canonical_path = fs::weakly_canonical(path, error);
	return QString::fromStdString((error ? path : canonical_path).string());
}

QString play_test_executable_path(const fs::path& warcraft_root) {
#if defined(__APPLE__)
	const fs::path macos_binary = warcraft_root / "_retail_" / "x86_64" / "Warcraft III.app" / "Contents" / "MacOS" / "Warcraft III";
	if (fs::exists(macos_binary)) {
		return canonical_existing_path(macos_binary);
	}
#endif

	const fs::path windows_binary = warcraft_root / "x86_64" / "Warcraft III.exe";
	if (fs::exists(windows_binary)) {
		return canonical_existing_path(windows_binary);
	}

	return {};
}

QStringList play_test_arguments(const fs::path& map_path, const QString& extra_arguments) {
	QStringList arguments;
	arguments << "-launch"
			  << "-loadfile" << canonical_existing_path(map_path);

	if (!extra_arguments.trimmed().isEmpty()) {
		arguments << QProcess::splitCommand(extra_arguments);
	}
	return arguments;
}

}
