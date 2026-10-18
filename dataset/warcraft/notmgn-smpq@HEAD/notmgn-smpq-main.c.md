---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "main.c"
ref: HEAD
---

/*
    main.c - StormLib MPQ archiving utility
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
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>

#if defined(WIN32) || defined(_MSC_VER)
#define strcasecmp _stricmp
#endif

#ifdef _MSC_VER
#define S_ISDIR(x) ((x) & _S_IFDIR)
#endif

#include "common.h"

#define HELP \
	"Usage: smpq [action] [options] [archive] [files]\n" \
	"\n" \
	"SMPQ - StormLib MPQ archiving utility, version " VERSION "\n" \
	"\n" \
	"Action:\n" \
	"     -c, --create                  Create new archive with file(s)\n" \
	"     -a, --append, --add           Append file(s) to archive\n" \
	"     -d, -r, --delete, --remove    Remove file(s) from archive\n" \
	"     -R, --rename                  Rename file in archive\n" \
	"     -l, --list                    List file(s) of archive\n" \
	"     -e, -x, --extract             Extract file(s) from archive\n" \
	"     -i, --info                    Show info about archive\n" \
	"\n" \
	"     -h, -u, --help, --usage       Show this help/usage information\n" \
	"     -V, --license, --version      Show version license information\n" \
	"\n" \
	"Options:\n" \
	"     -L, --listfile <file>         Additional external listfile (not used when appending file)\n" \
	"     -n, --no-system-listfiles     Do not load system listfile(s) (not used when appending file)\n" \
	"     -N, --no-archive-listfile     Do not use/create archive listfile (file names will not be read/stored)\n" \
	"     -A, --no-attributes           Do not allow using file attributes (time, checksum, hash)\n" \
	"     -M, --mpq-version <version>   Specify MPQ archive version: (default: use new version 4)\n" \
	"          1  support up to 4GB size of archive \n" \
	"          2  support greater then 4GB size of archive (Introduced in World of Warcraft: The Burning Crusade)\n" \
	"          3  (Introduced in World of Warcraft: Cataclysm Beta)\n" \
	"          4  (Introduced in World of Warcraft: Cataclysm)\n" \
	"     -S, --sector-crc              Store/Check CRC for each sector, ignored if file has none compression or is single unit\n" \
	"     -q, --quiet                   Be quiet, do not show any output\n" \
	"     -f, -o, --force, --overwrite  Enable overwrite file(s)\n" \
	"     -v, --verbose                 Enable verbose output\n" \
	"     -O, --locale <id>             Set locale id (default: neutral=0)\n" \
	"          For all locale id see [MS-LCID] section 2.2: https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/\n" \
	"\n" \
	"Options for appending file(s) to archive:\n" \
	"     -m, --max-file-count <count>  Set maximum file count of archive (power of 2, 0 - autodetect) (default: 0)\n" \
	"     -E, --encrypt                 Store as encrypted\n" \
	"     -F, --fix-key                 Encryption key will be adjusted according to file size in the archive (need -E)\n" \
	"     -D, --deletion-marker         Set deletion marker\n" \
	"     -U, --single-unit             Add file as single unit, cannot be encrypted\n" \
	"     -C, --compression <method>    Compression method: (default: ZLIB)\n" \
	"          none                   None compression\n" \
	"          IMPLODE                Pkware Data Compression IMPLODE method - OBSOLETE (It was used only in Diablo I)\n" \
	"          PKWARE                 Pkware Data compression\n" \
	"          HUFFMANN               Huffmann compression (Introduced in Starcraft I)\n" \
	"          ADPCM_MONO             IMA ADPCM compression for 1-channel (mono) WAVE files - Lossy compression, only for WAVE files (Now it is not used)\n" \
	"          ADPCM_STEREO           IMA ADPCM compression for 2-channel (stereo) WAVE files - Lossy compression, only for WAVE files (Now it is not used)\n" \
	"          ZLIB                   ZLIB compression (Introduced in Warcraft III)\n" \
	"          BZIP2                  BZIP2 compression (Introduced in World of Warcraft: The Burning Crusade)\n" \
	"          SPARSE                 SPARSE compression (Introduced in Starcraft II)\n" \
	"          LZMA                   LZMA compression (Introduced in Starcraft II)\n" \
	"\n" \
	"          HUFFMANN+ADPCM_MONO    Together Huffmann and IMA ADPCM compression for 1-channel (mono) WAVE files\n" \
	"          HUFFMANN+ADPCM_STEREO  Together Huffmann and IMA ADPCM compression for 2-channel (stereo) WAVE files\n" \
	"          ZLIB+PKWARE            Together ZLIB and Pkware Data compression\n" \
	"          BZIP2+PKWARE           Together BZIP2 and Pkware Data compression\n" \
	"          SPARSE+ZLIB            Together SPARSE and ZLIB compression\n" \
	"          SPARSE+PKWARE          Together SPARSE and Pkware Data compression\n" \
	"          SPARSE+BZIP2           Together SPARSE and BZIP2 compression\n" \
	"          SPARSE+ZLIB+PKWARE     Together SPARSE, ZLIB and Pkware Data compression\n" \
	"          SPARSE+BZIP2+PKWARE    Together SPARSE, BZIP2 and Pkware Data compression\n" \
	"\n" \
	"Options for extracting file(s) from archive:\n" \
	"     -P, --partial                 Archive is partial (default: autodetect) (Partial archives were used by trial version of World of Warcraft)\n" \
	"     -X, --not-encrypted           Archive is not encrypted (default: autodetect) (Encrypted archives have Starcraft II installation)\n" \
	"     -p                            Open more (patched) archives with directory prefix (prefix:archive), when file is in more archives, will be extracted from last\n" \
	"          Usage with more (patched) archives:\n" \
	"            smpq -l|-x [options] [archive] -p [prefix1:archive1] [prefix2:archive2] ... -- [files]\n" \
	"\n" \
	"Examples:\n" \
	"       Create empty archive `archive.mpq'\n" \
	"         smpq -c archive.mpq\n" \
	"       Create archive `archive.mpq' with two files `file1.txt' and `file2.txt'\n" \
	"         smpq -c archive.mpq file1.txt file2.txt\n" \
	"       Extract all files from archive `archive.mpq' to current directory\n" \
	"         smpq -x archive.mpq\n" \
	"       Extract files with extension .txt from archive `archive.mpq'\n" \
	"         smpq -x archive.mpq '*.txt'\n" \
	"       Show information about archive `archive.mpq'\n" \
	"         smpq -i archive.mpq\n" \
	""

#define LICENSE \
	"SMPQ - StormLib MPQ archiving utility, version " VERSION "\n" \
	"Copyright (C) 2010 - 2016  Pali Rohar <pali.rohar@gmail.com>\n" \
	"\n" \
	"This program is free software: you can redistribute it and/or modify\n" \
	"it under the terms of the GNU General Public License as published by\n" \
	"the Free Software Foundation, either version 3 of the License, or\n" \
	"(at your option) any later version.\n" \
	"\n" \
	"This program is distributed in the hope that it will be useful,\n" \
	"but WITHOUT ANY WARRANTY; without even the implied warranty of\n" \
	"MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\n" \
	"GNU General Public License for more details.\n" \
	"\n" \
	"You should have received a copy of the GNU General Public License\n" \
	"along with this program.  If not, see <http://www.gnu.org/licenses/>.\n" \
	""

extern char StormLibCopyright[];

const char * app;

static char action = 0;
static unsigned int flags = 0;
static unsigned int skip = 0;

static void parse(char c) {

	switch ( c ) {

		case 'L':
			flags |= LISTFILE;
			skip = LISTFILE_ARG;
			break;

		case 'n':
			flags |= NO_SYSTEM_LF;
			break;

		case 'N':
			flags |= NO_LISTFILE;
			break;

		case 'A':
			flags |= NO_ATTRIBUTES;
			break;

		case 'M':
			flags |= MPQ_VERSION;
			skip = MPQ_VERSION_ARG;
			break;

		case 'S':
			flags |= SECTOR_CRC;
			break;

		case 'q':
			flags |= QUIET;
			break;

		case 'f':
		case 'o':

			flags |= OVERWRITE;
			break;

		case 'v':

			flags |= VERBOSE;
			break;

		case 'O':
			flags |= LOCALE;
			skip = LOCALE_ARG;
			break;

		case 'm':
			flags |= MAX_FILE_COUNT;
			skip = MAX_FILE_COUNT_ARG;
			break;

		case 'E':
			flags |= ENCRYPT;
			break;

		case 'F':
			flags |= FIX_KEY;
			break;

		case 'D':
			flags |= DELETE_MARKER;
			break;

		case 'U':
			flags |= SINGLE_UNIT;
			break;

		case 'C':
			flags |= COMPRESSION;
			skip = COMPRESSION_ARG;

		case 'p':
			flags |= MPQ_PATCHED;
			break;

		case 'P':
			flags |= MPQ_PARTIAL;
			break;

		case 'X':
			flags |= MPQ_NOT_ENCRYPTED;
			break;

		case 'c':
		case 'a':
		case 'd':
		case 'r':
		case 'R':
		case 'l':
		case 'e':
		case 'x':
		case 'i':

			if ( action != 0 ) {

				fprintf(stderr, "%s: Error: More then one action specified\n", app);
				exit(-1);

			}

			action = c;

			if ( action == 'l' ) {

				flags |= LIST;
				action = 'x';

			}

			if ( action == 'c' ) {

				flags |= CREATE;
				action = 'a';

			}

			if ( action == 'd' )
				action = 'r';

			if ( action == 'e' )
				action = 'x';

			break;

		case 'h':
		case 'u':

			printf(HELP);
			exit(0);

		case 'V':

			printf(LICENSE "\n\nSMPQ use %s\n", StormLibCopyright);
			exit(0);

		default:

			fprintf(stderr, "%s: Error: unknown option/action -%c specified\n", app, c);
			exit(-1);

	}


}

int main(int argc, char * argv[]) {

	int ret, i, j;
	int skipArg[10] = { 0 };

	unsigned int mpq_version = 4;
	const char * listfile = NULL;
	unsigned int locale = 0;
	unsigned int maxFileCount = 0;
	const char * compression = "ZLIB";
	const char * archive;

	int parchivesc;
	const char ** parchives;

	int filesc;
	const char ** files;

	app = argv[0];

	for ( i = 1; i < argc; ++i ) {

		if ( skip ) {

			skipArg[skip] = i;
			skip = 0;
			continue;

		}

		if ( argv[i][0] != '-' )
			break;

		if ( argv[i][1] == '-' ) {

			if ( strcmp(argv[i], "--create") == 0 )
				parse('c');
			else if ( strcmp(argv[i], "--append") == 0 || strcmp(argv[i], "--add") == 0 )
				parse('a');
			else if ( strcmp(argv[i], "--delete") == 0 || strcmp(argv[i], "--remove") == 0 )
				parse('r');
			else if ( strcmp(argv[i], "--rename") == 0 )
				parse('R');
			else if ( strcmp(argv[i], "--list") == 0 )
				parse('l');
			else if ( strcmp(argv[i], "--extract") == 0 )
				parse('x');
			else if ( strcmp(argv[i], "--info") == 0 )
				parse('i');
			else if ( strcmp(argv[i], "--help") == 0 || strcmp(argv[i], "--usage") == 0 )
				parse('h');
			else if ( strcmp(argv[i], "--license") == 0 || strcmp(argv[i], "--version") == 0 )
				parse('V');
			else if ( strcmp(argv[i], "--listfile") == 0 )
				parse('L');
			else if ( strcmp(argv[i], "--no-system-listfiles") == 0 )
				parse('n');
			else if ( strcmp(argv[i], "--no-archive-listfile") == 0 )
				parse('N');
			else if ( strcmp(argv[i], "--no-attributes") == 0 )
				parse('A');
			else if ( strcmp(argv[i], "--mpq-version") == 0 )
				parse('M');
			else if ( strcmp(argv[i], "--sector-crc") == 0 )
				parse('S');
			else if ( strcmp(argv[i], "--quiet") == 0 )
				parse('q');
			else if ( strcmp(argv[i], "--force") == 0 || strcmp(argv[i], "--overwrite") == 0 )
				parse('f');
			else if ( strcmp(argv[i], "--verbose") == 0 )
				parse('v');
			else if ( strcmp(argv[i], "--locale") == 0 )
				parse('O');
			else if ( strcmp(argv[i], "--max-file-count") == 0 )
				parse('m');
			else if ( strcmp(argv[i], "--encrypt") == 0 )
				parse('E');
			else if ( strcmp(argv[i], "--fix-key") == 0 )
				parse('F');
			else if ( strcmp(argv[i], "--delete-marker") == 0 )
				parse('D');
			else if ( strcmp(argv[i], "--single-unit") == 0 )
				parse('U');
			else if ( strcmp(argv[i], "--compression") == 0 )
				parse('C');
			else if ( strcmp(argv[i], "--partial") == 0 )
				parse('P');
			else if ( strcmp(argv[i], "--not-encrypted") == 0 )
				parse('X');
			else {
				fprintf(stderr, "%s Error: unknown option/action %s specified\n", app, argv[i]);
				return -1;
			}

			continue;

		}

		for ( j = 1; j < (int)strlen(argv[i]); ++j )
			parse(argv[i][j]);

	}

	if ( action == 0 ) {

		fprintf(stderr, "%s Error: No action specified\n", app);
		return -1;

	}

	if ( argc - i <= 0 ) {

		fprintf(stderr, "%s Error: No archive specified\n", app);
		return -1;

	}

	if ( flags & MPQ_VERSION ) {

		if ( skipArg[MPQ_VERSION_ARG] > argc-1 || skipArg[MPQ_VERSION_ARG] == 0 ) {

			fprintf(stderr, "%s Error: No MPQ archive version specified\n", app);
			return -1;

		}

		mpq_version = atoi(argv[skipArg[MPQ_VERSION_ARG]]);

		if ( mpq_version < 1 || mpq_version > 4 ) {

			fprintf(stderr, "%s Error: Unsupported MPQ archive version specified\n", app);
			return -1;

		}

	}

	if ( mpq_version == 1 )
		flags |= MPQ_VERSION_1;
	else if ( mpq_version == 2 )
		flags |= MPQ_VERSION_2;
	else if ( mpq_version == 3 )
		flags |= MPQ_VERSION_3;
	else if ( mpq_version == 4 )
		flags |= MPQ_VERSION_4;

	if ( flags & LISTFILE ) {

		struct stat st;

		if ( skipArg[LISTFILE_ARG] > argc-1 || skipArg[LISTFILE_ARG] == 0 ) {

			fprintf(stderr, "%s Error: No listfile specified\n", app);
			return -1;

		}

		listfile = argv[skipArg[LISTFILE_ARG]];

		if ( stat(listfile, &st) == -1 ) {

			fprintf(stderr, "%s Error: Cannot stat listfile `%s': %s\n", app, listfile, strerror(errno));
			return -1;

		}

		if ( S_ISDIR(st.st_mode) ) {

			fprintf(stderr, "%s Error: Cannot open listfile `%s': It is directory\n", app, listfile);
			return -1;

		}

	}

	if ( flags & LOCALE ) {

		if ( skipArg[LOCALE_ARG] > argc-1 || skipArg[LOCALE_ARG] == 0 ) {

			fprintf(stderr, "%s Error: No locale specified\n", app);
			return -1;

		}

		locale = strtol(argv[skipArg[LOCALE_ARG]], NULL, 0);

	}

	if ( flags & MAX_FILE_COUNT ) {

		if ( skipArg[MAX_FILE_COUNT_ARG] > argc-1 || skipArg[MAX_FILE_COUNT_ARG] == 0 ) {

			fprintf(stderr, "%s Error: No maximum file count specified\n", app);
			return -1;

		}

		maxFileCount = atoi(argv[skipArg[MAX_FILE_COUNT_ARG]]);

	}

	if ( flags & COMPRESSION ) {

		if ( skipArg[COMPRESSION_ARG] > argc-1 || skipArg[COMPRESSION_ARG] == 0 ) {

			fprintf(stderr, "%s Error: No MPQ compression method specified\n", app);
			return -1;

		}

		compression = argv[skipArg[COMPRESSION_ARG]];

	}

	archive = argv[i++];

	if ( ! ( flags & MPQ_NOT_ENCRYPTED ) && strlen(archive) > 5 && strcasecmp(archive+strlen(archive)-5, ".mpqe") == 0 )
		flags |= MPQ_ENCRYPTED;

	if ( action == 'i' ) {

		if ( i < argc-1 ) {

			fprintf(stderr, "%s Error: Info need only one archive\n", app);
			return -1;

		}

		return smpq_info(archive, flags);

	}

	if ( action == 'R' ) {

		if ( i+1 < argc-1 ) {

			fprintf(stderr, "%s Error: Cannot rename more then one file\n", app);
			return -1;

		}

		return smpq_rename(archive, argv[i], argv[i+1], flags, listfile, locale);

	}

	parchivesc = argc - i - 1;
	parchives = (const char **)malloc(argc * sizeof(const char *));

	if ( action == 'x' && i < argc && strcmp(argv[i], "-p") == 0 ) {

		while ( i < argc ) {

			if ( strcmp(argv[i], "--") == 0 )
				break;

			++i;

			parchives[parchivesc - argc + i] = argv[i];

		}

	}

	if ( parchivesc - argc + i <= 0 ) {

		parchives[0] = NULL;

	} else {

		parchives[parchivesc - argc + i] = NULL;

		if ( i < argc )
			++i;

	}

	filesc = argc - i;
	files = (const char **)malloc(argc * sizeof(const char *));

	for ( ; i < argc; ++i )
		files[filesc - argc + i] = argv[i];

	if ( action == 'x' ) {

		if ( filesc == 0 ) {

			filesc = 1;
			files[0] = "*";

		}

	} else if ( action != 'a' || ! ( flags & CREATE ) ) {

		if ( filesc == 0 ) {

			fprintf(stderr, "%s Error: No file(s) specified\n", app);
			free((void *)parchives);
			free((void *)files);
			return -1;

		}

	}

	files[filesc] = NULL;

	switch ( action ) {

		case 'a':
			ret = smpq_append(archive, files, flags, locale, maxFileCount, compression);
			break;

		case 'x':
			ret = smpq_extract(archive, files, flags, listfile, locale, parchives);
			break;

		case 'r':
			ret = smpq_remove(archive, files, flags, listfile, locale);
			break;

		default:
			ret = 1;
			break;

	}

	free((void *)parchives);
	free((void *)files);
	return ret;

}
