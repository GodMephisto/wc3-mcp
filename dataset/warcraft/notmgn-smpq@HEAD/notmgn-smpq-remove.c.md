---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "remove.c"
ref: HEAD
---

/*
    remove.cpp - StormLib MPQ archiving utility
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

#include <string.h>

#include "common.h"

int smpq_remove(const char * archive, const char * const files[], unsigned int flags, const char * listfile, unsigned int locale) {

	int i;
	int needCompact = 0;
	HANDLE SArchive = NULL;

	unsigned int SFlags = 0;

	if ( flags & NO_LISTFILE )
		SFlags |= MPQ_OPEN_NO_LISTFILE;

	if ( flags & NO_ATTRIBUTES )
		SFlags |= MPQ_OPEN_NO_ATTRIBUTES;

	if ( flags & MPQ_VERSION_1 )
		SFlags |= MPQ_OPEN_FORCE_MPQ_V1;

	if ( flags & SECTOR_CRC )
		SFlags |= MPQ_OPEN_CHECK_SECTOR_CRC;

	if ( ! SFileOpenArchive(archive, 0, SFlags, &SArchive) ) {

		if ( ! ( flags & QUIET ) )
			printError(archive, "Cannot open archive", archive, GetLastError());

		return -1;

	}

	if ( ! ( flags & NO_SYSTEM_LF ) )
		smpq_systemlistfiles(SArchive, archive, flags);

	if ( ! ( flags & NO_LISTFILE ) )
		SFileAddListFile(SArchive, NULL);

	SFileSetLocale(locale);

	for ( i = 0; files[i]; ++i ) {

		const char * fileName = files[i];
		char SFileName[1024];

		if ( strlen(fileName)+1 > 1024 )
			continue;

		toArchivePath(SFileName, fileName);

		if ( flags & VERBOSE )
			printVerbose(archive, "Remove file", SFileName);

		SFlags = SFILE_OPEN_FROM_MPQ;

		if ( ! SFileRemoveFile(SArchive, SFileName, SFlags) ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "Cannot remove existing file", SFileName, GetLastError());

			continue;

		}

		if ( ! needCompact )
			needCompact = 1;

		SFileFlushArchive(SArchive);

	}

	if ( needCompact ) {

		unsigned int fileCount;
		unsigned int maxFileCount;

		if ( ! SFileGetFileInfo(SArchive, SFileMpqNumberOfFiles, &fileCount, sizeof(fileCount), 0) )
			fileCount = 6;

		if ( ! SFileGetFileInfo(SArchive, SFileMpqMaxFileCount, &maxFileCount, sizeof(maxFileCount), 0) )
			maxFileCount = 6;

		if ( fileCount < 6 )
			fileCount = 6;

		if ( fileCount * 2 <= maxFileCount ) {

			if ( flags & VERBOSE )
				printVerbose(archive, "Change maximum file count", archive);

			if ( ! SFileSetMaxFileCount(SArchive, fileCount) )
				if ( ! ( flags & QUIET ) )
					printError(archive, "Cannot change maximum file count", archive, GetLastError());

		}

		if ( flags & VERBOSE )
			printVerbose(archive, "Compact archive", archive);

		if ( ! SFileCompactArchive(SArchive, listfile, 0) )
			if ( ! ( flags & QUIET ) )
				printError(archive, "Cannot compact archive", archive, GetLastError());

		SFileFlushArchive(SArchive);

	}

	SFileCloseArchive(SArchive);

	return 0;

}
