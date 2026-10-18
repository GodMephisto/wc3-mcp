---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "listfiles.c"
ref: HEAD
---

/*
    listfiles.cpp - StormLib MPQ archiving utility
    Copyright (C) 2010 - 2016  Pali Rohár <pali.rohar@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

#include <StormLib.h>

#include <sys/types.h>
#include <sys/stat.h>
#include <string.h>
#include <stdlib.h>

#if defined(WIN32) || defined(_MSC_VER)
#define strcasecmp _stricmp
#endif

#ifdef _MSC_VER

char * dirname(char *);

#else

#include <libgen.h>
#include <dirent.h>

#endif

#include "common.h"

void smpq_systemlistfiles(void * SArchive, const char * archive, unsigned int flags) {

#if defined(WIN32) || defined(_MSC_VER)

	char processPath[512];
	char * listfile;
	const char * LISTPATH;
	WIN32_FIND_DATA FindFileData;
	HANDLE hFind;

	GetModuleFileName(GetModuleHandle(NULL), processPath, sizeof(processPath));
	LISTPATH = dirname(processPath);

	hFind = FindFirstFile(LISTPATH, &FindFileData);

	if ( hFind == INVALID_HANDLE_VALUE )
		return;

	do {

		if ( FindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY )
			continue;

		listfile = (char *)malloc(strlen(LISTPATH) + strlen(FindFileData.cFileName) + 2);

		if ( ! listfile )
			continue;

		strcpy(listfile, LISTPATH);
		strcpy(listfile+strlen(LISTPATH) + 1, FindFileData.cFileName);
		listfile[strlen(LISTPATH)] = '\\';

		if ( flags & VERBOSE )
			printVerbose(archive, "Loading system listfile", listfile);

		SFileAddListFile((HANDLE)SArchive, listfile);

		free(listfile);

	} while ( FindNextFile(hFind, &FindFileData) );

	FindClose(hFind);

#else

#define LISTPATH "/usr/share/stormlib"

	struct dirent * ent;
	DIR * dir = opendir(LISTPATH);

	if ( ! dir )
		return;

	while ( ( ent = readdir(dir) ) ) {

		struct stat st;
		char listfile[1024];

		if ( strlen(LISTPATH)+strlen(ent->d_name)+2 > 1024 )
			continue;

		if ( strcasecmp(ent->d_name+strlen(ent->d_name)-4, ".txt") != 0 )
			continue;

		strcpy(listfile, LISTPATH);
		strcpy(listfile+strlen(LISTPATH)+1, ent->d_name);
		listfile[strlen(LISTPATH)] = '/';

		if ( stat(listfile, &st) == -1 )
			continue;

		if ( S_ISDIR(st.st_mode) )
			continue;

		if ( flags & VERBOSE )
			printVerbose(archive, "Loading system listfile", listfile);

		SFileAddListFile((HANDLE)SArchive, listfile);

	}

	closedir(dir);

#undef LISTPATH

#endif

}
