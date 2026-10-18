---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "rename.c"
ref: HEAD
---

/*
    rename.cpp - StormLib MPQ archiving utility
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

#include "common.h"

int smpq_rename(const char * archive, const char * oldName, const char * newName, unsigned int flags, const char * listfile, unsigned int locale) {

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

	SFileAddListFile(SArchive, listfile);

	SFileSetLocale(locale);

	if ( flags & VERBOSE )
		printVerbose(archive, "Rename file", oldName);

	if ( ! SFileRenameFile(SArchive, oldName, newName) ) {

		if ( ! ( flags & QUIET ) )
			printError(archive, "Cannot rename file", oldName, GetLastError());

		SFileCloseArchive(SArchive);

		return -1;

	}

	SFileCloseArchive(SArchive);

	return 0;

}
