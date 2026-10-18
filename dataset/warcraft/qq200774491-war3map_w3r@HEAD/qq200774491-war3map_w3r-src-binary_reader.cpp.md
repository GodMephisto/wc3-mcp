---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "src/binary_reader.cpp"
ref: HEAD
---

#include "binary_reader.h"

#include <fstream>
#include <iostream>

namespace w3r {

BinaryReader::BinaryReader(const std::vector<u8> &data)
    : data_(data), position_(0) {}

BinaryReader::BinaryReader(std::vector<u8> &&data)
    : data_(std::move(data)), position_(0) {}

void BinaryReader::checkBounds(size_t bytes) const {
  if (position_ + bytes > data_.size()) {
    throw std::runtime_error("BinaryReader: read past end of buffer");
  }
}

void BinaryReader::seek(size_t pos) {
  if (pos > data_.size()) {
    throw std::runtime_error("BinaryReader: seek position out of bounds");
  }
  position_ = pos;
}

void BinaryReader::skip(size_t bytes) {
  checkBounds(bytes);
  position_ += bytes;
}

i32 BinaryReader::readI32() { return read<i32>(); }
u32 BinaryReader::readU32() { return read<u32>(); }
u8 BinaryReader::readU8() { return read<u8>(); }
f32 BinaryReader::readF32() { return read<f32>(); }

std::string BinaryReader::readString(size_t length) {
  checkBounds(length);
  std::string result(reinterpret_cast<const char *>(data_.data() + position_),
                     length);
  position_ += length;
  return result;
}

std::string BinaryReader::readNullTerminatedString() {
  std::string result;
  while (position_ < data_.size()) {
    char c = static_cast<char>(data_[position_++]);
    if (c == '\0')
      break;
    result += c;
  }
  return result;
}

void BinaryReader::readBytes(u8 *buffer, size_t count) {
  checkBounds(count);
  std::memcpy(buffer, data_.data() + position_, count);
  position_ += count;
}

std::vector<u8> LoadBinaryFile(const std::string &path) {
  std::ifstream file(path, std::ios::binary | std::ios::ate);
  if (!file.is_open()) {
    throw std::runtime_error("Failed to open file: " + path);
  }

  auto file_size = file.tellg();
  file.seekg(0, std::ios::beg);

  std::vector<u8> data(static_cast<size_t>(file_size));
  if (!file.read(reinterpret_cast<char *>(data.data()), file_size)) {
    throw std::runtime_error("Failed to read file: " + path);
  }

  return data;
}

} // namespace w3r
