---
type: vendor-doc
source_repo: "https://github.com/qWEWAq/MPQMapExtract"
source_path: "mpqread.cpp"
ref: HEAD
---

#include <stdexcept>
#include "mpqread.h"
#include "mpqtypes.h"
#include "mpqcrypt.h"
#include "scenariochk.h"
#include "cmpdcmp.h"
#include <vector>
#include <set>
#include <sstream>
#include <cstdint>
#include <fstream>

using namespace std;
class MpqReadImpl;
class MpqReadImpl {
public:
    explicit MpqReadImpl(const std::string &mpqName);

    ~MpqReadImpl();

    int getHashEntryCount() const;

    const HashTableEntry *getHashEntry(int index) const;
    const HashTableEntry *getHashEntry(const std::string &fname) const;
    const BlockTableEntry *getBlockEntry(int index) const;
    std::string getDecryptedBlockContent(const HashTableEntry *hte, const BlockTableEntry *blockEntry) const;
    
private:
    uint32_t getKnownFilenameKey(const HashTableEntry *hashEntry, const BlockTableEntry *blockEntry) const;

    MPQHeader header;
    mutable std::set<std::string> knownFileNames;
    std::vector<HashTableEntry> hashTable;
    std::vector<BlockTableEntry> blockTable;
    size_t startBlockOffset;
    size_t realBlockCount;
    mutable std::ifstream is;
};

template <typename T>
void readTable(std::istream &is, size_t tableOffset, size_t entryCount, const char* tableKeyString, std::vector<T> &output)
{
    // Read data
    std::vector<char> tableData;
    size_t tableSize = entryCount * sizeof(T);
    tableData.resize(tableSize);
    is.seekg(tableOffset, std::ios_base::beg);
    is.read(tableData.data(), tableSize);

    // Decrypt data
    const uint32_t hashTableKey = HashString(tableKeyString, MPQ_HASH_FILE_KEY);
    DecryptData(tableData.data(), tableSize, hashTableKey);

    for (size_t i = 0; i < tableSize >> 4; i++){
        const auto &TableEntry = *(reinterpret_cast<T*>(tableData.data()) + i);
        output.push_back(TableEntry);
    }
}

#include <iostream>
bool is_freezed;
bool isFreezed() { return is_freezed; }
void check_freezed(std::istream &is, size_t signOffset){
    // "freeze06 protect"
    is.seekg(signOffset);
    std::vector<char> dataA(6);
    is.read(dataA.data(), 6);
    std::string strDataA(dataA.begin(), dataA.end());

    is.seekg(signOffset+9);
    std::vector<char> dataB(7);
    is.read(dataB.data(), 7);
    std::string strDataB(dataB.begin(), dataB.end());

    bool equal = (strDataA.find("freeze") != std::string::npos) && (strDataB.find("protect") != std::string::npos);
    if (equal) {
        is_freezed = true;
        is.seekg(signOffset + 6);
        std::vector<char> Version(2);
        is.read(Version.data(), 2);

        std::cout << "                          *                                         *\n";
        std::cout << "        *                                        *\n";
        std::cout << "                [[freeze Protection Detected :: freeze v" << Version[0] << Version[1] << "]]\n";
        std::cout << "                                  *                       *\n";
        std::cout << "                *                                                          *\n";
    }
    else {
        is_freezed = false;
    }
}

