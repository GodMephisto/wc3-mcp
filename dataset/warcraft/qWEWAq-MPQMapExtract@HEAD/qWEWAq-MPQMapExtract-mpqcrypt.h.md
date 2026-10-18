---
type: vendor-doc
source_repo: "https://github.com/qWEWAq/MPQMapExtract"
source_path: "mpqcrypt.h"
ref: HEAD
---

#ifndef MPQ_MPQCRYPT_H
#define MPQ_MPQCRYPT_H

#include "mpqtypes.h"
#include <functional>

void EncryptData(void *lpbyBuffer, uint32_t dwLength, uint32_t dwKey);
void DecryptData(void *lpbyBuffer, uint32_t dwLength, uint32_t dwKey);
uint32_t GetKey(const void* data, uint32_t flen, uint32_t clen, uint32_t sectorsize);

#define MPQ_HASH_TABLE_OFFSET	0
#define MPQ_HASH_NAME_A	1
#define MPQ_HASH_NAME_B	2
#define MPQ_HASH_FILE_KEY	3

uint32_t HashString(const char *lpszString, uint32_t dwHashType);
uint32_t GetFileDecryptKey(const void *buffer, uint32_t bufferSize, uint32_t expectedFirstDword,
                           const std::function<bool(const void *)> &validator);

#endif //MPQ_MPQCRYPT_H
