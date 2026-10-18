---
type: vendor-doc
source_repo: "https://github.com/l-Fingon-l/w3x-32bit-MapHash"
source_path: "Map_32bitHash.cpp"
ref: HEAD
---

#include <cstdio>
#include "StormLib.h"
#include <vector>
#include <fstream>
#include <sstream>
#include <iostream>

using namespace std;
const uint32_t chunk = 0x400;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// minor helper functions
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

uint32_t ROTL_u32(uint32_t x, uint32_t n)
{
	int size = 32;
	return ((x) << (n)) | ((x) >> (size - (n)));
}

uint32_t ROTR_u32(uint32_t x, uint32_t n)
{
	int size = 32;
	return ((x) >> (n)) | ((x) << (size - (n)));
}

//////////////////////////////////////////////////////////////////////////////////////////////////////
// major helper functions
//////////////////////////////////////////////////////////////////////////////////////////////////////

uint32_t XORRotateLeft(uint8_t* data, uint32_t length, uint32_t Val)
{
	// a big thank you to Fingon for figuring this out

	uint32_t i = 0, ceiling = length / 4;
	uint32_t* data_ = (uint32_t*)data;

	if (ceiling)
	{
		while (i < ceiling)
		{
			Val = ROTL_u32(Val ^ data_[i], 3);
			i++;
		}
	}

	i = i * 4;
	while (i < length)
	{
		Val = ROTL_u32(Val ^ data[i], 3);
		i++;
	}

	return Val;
}

uint32_t chunked_checksum(uint8_t* file, int32_t size, uint32_t checksum)
{
	size -= chunk;

	while (size > -1)
	{
		checksum = ROTL_u32(XORRotateLeft(file, chunk, checksum), 3);
		file += chunk;
		size -= chunk;
	}

	return checksum;
}

//////////////////////////////////////////////////////////////////////////////////////////////////////
// the main one
//////////////////////////////////////////////////////////////////////////////////////////////////////

uint32_t XORchecksum(string MapMPQFileName)
{
	// load the map MPQ

	HANDLE MapMPQ;
	bool MapMPQReady = false;

#ifdef WIN32
	const wstring MapMPQFileNameW = wstring(begin(MapMPQFileName), end(MapMPQFileName));

	if (SFileOpenArchive(MapMPQFileNameW.c_str(), 0, MPQ_OPEN_FORCE_MPQ_V1, &MapMPQ))
#else
	if (SFileOpenArchive(MapMPQFileName.c_str(), 0, MPQ_OPEN_FORCE_MPQ_V1, &MapMPQ))
#endif
	{
		cout << "loading MPQ file [" + MapMPQFileName + "]\n";
		MapMPQReady = true;
	}
	else
		cout << "warning - unable to load MPQ file [" + MapMPQFileName + "]\n";

	if (MapMPQReady)
	{
		// a big thank you to Fingon for figuring the map_crc algorithm out

		uint32_t Val = 0;

		if (MapMPQReady)
		{
			vector<string> FileList;
			FileList.push_back("war3map.j");                  // full file // no shift
			FileList.push_back("scripts\\war3map.j");         // +3 shift after each 1024 byte chunk and no file shift
			FileList.push_back("war3map.lua");                // full file // no shift
			FileList.push_back("war3map.w3e");                // full 1024 byte chunks only 
			FileList.push_back("war3map.wpm");                // full 1024 byte chunks only
			FileList.push_back("war3map.doo");                // full 1024 byte chunks only
			FileList.push_back("war3map.w3u");                // full 1024 byte chunks only
			FileList.push_back("war3map.w3b");                // full 1024 byte chunks only
			FileList.push_back("war3map.w3d");                // full 1024 byte chunks only
			FileList.push_back("war3map.w3a");                // full 1024 byte chunks only
			FileList.push_back("war3map.w3q");                // full 1024 byte chunks only
			bool FoundScript = false;

			for (vector<string> ::iterator i = FileList.begin(); i != FileList.end(); i++)
			{
				if (FoundScript && (*i == "scripts\\war3map.j" || *i == "war3map.lua"))
					continue;

				HANDLE SubFile;

				if (SFileOpenFileEx(MapMPQ, (*i).c_str(), 0, &SubFile))
				{
					uint32_t FileLength = SFileGetFileSize(SubFile, NULL);

					if (FileLength > 0 && FileLength != 0xFFFFFFFF)
					{
						BYTE* SubFileData = new BYTE[FileLength];
						DWORD BytesRead = 0;

						if (SFileReadFile(SubFile, SubFileData, FileLength, &BytesRead, 0))
						{
							if (*i == "war3map.j" || *i == "scripts\\war3map.j" || *i == "war3map.lua")
							{
								FoundScript = true;
								Val = XORRotateLeft((BYTE*)SubFileData, BytesRead, Val);
							}
							else Val = chunked_checksum((BYTE*)SubFileData, BytesRead, Val);
						}

						delete[] SubFileData;
					}

					SFileCloseFile(SubFile);
				}
			}

			if (!FoundScript)
				cout << "couldn't find war3map.j, war3map.lua or scripts\\war3map.j in MPQ file, calculated map_crc is probably wrong\n";

			return Val;
		}
		else
			cout << "unable to calculate map_crc - map MPQ file not loaded\n";
	}
	else
		cout << "no map data available, using config file for map_crc\n";

	return 0xFFFFFFFF; // be careful since it's a valid checksum still
}

int main()
{
//	string map = "D:\\w3c_1v1_concealedhill_anon.w3x";
	string map = "D:\\map.w3x";

	uint32_t hash = XORchecksum(map);

	cout << "calculated map_crc = " << hash << " or (hex) " << hex << hash << endl;
	ofstream write("log_hash.txt", ofstream::app);
	write << "calculated map_crc = " << hash << " or (hex) " << hex << hash << endl;
	write.close();

	std::cin.get();
}
