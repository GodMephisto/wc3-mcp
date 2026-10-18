---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "extract.c"
ref: HEAD
---

/*
    extract.cpp - StormLib MPQ archiving utility
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
#include <time.h>
#include <stdlib.h>
#include <stdio.h>

#if defined(WIN32) || defined(_MSC_VER)

#include <sys/utime.h>
#include <direct.h>
#include <io.h>

#define mkdir _mkdir
#define stat _stat
#define utime _utime
#define utimbuf _utimbuf

#endif

#ifdef _MSC_VER

#define S_ISDIR(x) ((x) & _S_IFDIR)
char * dirname(char *);

#else

#include <libgen.h>
#include <utime.h>

#endif

#include "common.h"

static int mkpath(const char * s) {

	char * q = NULL;
	char * r = NULL;
	char * path = NULL;
	char * up = NULL;
	int rv = -1;

#if defined(WIN32) || defined(_MSC_VER)
	if ( strcmp(s, "\\") == 0 )
		return 0;
#endif

	if ( strcmp(s, ".") == 0 || strcmp(s, "/") == 0 )
		return 0;

	if ( ( path = strdup(s) ) == NULL )
		return -1;

	if ( ( q = strdup(s) ) == NULL )
		return -1;

	if ( ( r = dirname(q) ) == NULL )
		goto out;

	if ( ( up = strdup(r) ) == NULL )
		return -1;

	if ( ( mkpath(up) == -1 ) && ( errno != EEXIST ) )
		goto out;

#if defined(WIN32) || defined(_MSC_VER)
	if ( ( mkdir(path) != -1 ) || ( errno == EEXIST ) )
#else
	if ( ( mkdir(path, S_IRWXU|S_IRGRP|S_IXGRP|S_IROTH|S_IXOTH) != -1 ) || ( errno == EEXIST ) )
#endif
		rv = 0;

out:
	if ( up != NULL )
		free(up);

	free(q);
	free(path);

	return rv;

}

struct trie {

	struct trie * t[255];
	int end;

};

static const struct trie * trie_alloc(void) {

	struct trie * t = (struct trie *)calloc(1, sizeof(struct trie));
	return t;

}

static void trie_free(const struct trie * t) {

	unsigned int i;

	if ( ! t )
		return;

	for ( i = 0; i < 255; ++i )
		if ( t->t[i] )
			trie_free(t->t[i]);

	free((struct trie *)t);

}

static void trie_add(const struct trie * tr, const char * str) {

	unsigned int i;
	unsigned int len = strlen(str);

	struct trie * t = (struct trie *)tr;

	for ( i = 0; i < len; ++i ) {

		if ( ! t->t[(int)str[i]] )
			t->t[(int)str[i]] = (struct trie *)trie_alloc();

		t = t->t[(int)str[i]];

	}

	t->end = 1;

}

static int trie_find(const struct trie * tr, const char * str) {

	unsigned int i;
	unsigned int len = strlen(str);

	struct trie * t = (struct trie *)tr;

	for ( i = 0; i < len; ++i ) {

		t = t->t[(int)str[i]];

		if ( ! t )
			return 0;

	}

	return t->end;

}

int smpq_extract(const char * archive, const char * const files[], unsigned int flags, const char * listfile, unsigned int locale, const char * const parchives[]) {

	int i, j;
	HANDLE SArchive = NULL;

	unsigned int SFlags = STREAM_FLAG_READ_ONLY;

	if ( flags & NO_LISTFILE )
		SFlags |= MPQ_OPEN_NO_LISTFILE;

	if ( flags & NO_ATTRIBUTES )
		SFlags |= MPQ_OPEN_NO_ATTRIBUTES;

	if ( flags & MPQ_VERSION_1 )
		SFlags |= MPQ_OPEN_FORCE_MPQ_V1;

	if ( flags & SECTOR_CRC )
		SFlags |= MPQ_OPEN_CHECK_SECTOR_CRC;

	if ( flags & MPQ_PARTIAL )
		SFlags |= STREAM_PROVIDER_PARTIAL;

	if ( flags & MPQ_ENCRYPTED )
		SFlags |= STREAM_PROVIDER_MPQE;

	if ( ! SFileOpenArchive(archive, 0, SFlags, &SArchive) ) {

		if ( ! ( flags & QUIET ) )
			printError(archive, "Cannot open archive", archive, GetLastError());

		return -1;

	}

	for ( i = 0; parchives[i]; ++i ) {

		char * parchive;
		const char * prefix;

		if ( ( parchive = strchr((char *)parchives[i], ':') ) ) {

			*parchive = 0;
			++parchive;
			prefix = parchives[i];

		} else {

			parchive = (char *)parchives[i];
			prefix = "";

		}

		if ( flags & VERBOSE )
			printVerbose(archive, "Opening patched archive", parchive);

		if ( ! SFileOpenPatchArchive(SArchive, parchive, prefix, 0) ) {

			if ( ! ( flags & QUIET ) )
				printError(archive, "Cannot open patched archive", parchive, GetLastError());

			SFileCloseArchive(SArchive);

			return -1;

		}

	}

	if ( ! ( flags & NO_SYSTEM_LF ) )
		smpq_systemlistfiles(SArchive, archive, flags);

	if ( ! ( flags & NO_LISTFILE ) )
		SFileAddListFile(SArchive, NULL);

	SFileSetLocale(locale);

	SFlags = SFILE_OPEN_FROM_MPQ;

	for ( i = 0; files[i]; ++i ) {

		const struct trie * t;
		char mask[512];

		SFILE_FIND_DATA SFileFindData;
		HANDLE SFileFind = NULL;

		toArchivePath(mask, files[i]);

		SFileFind = SFileFindFirstFile(SArchive, mask, &SFileFindData, listfile);

		if ( ! SFileFind ) {

			HANDLE SFile;

			SFileFind = (HANDLE)0xFFFFFFFF;
			SFileFindData.dwFileTimeLo = 0;
			SFileFindData.dwFileTimeHi = 0;

			strcpy(SFileFindData.cFileName, mask);

			if ( SFileOpenFileEx(SArchive, mask, SFlags, &SFile) ) {

				unsigned int high = 0;
				unsigned int low = SFileGetFileSize(SFile, (DWORD*)&high);

				SFileGetFileName(SFile, mask);

				SFileFindData.dwFileSize = low | ( (unsigned long long int)high << 32 );

				SFileCloseFile(SFile);

			}

		}

		t = trie_alloc();

		while ( SFileFind ) {

			struct stat st;
			FILE * file = NULL;
			char fileName[1024];
			char fileDir[1024];
			unsigned int fileSize = SFileFindData.dwFileSize;
			time_t fileTime = 0;

			HANDLE SFile = NULL;
			const char * SFileName = SFileFindData.cFileName;
			unsigned long long int SFileTime = SFileFindData.dwFileTimeLo | ( ((unsigned long long int)SFileFindData.dwFileTimeHi) << 32 );

			int last = 0;
			char buffer[0x10000];
			size_t bytes = 1;

			if ( strlen(SFileFindData.cFileName)+1 > 1024 )
				goto next;

			if ( strcmp(SFileName, "(listfile)") == 0 || strcmp(SFileName, "(signature)") == 0 || strcmp(SFileName, "(attributes)") == 0 || strstr(SFileName, "(patch_metadata)") != NULL )
				goto next;

			fromArchivePath(fileName, SFileName);

			if ( trie_find(t, SFileName) )
				goto next;
			else
				trie_add(t, SFileName);

			if ( ! fromFileTime(&fileTime, SFileTime) )
				fileTime = 0;

			if ( ! SFileOpenFileEx(SArchive, SFileName, SFlags, &SFile) ) {

				if ( ! ( flags & QUIET ) )
					printError(archive, "Cannot open file in archive", SFileName, GetLastError());

				goto next;

			}

			j = -1;

			while ( SFileName[++j] )
				if ( SFileName[j] == '\\' )
					last = j;

			if ( ( flags & VERBOSE ) && ! ( flags & LIST ) )
				printVerbose(archive, "Extract", fileName);

			if ( ( flags & LIST ) ) {

				char strtime[80];
				strftime(strtime, 80, "%Y-%m-%d %H:%M", localtime(&fileTime));
				printMessage("%12u %s %s", fileSize, strtime, fileName);

			}

			if ( flags & LIST )
				goto next;

			memcpy(fileDir, fileName, last);
			fileDir[last] = 0;

			if ( last != 0 ) {

				if ( mkpath(fileDir) != 0 ) {

					if ( ! ( flags & QUIET ) ) {

						printError(archive, "Cannot create directory", fileDir, errno);
						printError(archive, "Cannot extract file", fileName, ENOENT);

					}

					goto next;

				}

			}

			if ( stat(fileName, &st) != -1 ) {

				if ( ! ( flags & OVERWRITE ) ) {

					if ( ! ( flags & QUIET ) )
						printError(archive, "Cannot extract file", fileName, EEXIST);

					goto next;

				}

				if ( S_ISDIR(st.st_mode) ) {

					if ( ! ( flags & QUIET ) )
						printError(archive, "Cannot extract file", fileName, EISDIR);

					goto next;

				}

				if ( flags & VERBOSE )
					printVerbose(archive, "Remove old file", fileName);

				if ( unlink(fileName) != 0 ) {

					if ( ! ( flags & QUIET ) )
						printError(archive, "Cannot remove existing file", fileName, errno);

					goto next;

				}

			}

			file  = fopen(fileName, "wb");

			if ( ! file ) {

				if ( ! ( flags & QUIET ) )
					printError(archive, "Cannot open file", fileName, errno);

				goto next;

			}

			while ( 1 ) {

				int eof = 0;

				if ( ! SFileReadFile(SFile, buffer, sizeof(buffer), (DWORD *)&bytes, NULL) ) {

					eof = ( GetLastError() == ERROR_HANDLE_EOF );

					if ( ! eof ) {

						if ( ! ( flags & QUIET ) )
							printError(archive, "Cannot read file", SFileName, GetLastError());

						break;

					}

				}

				if ( fwrite(buffer, 1, bytes, file) != bytes ) {

					if ( ! ( flags & QUIET ) )
						printError(archive, "Cannot write file", fileName, errno);

					break;

				}

				if ( eof )
					break;

			}

			fclose(file);

			{
				struct utimbuf fileTimeBuf = { fileTime, fileTime };
				utime(fileName, &fileTimeBuf);
			}

next:
			if ( SFile )
				SFileCloseFile(SFile);

			if ( SFileFind == (HANDLE)0xFFFFFFFF )
				break;

			if ( ! SFileFindNextFile(SFileFind, &SFileFindData) )
				break;

		}

		trie_free(t);

		if ( SFileFind != (HANDLE)0xFFFFFFFF )
			SFileFindClose(SFileFind);

	}

	SFileCloseArchive(SArchive);

	return 0;

}
