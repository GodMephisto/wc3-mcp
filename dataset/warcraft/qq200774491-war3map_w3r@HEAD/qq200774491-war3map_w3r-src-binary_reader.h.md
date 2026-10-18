---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "src/binary_reader.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <cstring>
#include <stdexcept>
#include <string>
#include <vector>

namespace w3r {

using u8 = uint8_t;
using u16 = uint16_t;
using u32 = uint32_t;
using i32 = int32_t;
using f32 = float;

class BinaryReader {
public:
  BinaryReader() = default;
  explicit BinaryReader(const std::vector<u8> &data);
  explicit BinaryReader(std::vector<u8> &&data);

  BinaryReader(const BinaryReader &) = delete;
  BinaryReader &operator=(const BinaryReader &) = delete;
  BinaryReader(BinaryReader &&) = default;
  BinaryReader &operator=(BinaryReader &&) = default;

  size_t position() const { return position_; }
  size_t size() const { return data_.size(); }
  size_t remaining() const { return data_.size() - position_; }
  bool eof() const { return position_ >= data_.size(); }

  void seek(size_t pos);
  void skip(size_t bytes);

  i32 readI32();
  u32 readU32();
  u8 readU8();
  f32 readF32();

  std::string readString(size_t length);
  std::string readNullTerminatedString();

  void readBytes(u8 *buffer, size_t count);

private:
  void checkBounds(size_t bytes) const;

  template <typename T> T read() {
    static_assert(std::is_trivially_copyable_v<T>,
                  "Type must be trivially copyable");
    checkBounds(sizeof(T));
    T value;
    std::memcpy(&value, data_.data() + position_, sizeof(T));
    position_ += sizeof(T);
    return value;
  }

  std::vector<u8> data_;
  size_t position_ = 0;
};

std::vector<u8> LoadBinaryFile(const std::string &path);

} // namespace w3r
