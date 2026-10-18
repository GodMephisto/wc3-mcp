---
type: vendor-doc
source_repo: "https://github.com/qWEWAq/MPQMapExtract"
source_path: "mpqcrypt.cpp"
ref: HEAD
---

#include <cctype>
#include <cassert>
#include <cstring>
#include <vector>
#include <functional>
#include <cstdint>
#include "mpqcrypt.h"


uint32_t dwCryptTable[0x500];

// The encryption and hashing functions use a number table in their procedures. This table must be initialized before the functions are called the first time.
void InitializeCryptTable() {
    static bool inited = false;
    if (inited) return;
    inited = true;

    uint32_t seed = 0x00100001;
    uint32_t index1 = 0;
    uint32_t index2 = 0;
    int i;

    for (index1 = 0; index1 < 0x100; index1++) {
        for (index2 = index1, i = 0; i < 5; i++, index2 += 0x100) {
            uint32_t temp1, temp2;

            seed = (seed * 125 + 3) % 0x2AAAAB;
            temp1 = (seed & 0xFFFF) << 0x10;

            seed = (seed * 125 + 3) % 0x2AAAAB;
            temp2 = (seed & 0xFFFF);

            dwCryptTable[index2] = (temp1 | temp2);
        }
    }
}

void EncryptData(void *lpbyBuffer, uint32_t dwLength, uint32_t dwKey) {
    InitializeCryptTable();
    assert(lpbyBuffer);

    uint32_t *lpdwBuffer = reinterpret_cast<uint32_t*>(lpbyBuffer);
    uint32_t seed = 0xEEEEEEEE;
    uint32_t ch;

    dwLength /= sizeof(uint32_t);

    while (dwLength-- > 0) {
        seed += dwCryptTable[0x400 + (dwKey & 0xFF)];
        ch = *lpdwBuffer ^ (dwKey + seed);

        dwKey = ((~dwKey << 0x15) + 0x11111111) | (dwKey >> 0x0B);
        seed = *lpdwBuffer + seed + (seed << 5) + 3;

        *lpdwBuffer++ = ch;
    }
}

void DecryptData(void *lpbyBuffer, uint32_t dwLength, uint32_t dwKey) {
    InitializeCryptTable();
    assert(lpbyBuffer);

    uint32_t *lpdwBuffer = reinterpret_cast<uint32_t*>(lpbyBuffer);
    uint32_t seed = 0xEEEEEEEE;
    uint32_t ch;

    dwLength /= sizeof(uint32_t);
    while (dwLength-- > 0) {
        seed += dwCryptTable[0x400 + (dwKey & 0xFF)];
        ch = *lpdwBuffer ^ (dwKey + seed);

        dwKey = ((~dwKey << 0x15) + 0x11111111) | (dwKey >> 0x0B);
        seed = ch + seed + (seed << 5) + 3;

        *lpdwBuffer++ = ch;
    }
}
// Different types of hashes to make with HashString

// Based on code from StormLib.
uint32_t HashString(const char *lpszString, uint32_t dwHashType) {
    InitializeCryptTable();
    assert(lpszString);
    assert(dwHashType <= MPQ_HASH_FILE_KEY);

    if (dwHashType == MPQ_HASH_FILE_KEY)
        while (strchr(lpszString, '\\') != NULL) lpszString = strchr(lpszString, '\\') + 1;


    uint32_t seed1 = 0x7FED7FED;
    uint32_t seed2 = 0xEEEEEEEE;
    uint32_t ch;

    while (*lpszString != 0) {
        ch = toupper(*lpszString++);

        seed1 = dwCryptTable[(dwHashType * 0x100) + ch] ^ (seed1 + seed2);
        seed2 = ch + seed1 + seed2 + (seed2 << 5) + 3;
    }
    return seed1;
}



//////////////////////////////////////

