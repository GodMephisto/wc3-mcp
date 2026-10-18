---
type: vendor-doc
source_repo: "https://github.com/qWEWAq/MPQMapExtract"
source_path: "mpqmake.cpp"
ref: HEAD
---

#include <iostream>
#include <chrono>
#include <iomanip>
#include <sstream>
#include "scenariochk.h"
#include "mpqtypes.h"
#include "mpqread.h"
#include "mpqcrypt.h"
#include "cmpdcmp.h"
#include <vector>
#include <set>
#include <cstring>
#include <utility>
#include <stdexcept>
#include <random>
#include <set>

using HashTable = std::vector<HashTableEntry>;
using BlockTable = std::vector<BlockTableEntry>;
using BlockDataTable = std::vector<std::string>;


std::string makeMPQ(MpqReadPtr mr, std::vector<Section> sectionDatas) {
    // Prepare hashTable
    HashTable hashTable;
    auto hashEntryCount = mr->getHashEntryCount();
    for (int i = 0; i < hashEntryCount; i++) {
        auto hashEntry = mr->getHashEntry(i);
        hashTable.push_back(*hashEntry);
    }

    // get block and blockdata
    // new blockTable starts from 0(not realBlockOffset)
    BlockTable blockTable;
    BlockDataTable blockDataTable;
    size_t listhashIdx = -1;
    for (auto& hashEntry : hashTable) {
        if (hashEntry.blockIndex >= 0xFFFFFFFE) continue;
        auto blockEntry = mr->getBlockEntry(hashEntry.blockIndex);
        std::string blockData = mr->getBlockContent(&hashEntry);

        auto newBlockEntry = *blockEntry;
        if (newBlockEntry.fileFlag & BLOCK_COMPRESSED) {
            try {
                auto fdata = decompressBlock(newBlockEntry.fileSize, blockData);

                if (fdata.size() >= 12 &&
                    memcmp(fdata.data(), "RIFF", 4) == 0 &&
                    memcmp(fdata.data() + 8, "WAVE", 4) == 0) {
                    auto cmpdata = compressToBlock(fdata,
                        MAFA_COMPRESS_STANDARD,
                        MAFA_COMPRESS_WAVE);
                    if (cmpdata.size() < blockData.size()) blockData = cmpdata;
                    newBlockEntry.blockSize = blockData.size();
                }
            }
            catch (std::runtime_error e) {}
        }
        blockDataTable.push_back(blockData);
        newBlockEntry.fileFlag &= ~(BLOCK_ENCRYPTED | BLOCK_KEY_ADJUSTED);
        blockTable.push_back(newBlockEntry);
        hashEntry.blockIndex = blockTable.size() - 1;
    }

    size_t chkBlockIndex = 0;
    BlockTableEntry sceneChkBlock;
    std::string sceneChkBlockData;
    for (int i = 0; i < hashTable.size(); i++) {
        auto& entry = hashTable[i];
        if (entry.hashA == 0xB701656E && entry.hashB == 0xFCFB1EED) {
            chkBlockIndex = entry.blockIndex;
            sceneChkBlock = blockTable[chkBlockIndex];
            sceneChkBlockData.resize(blockDataTable[chkBlockIndex].size());
            sceneChkBlockData = blockDataTable[chkBlockIndex];
            break;
        }
    }

    {
        if (sceneChkBlock.fileFlag & BLOCK_IMPLODED) throw std::runtime_error("Cannot decompress imploded block");
        std::string rawchk =
            (sceneChkBlock.fileFlag & BLOCK_COMPRESSED) ?
            decompressBlock(sceneChkBlock.fileSize, sceneChkBlockData) :
            sceneChkBlockData;

        ScenarioChk schk = ScenarioChk(rawchk, false);
        schk.swapSections(sectionDatas);

        rawchk = schk.writeChunk();
        sceneChkBlockData = compressToBlock(
            rawchk,
            MAFA_COMPRESS_STANDARD,
            MAFA_COMPRESS_STANDARD
        );
        sceneChkBlock.fileSize = rawchk.size();
        sceneChkBlock.blockSize = sceneChkBlockData.size();
        sceneChkBlock.fileFlag |= BLOCK_COMPRESSED;
        blockTable[chkBlockIndex] = sceneChkBlock;
        blockDataTable[chkBlockIndex] = sceneChkBlockData;
    }

    // evaluate MPQ file size
    size_t newArchiveSize = sizeof(MPQHeader);
    uint32_t hashTableOffset, blockTableOffset;
    {
        // Rest of the block are resource blocks. Put them.
        for (size_t i = 0; i < blockDataTable.size(); i++) {
            blockTable[i].blockOffset = newArchiveSize;
            newArchiveSize += blockDataTable[i].size();
        }

        // hashTable size calculate
        hashTableOffset = newArchiveSize;
        newArchiveSize += hashTable.size() * 16;

        // blockTable size calculate
        blockTableOffset = newArchiveSize;
        newArchiveSize += blockTable.size() * 16;
    }


    // create NEW MPQ
    std::vector<char> archiveBuffer(newArchiveSize);
    size_t cursor = 32;

    // Write MPQ Header
    MPQHeader header;
    header.magic = 0x1A51504D;
    header.headerSize = 32;
    header.mpqSize = newArchiveSize;
    header.mpqVersion = 0;
    header.sectorSizeShift = 3;
    header.unused0 = 0;
    header.hashTableOffset = hashTableOffset;
    header.blockTableOffset = blockTableOffset;
    header.hashTableEntryCount = hashTable.size();
    header.blockTableEntryCount = blockTable.size();
    memcpy(archiveBuffer.data(), &header, sizeof(MPQHeader));

    // Write resource files
    for (size_t i = 0; i < blockDataTable.size(); i++) {
        const auto& blockData = blockDataTable[i];
        memcpy(archiveBuffer.data() + cursor, blockData.data(), blockData.size());
        cursor += blockData.size();
    }

    // Write hash data
    const uint32_t hashTableKey = HashString("(hash table)", MPQ_HASH_FILE_KEY);
    for (const auto& hashEntry : hashTable) {
        memcpy(archiveBuffer.data() + cursor, &hashEntry, sizeof(hashEntry));
        cursor += sizeof(hashEntry);
    }

    EncryptData(archiveBuffer.data() + cursor - 16 * hashTable.size(),
        16 * hashTable.size(),
        hashTableKey
    );

    // Write block table data
    const uint32_t blockTableKey = HashString("(block table)", MPQ_HASH_FILE_KEY);
    for (const auto& blockEntry : blockTable) {
        memcpy(archiveBuffer.data() + cursor, &blockEntry, sizeof(blockEntry));
        cursor += sizeof(blockEntry);
    }

    EncryptData(archiveBuffer.data() + cursor - 16 * blockTable.size(),
        16 * blockTable.size(),
        blockTableKey
    );

    // Done
    return std::string(archiveBuffer.begin(), archiveBuffer.end());
}

