---
type: vendor-doc
source_repo: "https://github.com/qWEWAq/MPQMapExtract"
source_path: "mpqread.h"
ref: HEAD
---

#ifndef MPQ_MPQREAD_H
#define MPQ_MPQREAD_H

#include <memory>
#include <string>
#include "scenariochk.h"
#include "mpqtypes.h"

class MpqReadImpl;

class MpqRead {
public:
    MpqRead(const std::string& mpqName);
    ~MpqRead();

    int getFileCount() const;
    int getHashEntryCount() const;
    int getBlockEntryCount() const;
    const HashTableEntry* getHashEntry(int index) const;
    const HashTableEntry* getHashEntry(const std::string& fname) const;
	const BlockTableEntry* getBlockEntry(int index) const;
	std::string getBlockContent(const HashTableEntry *hashEntry) const;

private:
    MpqReadImpl* pimpl;
};

using MpqReadPtr = std::shared_ptr<MpqRead>;
std::vector<Section> readCryptedMPQ(const std::string& mpqName);
std::vector<Section> findScenarioChk(MpqReadPtr mr);
MpqReadPtr readMPQ(const std::string& mpqName);
bool isFreezed();

#endif //MPQ_MPQREAD_H
