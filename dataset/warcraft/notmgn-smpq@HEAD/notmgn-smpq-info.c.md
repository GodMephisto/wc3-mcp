---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "info.c"
ref: HEAD
---

/*
    info.cpp - StormLib MPQ archiving utility
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

static inline unsigned int GetInfo(HANDLE SArchive, SFileInfoClass info) {

	unsigned int ret;

	if ( ! SFileGetFileInfo(SArchive, info, &ret, sizeof(ret), NULL) )
		return 0;

	if ( info == SFileMpqNumberOfFiles ) {

		static const char * internalFiles[] = { "(listfile)", "(signature)", "(attributes)", "(patch_metadata)" };
		int i;

		for ( i = 0; i < sizeof(internalFiles) / sizeof(internalFiles[0]); ++i ) {

			SFILE_FIND_DATA SFileFindData;
			HANDLE SFileFind;

			SFileFind = SFileFindFirstFile(SArchive, internalFiles[i], &SFileFindData, NULL);

			if ( SFileFind ) {

				--ret;
				SFileFindClose(SFileFind);

			}

		}

	}

	return ret;

}

int smpq_info(const char * archive, unsigned int flags) {

	HANDLE SArchive = NULL;

	unsigned int streamFlags;
	unsigned int verify;

	unsigned int SFlags = STREAM_FLAG_READ_ONLY;

	if ( flags & MPQ_VERSION_1 )
		SFlags |= MPQ_OPEN_FORCE_MPQ_V1;

	if ( flags & MPQ_ENCRYPTED )
		SFlags |= STREAM_PROVIDER_MPQE;

	if ( ! SFileOpenArchive(archive, 0, SFlags, &SArchive) ) {

		printError(archive, "Cannot open archive", archive, GetLastError());
		return -1;

	}

	printMessage("Archive name: %s", archive);
	printMessage("Archive size: %u", GetInfo(SArchive, SFileMpqArchiveSize));
	printMessage("Number of files in archive: %u", GetInfo(SArchive, SFileMpqNumberOfFiles));
	printMessage("Maximum file count of archive: %u", GetInfo(SArchive, SFileMpqMaxFileCount));
	printMessage("Hash table size: %u", GetInfo(SArchive, SFileMpqHashTableSize));
	printMessage("Block table size: %u", GetInfo(SArchive, SFileMpqBlockTableSize));
	printMessage("Sector size: %u", GetInfo(SArchive, SFileMpqSectorSize));

	streamFlags = GetInfo(SArchive, SFileMpqStreamFlags);

	if ( streamFlags & STREAM_PROVIDER_PARTIAL )
		printMessage("Archive partial: Yes");
	else
		printMessage("Archive partial: No");

	if ( streamFlags & STREAM_PROVIDER_MPQE )
		printMessage("Archive encryped: Yes");
	else
		printMessage("Archive encryped: No");

	verify = SFileVerifyArchive(SArchive);

	if ( verify == ERROR_NO_SIGNATURE )
		printMessage("Archive signature: No signature");
	else if ( verify == ERROR_VERIFY_FAILED )
		printMessage("Archive signature: Verification failed");
	else if ( verify == ERROR_WEAK_SIGNATURE_OK )
		printMessage("Archive signature: Weak digital signature - Valid");
	else if ( verify == ERROR_WEAK_SIGNATURE_ERROR )
		printMessage("Archive signature: Weak digital signature - Invalid");
	else if ( verify == ERROR_STRONG_SIGNATURE_OK )
		printMessage("Archive signature: Strong digital signature - Valid");
	else if ( verify == ERROR_STRONG_SIGNATURE_ERROR )
		printMessage("Archive signature: Strong digital signature - Invalid or No public key");

	SFileCloseArchive(SArchive);

	return 0;

}
