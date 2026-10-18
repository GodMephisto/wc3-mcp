---
type: vendor-doc
source_repo: "https://github.com/qWEWAq/MPQMapExtract"
source_path: "main.cpp"
ref: HEAD
---

﻿#include "mpqread.h"
#include "mpqwrite.h"
#include "mpqmake.h"
#include "scenariochk.h"
#include <fstream>
#include <cstring>
#include <iostream>
#include <cstdio>
#include <string>
#include <windows.h>
#include <iomanip> 
#include <cstdlib>
#include <Shlwapi.h>
#pragma comment(lib, "Shlwapi.lib")

float VERSION = 0.1;
std::string GetExecutableDirectory() {
	char path[MAX_PATH];
	GetModuleFileNameA(NULL, path, MAX_PATH);
	PathRemoveFileSpecA(path);
	return std::string(path);
}

int main(int argc, char** argv) {
    if(argc != 2 && argc != 3) return -1;

	std::string ifname = argv[1];
	std::string path = GetExecutableDirectory();
	std::string ofname = path + "\\result_map.scx";
	std::string sourcename_256x256	= path + "\\source_map\\original_map_256x256.scx";
	std::string sourcename_128x128	= path + "\\source_map\\original_map_128x128.scx";
	std::string sourcename_128x192	= path + "\\source_map\\original_map_128x192.scx";
	std::string sourcename_192x192	= path + "\\source_map\\original_map_192x192.scx";
	std::string sourcename_64x64	= path + "\\source_map\\original_map_64x64.scx";
	std::string sourcename_96x96	= path + "\\source_map\\original_map_96x96.scx";

	try {
		size_t tileX = 0;
		size_t tileY = 0;
		std::vector<Section> sectionDatas = readCryptedMPQ(ifname);
		for (auto& data : sectionDatas) {
			if (strncmp(data.name, "DIM ", 4) == 0) {
				tileX = data.data[3] * 0x100 + data.data[2];
				tileY = data.data[1] * 0x100 + data.data[0];
			}
		}
		std::cout << "-------------------------------------------------------------------------------\n";

		MpqReadPtr hMPQ;
		if (tileX == 256 && tileY == 256) {
			hMPQ = readMPQ(sourcename_256x256);
		}
		else if (tileX == 128 && tileY == 128) {
			hMPQ = readMPQ(sourcename_128x128);
		}
		else if (tileX == 128 && tileY == 192) {
			hMPQ = readMPQ(sourcename_128x192);
		}
		else if (tileX == 192 && tileY == 192) {
			hMPQ = readMPQ(sourcename_192x192);
		}
		else if (tileX == 64 && tileY == 64) {
			hMPQ = readMPQ(sourcename_64x64);
		}
		else if (tileX == 96 && tileY == 96) {
			hMPQ = readMPQ(sourcename_96x96);
		}
		else {
			throw std::runtime_error("                      File size not supported... (sry)");
		}
		std::string data = makeMPQ(hMPQ, sectionDatas);
		hMPQ = nullptr;

		std::ofstream os(ofname, std::ios_base::binary);

		os.write(data.data(), data.size());
		std::cout << "                MPQRig Ver " << VERSION << " , " << "Created At " << ".\\result_map.scx" << "\n";
		std::cout << "                    press any key to Exit :: Made By qWEWA\n";
		os.close();
		system("pause");
	}
    catch (std::runtime_error e) {
        puts(e.what());
		std::cout << "                   Error :: press any key to Exit\n";
		system("pause");
		return -2;
    }
    return 0;
}
