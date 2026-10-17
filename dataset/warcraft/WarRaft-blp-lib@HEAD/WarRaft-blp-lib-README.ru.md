---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "README.ru.md"
ref: HEAD
---

# blp-lib

> Важно
> 
> Русскоязычная документация может отставать от актуальной версии. Разработчик поддерживает актуальность только английской документации в репозитории: https://github.com/WarRaft/blp-lib.
> 
> Пожалуйста, ориентируйтесь на английскую версию: [README.md](README.md).

Библиотека на Rust для работы с файлами BLP (текстуры Blizzard) с C-интерфейсом для использования в других проектах.

## Описание

Библиотека предоставляет C-совместимый API для работы с BLP, основанный на Rust-крейте [blp](https://crates.io/crates/blp). Поддерживается загрузка BLP из буфера или с диска, декодирование в RGBA, точечное декодирование выбранного мип-уровня в PNG, извлечение JPEG для JPEG-BLP, а также кодирование изображений в BLP с контролем мипов.

По умолчанию при декодировании материализуется только базовый (верхний) мип-уровень для лучшей производительности. Для получения других мипов используйте функции декодирования выбранного мипа или кодирование с явными флагами видимости мипов.

## Артефакты и сборка

Рекомендуется использовать скрипт дистрибутивной сборки:

```bash
./build-only.sh
```

Итоги сборки (папка `dist/`):
- macOS (universal, arm64+x86_64):
  - `libblp-macos.a` — статическая библиотека
  - `libblp-macos.dylib` — динамическая библиотека
- Windows (GNU):
  - `libblp-windows.a` — статическая библиотека
  - `blp-windows.dll` — динамическая библиотека
- Linux (musl):
  - `libblp-linux.a` — статическая библиотека
- Заголовок:
  - `blp.h` — общий C/C++ заголовок для подключения

Примечание: во время сборки для Linux (musl) Cargo может вывести предупреждение о неподдерживаемом типе `cdylib`. Это ожидаемо и на функциональность не влияет. Для сборок мы оставляем это предупреждение.

## Требования

Для запуска скриптов сборки потребуются:
- rustup и cargo (стандартный Rust toolchain)
- jq
- lipo (macOS, ставится с Xcode Command Line Tools)
- file (утилита определения типа файлов)

Скрипт сам установит необходимые целевые платформы через `rustup target add` при первом запуске.

## Подключение в C/C++ проектах

- Для локальной разработки можно подключать `include/blp_lib.h`.
- Для использования артефактов из дистрибутива — подключайте `dist/blp.h`.

Примеры линковки:

- macOS:
  ```bash
  gcc -I./dist -L./dist your_program.c -o your_program -lblp-macos -ldl
  ```
- Linux (musl):
  ```bash
  gcc -I./dist -L./dist your_program.c -o your_program -lblp-linux -ldl -lpthread
  ```
- Windows (MinGW, статически):
  ```bash
  gcc -I./dist -L./dist your_program.c -o your_program.exe -lblp-windows
  ```

## Примеры на C

В каталоге `examples/` есть примеры:
- `decode_file.c` — декодирует один .blp в PNG/RGBA;
- `encode_file.c` — кодирует одно изображение в .blp;
- `decode_dir.c` — пакетное декодирование каталога .blp в папку PNG с сохранением относительных путей;
- `encode_dir.c` — пакетное кодирование каталога изображений в .blp.

Собрать примеры можно скриптом:
```bash
./build-examples.sh
```
Готовые бинарники окажутся в `examples/dist/`.

Для быстрой проверки есть сценарий:
```bash
./tests/run_examples.sh
```
Он собирает примеры и конвертирует `test-data/blp` в PNG рядом (использует `decode_dir`).

## Интеграция с CMake

Ниже минимальный пример для локальной сборки из артефактов в `dist/` (подберите библиотеку по платформе):

```cmake
cmake_minimum_required(VERSION 3.15)
project(sample C)
add_executable(sample main.c)

target_include_directories(sample PRIVATE ${CMAKE_SOURCE_DIR}/dist)
target_link_directories(sample PRIVATE ${CMAKE_SOURCE_DIR}/dist)

if(APPLE)
  target_link_libraries(sample PRIVATE blp-macos dl)
elseif(WIN32)
  target_link_libraries(sample PRIVATE blp-windows)
else()
  target_link_libraries(sample PRIVATE blp-linux dl pthread)
endif()
```

Примечание: имена библиотек без префикса `lib` и расширения (`.a/.dylib/.dll/.so`) — это принятая форма для `target_link_libraries`.

## Заметки по MSVC (Windows)

В дистрибутиве поставляются артефакты, собранные под `x86_64-pc-windows-gnu` (MinGW): `libblp-windows.a` и `blp-windows.dll`. Они подходят для сборки с MinGW. Для MSVC формат импортной библиотеки отличается (нужно `.lib`). Варианты:
- Использовать MinGW для линковки;
- Собрать библиотеку под `x86_64-pc-windows-msvc` из исходников (добавив соответствующую цель и правила);
- Сгенерировать импортную библиотеку `.lib` для MSVC из DLL сторонними инструментами (не рекомендуется, может быть нестабильно).

## ABI и управление памятью

- API использует C ABI (`extern "C"`); исключения не прокидываются через границу FFI.
- Все буферы, выделенные библиотекой (например, `BlpImage.data`), необходимо освобождать через `blp_free_image`.
- Структуры имеют фиксированную компоновку; выравнивание стандартное для C.

## Поддерживаемые платформы

- macOS: универсальные библиотеки (arm64 + x86_64)
- Windows (GNU toolchain): x86_64
- Linux (musl): x86_64 (статическая библиотека)

## Диагностика и известные предупреждения

- Cargo может выводить: `warning: dropping unsupported crate type 'cdylib' for target 'x86_64-unknown-linux-musl'` — это нормально, динамическая библиотека для musl не собирается.
- Сообщения `strip` об уже «очищенных» объектах подавлены в скрипте `build-settings.sh` (функция `strip_safe`).

## Версионирование

- Версия библиотеки доступна через `blp_get_version()` как строка.
- Внутренне библиотека использует крейт `blp` (см. версию в `Cargo.toml`).

## C API (основное)

Структуры:
- `BlpImage` — данные изображения (width, height, data (RGBA), data_len)
- `BlpResult` — коды результатов (`BLP_SUCCESS`, `BLP_INVALID_INPUT`, `BLP_PARSE_ERROR`, `BLP_MEMORY_ERROR`, `BLP_UNKNOWN_ERROR`)

Базовые функции:
- `BlpResult blp_load_from_buffer(const uint8_t* data, uint32_t len, BlpImage* out)` — загрузить из памяти;
- `BlpResult blp_load_from_file(const char* path, BlpImage* out)` — загрузить с диска;
- `void blp_free_image(BlpImage* img)` — освободить память, выделенную библиотекой;
- `const char* blp_get_version(void)` — версия библиотеки;
- `int blp_is_valid(const uint8_t* data, uint32_t len)` — валидирует .blp.

Кодирование:
- `BlpResult blp_encode_file_to_blp(const char* input_image_path, const char* output_blp_path, uint8_t quality, uint32_t mip_count)` — кодирование файла с количеством мипов;
- `BlpResult blp_encode_file_to_blp_with_flags(const char* input_image_path, const char* output_blp_path, uint8_t quality, const uint8_t* mip_visible, uint32_t mip_visible_len)` — кодирование файла с флагами видимости мипов (0/1);
- `BlpResult blp_encode_bytes_to_blp(const uint8_t* image_bytes, uint32_t image_len, const char* output_blp_path, uint8_t quality, uint32_t mip_count)` — кодирование из памяти;
- `BlpResult blp_encode_bytes_to_blp_with_flags(const uint8_t* image_bytes, uint32_t image_len, const char* output_blp_path, uint8_t quality, const uint8_t* mip_visible, uint32_t mip_visible_len)` — из памяти с флагами.

Декодирование/извлечение конкретного мипа:
- `BlpResult blp_decode_mip_to_png_from_file(const char* blp_path, uint32_t mip_index, const char* output_png_path)` — декодирует выбранный мип в PNG;
- `BlpResult blp_decode_mip_to_png_from_buffer(const uint8_t* blp_data, uint32_t blp_len, uint32_t mip_index, const char* output_png_path)` — то же из памяти;
- `BlpResult blp_extract_mip_to_jpg_from_file(const char* blp_path, uint32_t mip_index, const char* output_jpg_path)` — для JPEG-BLP извлекает «сырой» JPEG;
- `BlpResult blp_extract_mip_to_jpg_from_buffer(const uint8_t* blp_data, uint32_t blp_len, uint32_t mip_index, const char* output_jpg_path)` — то же из памяти.

## Управление памятью

- Все буферы, выделенные библиотекой (например, `BlpImage.data`), необходимо освобождать через `blp_free_image`.
- Строки и пути, передаваемые из пользовательского кода, должны быть нуль-терминированными (`\0`).

## Сборка и тесты

- Rust-библиотека:
  ```bash
  cargo build --release
  ```
- Дистрибутивные артефакты:
  ```bash
  ./build-only.sh
  ```
- Примеры:
  ```bash
  ./build-examples.sh
  ```
- Автотест сценариями:
  ```bash
  ./tests/run_examples.sh
  ```

Скрипт сборки подавляет шумные предупреждения `strip` об уже «очищенных» объектах. Предупреждение Cargo по `cdylib` для musl — допустимо.

## Лицензия

См. файл [LICENSE](LICENSE).

## Ссылки

- Английская документация: [README.md](README.md)
- Crate `blp` на crates.io: https://crates.io/crates/blp
