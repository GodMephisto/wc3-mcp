---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "append.c"
ref: HEAD
---

/*
    append.cpp - StormLib MPQ archiving utility
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
#include <errno.h>
#include <string.h>

#if defined(WIN32) || defined(_MSC_VER)
#define strcasecmp _stricmp
#define strncasecmp _strnicmp
#endif

#include "common.h"

int smpq_append(const char * archive, const char * const files[], unsigned int flags, unsigned int locale, unsigned int maxFileCount, const char * compression) {

	unsigned int SFlags = 0;
	unsigned int SCompFlags = 0;

	int i;
	HANDLE SArchive = NULL;

	if ( flags & ENCRYPT )
		SFlags |= MPQ_FILE_ENCRYPTED;

	if ( flags & FIX_KEY )
		SFlags |= MPQ_FILE_FIX_KEY;

	if ( flags & DELETE_MARKER )
		SFlags |= MPQ_FILE_DELETE_MARKER;

	if ( flags & SECTOR_CRC )
		SFlags |= MPQ_FILE_SECTOR_CRC;

	if ( flags & SINGLE_UNIT )
		SFlags |= MPQ_FILE_SINGLE_UNIT;

	if ( flags & OVERWRITE )
		SFlags |= MPQ_FILE_REPLACEEXISTING;

	if ( compression != NULL ) {

		if ( strcmp(compression, "none") == 0 ) {

		} else if ( strcasecmp(compression, "IMPLODE") == 0 ) {

			SFlags |= MPQ_FILE_IMPLODE;

		} else {

			SFlags |= MPQ_FILE_COMPRESS;

			if ( strcasecmp(compression, "HUFFMANN") == 0 )
				SCompFlags |= MPQ_COMPRESSION_HUFFMANN;
			else if ( strcasecmp(compression, "ADPCM_MONO") == 0 )
				SCompFlags |= MPQ_COMPRESSION_ADPCM_MONO;
			else if ( strcasecmp(compression, "ADPCM_STEREO") == 0 )
				SCompFlags |= MPQ_COMPRESSION_ADPCM_STEREO;
			else if ( strcasecmp(compression, "ZLIB") == 0 )
				SCompFlags |= MPQ_COMPRESSION_ZLIB;
			else if ( strcasecmp(compression, "PKWARE") == 0 )
				SCompFlags |= MPQ_COMPRESSION_PKWARE;
			else if ( strcasecmp(compression, "BZIP2") == 0 )
				SCompFlags |= MPQ_COMPRESSION_BZIP2;
			else if ( strcasecmp(compression, "SPARSE") == 0 )
				SCompFlags |= MPQ_COMPRESSION_SPARSE;
			else if ( strcasecmp(compression, "LZMA") == 0 )
				SCompFlags |= MPQ_COMPRESSION_LZMA;
			else if ( strcasecmp(compression, "HUFFMANN+ADPCM_MONO") == 0 )
				SCompFlags |= MPQ_COMPRESSION_HUFFMANN | MPQ_COMPRESSION_ADPCM_MONO;
			else if ( strcasecmp(compression, "HUFFMANN+ADPCM_STEREO") == 0 )
				SCompFlags |= MPQ_COMPRESSION_HUFFMANN | MPQ_COMPRESSION_ADPCM_STEREO;
			else if ( strcasecmp(compression, "ZLIB+PKWARE") == 0 )
				SCompFlags |= MPQ_COMPRESSION_ZLIB | MPQ_COMPRESSION_PKWARE;
			else if ( strcasecmp(compression, "BZIP2+PKWARE") == 0 )
				SCompFlags |= MPQ_COMPRESSION_BZIP2 | MPQ_COMPRESSION_PKWARE;
			else if ( strcasecmp(compression, "SPARSE+ZLIB") == 0 )
				SCompFlags |= MPQ_COMPRESSION_SPARSE | MPQ_COMPRESSION_ZLIB;
			else if ( strcasecmp(compression, "SPARSE+PKWARE") == 0 )
				SCompFlags |= MPQ_COMPRESSION_SPARSE | MPQ_COMPRESSION_PKWARE;
			else if ( strcasecmp(compression, "SPARSE+BZIP2") == 0 )
				SCompFlags |= MPQ_COMPRESSION_SPARSE | MPQ_COMPRESSION_BZIP2;
			else if ( strcasecmp(compression, "SPARSE+ZLIB+PKWARE") == 0 )
				SCompFlags |= MPQ_COMPRESSION_SPARSE | MPQ_COMPRESSION_ZLIB | MPQ_COMPRESSION_PKWARE;
			else if ( strcasecmp(compression, "SPARSE+BZIP2+PKWARE") == 0 )
				SCompFlags |= MPQ_COMPRESSION_SPARSE | MPQ_COMPRESSION_BZIP2 | MPQ_COMPRESSION_PKWARE;
			else if ( strcasecmp(compression, "choose") == 0 ) {

				if ( ! ( flags & QUIET ) )
					printError(archive, "Choose the best compression is not implemented yet", compression, EINVAL);

				return -1;

			} else {

				if ( ! ( flags & QUIET ) )
					printError(archive, "Specified unknown compression method", compression, EINVAL);

				return -1;

			}

		}

	}

	if ( flags & CREATE ) {

		struct stat st;
		unsigned int SOpenFlags = 0;

		if ( ( flags & OVERWRITE ) && stat(archive, &st) == 0 ) {

			if ( flags & VERBOSE )
				printVerbose(archive, "Remove old archive", archive);

			if ( unlink(archive) != 0 ) {

				if ( ! ( flags & QUIET ) )
					printError(archive, "Cannot remove existing archive", archive, errno);

				return -1;

			}

		}

		if ( flags & VERBOSE )
			printVerbose(archive, "Create new archive", archive);

		if ( flags & MPQ_VERSION_1 )
			SOpenFlags |= MPQ_CREATE_ARCHIVE_V1;
		else if ( flags & MPQ_VERSION_2 )
			SOpenFlags |= MPQ_CREATE_ARCHIVE_V2;
		else if ( flags & MPQ_VERSION_3 )
			SOpenFlags |= MPQ_CREATE_ARCHIVE_V3;
		else
			SOpenFlags |= MPQ_CREATE_ARCHIVE_V4;

		if ( ! ( flags & NO_ATTRIBUTES ) )
			SOpenFlags |= MPQ_CREATE_ATTRIBUTES;

		if ( maxFileCount == 0 )
			for ( i = 0; files[i]; ++i )
				++maxFileCount;

		if ( maxFileCount < 4 )
			maxFileCount = 4;

		if ( ! SFileCreateArchive(archive, SOpenFlags, maxFileCount, &SArchive) ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "Cannot create archive", archive, GetLastError());

			return -1;

		}

	} else {

		unsigned int SOpenFlags = 0;

		if ( flags & NO_LISTFILE )
			SOpenFlags |= MPQ_OPEN_NO_LISTFILE;

		if ( flags & NO_ATTRIBUTES )
			SOpenFlags |= MPQ_OPEN_NO_ATTRIBUTES;

		if ( flags & MPQ_VERSION_1 )
			SOpenFlags |= MPQ_OPEN_FORCE_MPQ_V1;

		if ( ! SFileOpenArchive(archive, 0, SOpenFlags, &SArchive) ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "Cannot open archive", archive, GetLastError());

			return -1;

		}

		if ( maxFileCount == 0 ) {

			unsigned int fileCount;

			if ( ! SFileGetFileInfo(SArchive, SFileMpqNumberOfFiles, &fileCount, sizeof(fileCount), 0) )
				fileCount = 0;

			for ( i = 0; files[i]; ++i )
				++fileCount;

			if ( ! SFileGetFileInfo(SArchive, SFileMpqMaxFileCount, &maxFileCount, sizeof(maxFileCount), 0) )
				maxFileCount = 0;

			if ( maxFileCount < fileCount )
				maxFileCount = fileCount;
			else
				maxFileCount = 0;

		}

		if ( maxFileCount != 0 ) {

			if ( flags & VERBOSE )
				printVerbose(archive, "Change maximum file count", archive);

			if ( ! SFileSetMaxFileCount(SArchive, maxFileCount) ) {

				if ( ! ( flags & QUIET ) )
					printError(archive, "Cannot change maximum file count", archive, GetLastError());

				SFileCloseArchive(SArchive);
				return -1;

			}

		}

	}

	SFileSetLocale(locale);

	for ( i = 0; files[i]; ++i ) {

		struct stat st;
		FILE * file = NULL;
		const char * fileName = files[i];
		size_t fileSize = 0;

		HANDLE SFile = NULL;
		char SFileName[1024];
		unsigned long long int SFileTime = 0;

		char buffer[0x10000];
		size_t bytes = 0;

		if ( strlen(fileName) + 1 > 1024 ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "File `%s' has too long path. Cannot create new file", SFileName, EPERM);

			continue;

		}

		toArchivePath(SFileName, fileName);

		if ( strlen(SFileName) == 16 && strncasecmp(SFileName, "File", 4) == 0 && SFileName[12] == '.' ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "File with mask `File????????.???\' is not allowed. Cannot create new file", SFileName, EPERM);

			continue;

		}

		if ( strcmp(SFileName, "(listfile)") == 0 || strcmp(SFileName, "(signature)") == 0 || strcmp(SFileName, "(attributes)") == 0 || strstr(SFileName, "(patch_metadata)") != NULL ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "Files `(listfile)' `(signature)' `(attributes)' `(patch_metadata)' are for internal usage. Cannot create new file", SFileName, EPERM);

			continue;

		}

		file = fopen(fileName, "rb");

		if ( ! file ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "Cannot open file", fileName, errno);

			continue;

		}

		fseek(file, 0, SEEK_END);
		fileSize = ftell(file);
		rewind(file);

		if ( flags & VERBOSE )
			printVerbose(archive, "Append file", SFileName);

		if ( stat(fileName, &st) == -1 ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "Cannot stat file", fileName, errno);

			fclose(file);
			continue;

		}

		toFileTime(&SFileTime, st.st_mtime);

		if ( ! SFileCreateFile(SArchive, SFileName, SFileTime, fileSize, locale, SFlags, &SFile) ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "Cannot create new file", SFileName, GetLastError());

			fclose(file);
			continue;

		}

		while ( 1 ) {

			bytes = fread(buffer, 1, sizeof(buffer), file);

			if ( ferror(file) ) {

				if ( ! ( flags & QUIET ) )
					printError(archive, "Cannot read file", fileName, errno);

				break;

			}

			if ( ! SFileWriteFile(SFile, buffer, bytes, SCompFlags) ) {

				if ( ! ( flags & QUIET ) )
					printError(archive, "Cannot write file new", SFileName, GetLastError());

				break;

			}

			if ( feof(file) )
				break;

		}

		fclose(file);
		SFileFinishFile(SFile);
		SFileFlushArchive(SArchive);

	}

	if ( flags & OVERWRITE )
		SFileCompactArchive(SArchive, NULL, 0);

	SFileCloseArchive(SArchive);

	return 0;

}