uint32_t GetFileDecryptKey(const void *buffer, uint32_t bufferSize, uint32_t expectedFirstDword,
                           const std::function<bool(const void *)> &validator) {
    if (bufferSize < 4) return 0xffffffff;
    uint8_t *decryptedBuffer = nullptr;

    // We know that decryptedOffsetTable[0] should be offsetTableLength.
    // Exploit storm encryption algorithm with that knowledge.
    for (uint32_t dwKeyLobyte = 0; dwKeyLobyte < 256; dwKeyLobyte++) {
        // seed += dwCryptTable[0x400 + (dwKey & 0xFF)];
        // ch = *lpdwBuffer ^ (dwKey + seed);

        // range of dwKey & 0xFF  :  00 ~ FF - Can be brute-forced
        // dwKey = (ch ^ *lpdwBuffer) - seed0
        // Check if this equation holds.
        const uint32_t seed = 0xEEEEEEEE + dwCryptTable[0x400 + dwKeyLobyte];
        const uint32_t dwKey = (*(uint32_t *) buffer ^ expectedFirstDword) - seed;
        if ((dwKey & 0xff) == dwKeyLobyte) {  // Viable candidate
            if (!decryptedBuffer) decryptedBuffer = new uint8_t[bufferSize];
            // Do full decryption and check if decrypted offset table is valid.
            memcpy(decryptedBuffer, buffer, bufferSize);
            DecryptData(decryptedBuffer, bufferSize, dwKey);

            if (validator(decryptedBuffer)) {
                delete[] decryptedBuffer;
                return dwKey;
            }
        }
    }


    // Cannot find viable candidate.
    delete[] decryptedBuffer;  // This works even if decryptedBuffer == nullptr
    return 0xFFFFFFFF;
}


uint32_t GetKey(const void* data, uint32_t flen, uint32_t clen, uint32_t sectorsize) {
    const uint32_t input = *(uint32_t*)data;
    uint32_t i;
    const uint32_t sectorn = (flen + (sectorsize - 1)) / sectorsize;
    uint32_t* offtb = new uint32_t[sectorn + 1];
    const uint32_t offtblen = 4 * (sectorn + 1);

    //clen might be wrong. asdf.

    if (offtblen > clen) {
        delete[] offtb;
        return -1;
    }
    uint32_t ret = -1;

    /* // Deprecated. clen (compressed length) can be modified
    for(i = 0 ; i < 256 ; i++) {
        //new seed = (0xEEEEEEEE + dwCryptTable[0x400 + (dwKey & 0xFF)]); dwkey & 0xff = i        .
        if(i == 0xE2) {
            printf("");
        }
        //after decrypting data with dwkey, this should be true. *(mpq_uint32*)data == offtblen.
        //ch = *lpdwBuffer ^ (dwKey + seed);
        const mpq_uint32 dwkps = input ^ offtblen;
        const mpq_uint32 seed = 0xEEEEEEEE + dwCryptTable[0x400 + i];
        const mpq_uint32 dwKey = dwkps - seed;
        if((dwKey & 0xff) == i) {
            //until this... We verify if this IS the key we wanted.
            memcpy(offtb, data, offtblen);
            DecryptData(offtb, offtblen, dwKey);
            if(offtb[sectorn] != clen) {
                continue;
            }
            else {
                mpq_uint32 j;
                for(j = 0 ; j < sectorn ; j++) {
                    if(offtb[j] > offtb[j + 1]) break;
                }
                if(j == sectorn) {
                    delete[] offtb;
                    return dwKey + 1;
                }
            }
        }
    }
    */

    //Standard method.
    std::vector<uint32_t> retv;
    for (i = 0; i < 256; i++) {
        //new seed = (0xEEEEEEEE + dwCryptTable[0x400 + (dwKey & 0xFF)]); dwkey & 0xff = i        .
        if (i == 0xE2) {
            printf("");
        }
        //after decrypting data with dwkey, this should be true. *(mpq_uint32*)data == offtblen.
        //ch = *lpdwBuffer ^ (dwKey + seed);
        const uint32_t dwkps = input ^ offtblen;
        const uint32_t seed = 0xEEEEEEEE + dwCryptTable[0x400 + i];
        const uint32_t dwKey = dwkps - seed;
        if ((dwKey & 0xff) == i) {
            //until this... We verify if this IS the key we wanted.
            memcpy(offtb, data, offtblen);
            DecryptData(offtb, offtblen, dwKey);

            //simple check. DELETE THIS IF YOU WANT TO SUPPORT MPQ_PROTECTED_MAP!
            if (0 && offtb[sectorn] != clen) {
                continue;
            }
            //AND INSERT THE FOLLOWINGS

            if (offtb[sectorn] > clen) {
                continue;
            }

            uint32_t j;
            for (j = 0; j < sectorn; j++) {
                if (offtb[j] > offtb[j + 1]) break;
            }
            if (j == sectorn) {
                retv.push_back(dwKey + 1);
            }
        }
    }

    if (retv.size() == 1) ret = retv[0];
    else if (retv.size() >= 2) {
        printf("[GetKey] Multiple(%d) possible keys. File may not be decrypted well.\n", retv.size()); //very rare case.
        ret = retv[0];
    }
    delete[] offtb;
    return ret;
}