using HashTable = std::vector<HashTableEntry>;
using BlockTable = std::vector<BlockTableEntry>;
using BlockDataTable = std::vector<std::string>;
std::vector<Section> findScenarioChk(MpqReadPtr mr) {
    // Prepare hashTable
    HashTable hashTable;
    auto hashEntryCount = mr->getHashEntryCount();
    for (int i = 0; i < hashEntryCount; i++) {
        auto hashEntry = mr->getHashEntry(i);
        hashTable.push_back(*hashEntry);
    }

    // get block and blockdata
    BlockTable blockTable;
    BlockDataTable blockDataTable;
    size_t index = 0;
    for (auto& hashEntry : hashTable) {
        if (hashEntry.blockIndex >= 0xFFFFFFFE) continue;
        auto blockEntry = mr->getBlockEntry(hashEntry.blockIndex);
        std::string blockData = mr->getBlockContent(&hashEntry);
        if (blockData.empty()) {
            hashEntry.hashA = 0xFFFFFFFF;
            hashEntry.hashB = 0xFFFFFFFF;
            hashEntry.blockIndex = 0xFFFFFFFF;
            continue;
        }
        index++;
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

    // find scenariochk
    BlockTableEntry sceneChkBlock;
    std::string sceneChkBlockData;
    for (int i = 0; i < hashTable.size(); i++) {
        auto& entry = hashTable[i];
        if (entry.hashA == 0xB701656E && entry.hashB == 0xFCFB1EED) {
            auto chkBlockIndex = entry.blockIndex;
            sceneChkBlock = blockTable[chkBlockIndex];
            sceneChkBlockData.resize(blockDataTable[chkBlockIndex].size());
            sceneChkBlockData = blockDataTable[chkBlockIndex];
            break;
        }
    }

    if (sceneChkBlock.fileFlag & BLOCK_IMPLODED) throw std::runtime_error("Cannot decompress imploded block");
    std::string rawchk =
        (sceneChkBlock.fileFlag & BLOCK_COMPRESSED) ?
        decompressBlock(sceneChkBlock.fileSize, sceneChkBlockData) :
        sceneChkBlockData;
    return ScenarioChk(rawchk, true).sections;
}

MpqReadImpl::MpqReadImpl(const std::string &mpqName){
    try{
        is.exceptions(std::ifstream::failbit | std::ifstream::badbit);
        is.open(mpqName, std::ios_base::in | std::ios_base::binary);
        is.read(reinterpret_cast<char *>(&header), sizeof(header));
        if (header.sectorSizeShift != 3) throw std::runtime_error("        Invalid sectorSizeShift(maybe encrypted with other protector)");
        check_freezed(is, header.blockTableEntryCount*sizeof(BlockTableEntry)-32);

        readTable<HashTableEntry>(is, header.hashTableOffset, header.hashTableEntryCount, "(hash table)", hashTable);
        if(is_freezed) readTable<BlockTableEntry>(is, 0, header.blockTableEntryCount-2, "(block table)", blockTable);
        else readTable<BlockTableEntry>(is, header.blockTableOffset, header.blockTableEntryCount, "(block table)", blockTable);

        startBlockOffset = (header.hashTableOffset+header.hashTableEntryCount*sizeof(HashTableEntry)+0xF)&~0xF;
        realBlockCount = ((header.blockTableEntryCount-2)*sizeof(BlockTableEntry)-startBlockOffset) / sizeof(BlockTableEntry);
    }
    catch (std::ifstream::failure e){
        throw std::runtime_error(std::string("Error reading file : ") + e.what());
    }
}

MpqReadImpl::~MpqReadImpl(){
    is.close();
}

int MpqReadImpl::getHashEntryCount() const{
    return hashTable.size();
}

const HashTableEntry *MpqReadImpl::getHashEntry(int index) const{
    return &hashTable[index];
}

const HashTableEntry *MpqReadImpl::getHashEntry(const std::string &_fname) const{
    auto fname = _fname.c_str();
    auto hashA = HashString(fname, MPQ_HASH_NAME_A);
    auto hashB = HashString(fname, MPQ_HASH_NAME_B);
    auto hashKey = HashString(fname, MPQ_HASH_TABLE_OFFSET);
    auto initialFindIndex = hashKey & (hashTable.size() - 1);
    auto index = initialFindIndex;
    do{
        auto hashTableEntry = &hashTable[index];
        if (hashTableEntry->blockIndex == 0xFFFFFFFF)
            return nullptr;
        else if (hashTableEntry->hashA == hashA && hashTableEntry->hashB == hashB){
            knownFileNames.insert(_fname);
            return hashTableEntry;
        }
        index = (index + 1) & (hashTable.size() - 1);
    } while (index != initialFindIndex);
    return nullptr;
}

const BlockTableEntry *MpqReadImpl::getBlockEntry(int index) const{
    return &blockTable[index];
}

uint32_t MpqReadImpl::getKnownFilenameKey(const HashTableEntry *hashEntry, const BlockTableEntry *blockEntry) const{
    uint32_t fileKey = 0xFFFFFFFF;
    for (const auto &s : knownFileNames){
        auto expectedHashA = HashString(s.c_str(), MPQ_HASH_NAME_A);
        auto expectedHashB = HashString(s.c_str(), MPQ_HASH_NAME_B);
        if (expectedHashA == hashEntry->hashA && expectedHashB == hashEntry->hashB){
            fileKey = HashString(s.c_str(), MPQ_HASH_FILE_KEY);
            if (blockEntry->fileFlag & 0x00020000){ // File key adjusted
                fileKey = (fileKey + blockEntry->blockOffset) ^ blockEntry->fileSize;
            }
            break;
        }
    }
    return fileKey;
}

std::string MpqReadImpl::getDecryptedBlockContent(const HashTableEntry *hashEntry, const BlockTableEntry *blockEntry) const{
    bool compressed = blockEntry->fileFlag & BLOCK_COMPRESSED;
    bool encrypted = blockEntry->fileFlag & BLOCK_ENCRYPTED;
    bool imploded = blockEntry->fileFlag & BLOCK_IMPLODED;

    const size_t blockStartIndex = startBlockOffset / sizeof(BlockTableEntry);
    if((hashEntry->blockIndex < blockStartIndex || hashEntry->blockIndex >= (header.blockTableEntryCount-2)) && is_freezed){
        return std::string("");
    }

    // Read entire block
    is.seekg(blockEntry->blockOffset, std::ios_base::beg);
    std::vector<char> buf(blockEntry->blockSize);
    is.read(buf.data(), blockEntry->blockSize);

    // Decompress as needed
    if (encrypted){
        const size_t sectorSize = 512u << header.sectorSizeShift;
        size_t sectorNum = (blockEntry->fileSize + sectorSize - 1) / sectorSize;
        uint32_t fileKey = getKnownFilenameKey(hashEntry, blockEntry);

        if (compressed || imploded){
            // Search file key with sectorOffsetTable structure.
            // if fileKey not known
            if (fileKey == 0xFFFFFFFF){
                auto encryptedOffsetTable = reinterpret_cast<const uint32_t *>(buf.data());
                const uint32_t offsetTableLength = 4 * (sectorNum + 1);

                // Get decryption key
                fileKey = GetFileDecryptKey(
                    encryptedOffsetTable,
                    offsetTableLength,
                    offsetTableLength,
                    [&](const void *_decrypted){
                        const auto decryptedOffsetTable = static_cast<const uint32_t *>(_decrypted);
                        // Last table
                        if (decryptedOffsetTable[0] != offsetTableLength)
                            return false;
                        if (decryptedOffsetTable[sectorNum] != blockEntry->blockSize)
                            return false;
                        bool valid = true;
                        for (size_t i = 0; i < sectorNum; i++){
                            if (decryptedOffsetTable[i] > decryptedOffsetTable[i + 1]){
                                valid = false;
                                break;
                            }
                        }
                        return valid;
                    });
                if (fileKey == 0xFFFFFFFF)
                    throw std::runtime_error("Key-extraction from encrypted block failed!");
                else{
                    // sectorOffsetTable we found the encryption key with is encrypted with (fileKey - 1).
                    fileKey++;
                }
            }
            // Decrypt sectorOffsetTable
            DecryptData(buf.data(), 4 * (sectorNum + 1), fileKey - 1);
            auto sectorOffsetTable = reinterpret_cast<uint32_t *>(buf.data());

            // Decrypt file data
            for (size_t sectorIndex = 0; sectorIndex < sectorNum; sectorIndex++){
                size_t thisSectorOffset = sectorOffsetTable[sectorIndex];
                size_t thisSectorSize = sectorOffsetTable[sectorIndex + 1] - sectorOffsetTable[sectorIndex];
                DecryptData(buf.data() + thisSectorOffset, thisSectorSize, fileKey + sectorIndex);
            }
        }
        else{
            // Non-compressed data don't have SectorOffsetTable
            // if it is a file(OGGs, RIFF, ...), we assume it must be OGGs file
            if (fileKey == 0xFFFFFFFF){
                do{
                    if (buf.size() > 4){
                        fileKey = GetFileDecryptKey(
                            buf.data(),
                            4,
                            0x5367674f, // OggS
                            [](const void *){
                                // TODO: add proper .ogg file validator
                                return true;
                            });
                        if (fileKey != 0xFFFFFFFF)
                            break;
                    }
                } while (false);

                if (fileKey == 0xFFFFFFFF){
                    printf("Cannot get key of non-compressed encrypted block\n");
                    printf(" - HASHA %08X, HASHB %08X, BLOCK %08X\n", hashEntry->hashA, hashEntry->hashB,
                           hashEntry->blockIndex);
                    throw std::runtime_error("Cannot get key of non-compressed encrypted block");
                }
            }

            // Decrypt file data
            for (size_t sectorIndex = 0; sectorIndex < sectorNum; sectorIndex++){
                size_t thisSectorOffset = sectorSize * sectorIndex;
                size_t thisSectorSize = min(sectorSize, blockEntry->fileSize - thisSectorOffset);
                DecryptData(buf.data() + thisSectorOffset, thisSectorSize, fileKey + sectorIndex);
            }
        }
    }

    // Return block data, compressed as-is
    return {buf.begin(), buf.end()};
}

/////////////////////////////

MpqRead::MpqRead(const std::string &mpqName) : pimpl(new MpqReadImpl(mpqName)) {}

MpqRead::~MpqRead() { delete pimpl; }

int MpqRead::getHashEntryCount() const { return pimpl->getHashEntryCount(); }

const HashTableEntry *MpqRead::getHashEntry(int index) const { return pimpl->getHashEntry(index); }
const HashTableEntry *MpqRead::getHashEntry(const std::string &fname) const { return pimpl->getHashEntry(fname); }
const BlockTableEntry *MpqRead::getBlockEntry(int index) const { return pimpl->getBlockEntry(index); }


std::string MpqRead::getBlockContent(const HashTableEntry *hashEntry) const {
    auto blockEntry = pimpl->getBlockEntry(hashEntry->blockIndex);
    return pimpl->getDecryptedBlockContent(hashEntry, blockEntry);
}

/////////////////////////////

std::vector<Section> readCryptedMPQ(const std::string &mpqName) {
    auto mr = std::make_shared<MpqRead>(mpqName);
    std::vector<Section> sectionDatas = findScenarioChk(mr);

    if (is_freezed == false) {
        throw std::runtime_error("====================================================================\n                   File Definitely Not FREEZED...;;\n====================================================================\n");
    }
    return sectionDatas;
}

MpqReadPtr readMPQ(const std::string& mpqName) {
    return std::make_shared<MpqRead>(mpqName);
}

