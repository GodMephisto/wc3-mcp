---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/std_module_compat.h"
ref: HEAD
---

#pragma once

#include <algorithm>
#include <array>
#include <atomic>
#include <bit>
#include <bitset>
#include <cassert>
#include <charconv>
#include <chrono>
#include <cmath>
#include <concepts>
#include <condition_variable>
#include <cstddef>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <cctype>
#include <cstring>
#include <deque>
#include <exception>
#include <expected>
#include <filesystem>
#include <format>
#include <fstream>
#include <functional>
#include <future>
#include <initializer_list>
#include <ios>
#include <iostream>
#include <limits>
#include <map>
#include <memory>
#include <mutex>
#include <new>
#include <numbers>
#include <numeric>
#include <optional>
#include <print>
#include <queue>
#include <ranges>
#include <regex>
#include <set>
#include <span>
#include <sstream>
#include <stack>
#include <stdexcept>
#include <string>
#include <string_view>
#include <thread>
#include <tuple>
#include <typeindex>
#include <type_traits>
#include <typeinfo>
#include <unordered_map>
#include <unordered_set>
#include <utility>
#include <variant>
#include <vector>

#if defined(__clang__) && defined(__cpp_lib_three_way_comparison)
// Qt's C++20 comparison helper defines an inline unnamed function object that
// Clang currently imports as duplicate module-owned definitions when several
// Qt-heavy project modules are imported by one translation unit. The language
// feature remains available; this only hides the library feature-test macro
// from Qt headers included after the shared standard header bundle.
#undef __cpp_lib_three_way_comparison
#endif
