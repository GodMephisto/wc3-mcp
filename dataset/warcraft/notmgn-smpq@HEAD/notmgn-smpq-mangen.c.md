---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "mangen.c"
ref: HEAD
---

/*
    mangen.c - StormLib MPQ archiving utility
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

#define main main_unused
#include "main.c"
#undef main

char StormLibCopyright[] = { 0 };
const char * app = "smpq";
int smpq_append(const char * archive, const char * const files[], unsigned int flags, unsigned int locale, unsigned int maxFileCount, const char * compression) { (void)archive; (void)files; (void)flags; (void)locale; (void)maxFileCount; (void)compression; return 0; }
int smpq_extract(const char * archive, const char * const files[], unsigned int flags, const char * listfile, unsigned int locale, const char * const parchives[]) { (void)archive; (void)files; (void)flags; (void)listfile; (void)locale; (void)parchives; return 0; }
int smpq_info(const char * archive, unsigned int flags) { (void)archive; (void)flags; return 0; }
int smpq_remove(const char * archive, const char * const files[], unsigned int flags, const char * listfile, unsigned int locale) { (void)archive; (void)files; (void)flags; (void)listfile; (void)locale; return 0; }
int smpq_rename(const char * archive, const char * oldName, const char * newName, unsigned int flags, const char * listfile, unsigned int locale) { (void)archive; (void)oldName; (void)newName; (void)newName; (void)flags; (void)listfile; (void)locale; return 0; }

#include <stdio.h>
#include <string.h>

#define prints(s, e) do { const char * _c; for ( _c = s; _c < e; ++_c ) putchar(*_c); } while (0)

int main(void) {

	const char * start = HELP;
	const char * end = start;
	const char * len = start + strlen(HELP) + 1;

	printf("'\\\" t -*- coding: UTF-8 -*-\n");
	printf(".TH SMPQ 1 \"" __DATE__ "\" \"SMPQ - StormLib MPQ archiving utility, version " VERSION "\"\n");

	while ( start < len && ( end = strchr(start, '\n') ) != NULL ) {

		if ( strncmp(start, "Usage:", 6) == 0 ) {

			const char * next = strstr(start+6, "smpq");
			if ( next == NULL || next > end )
				next = start+6;
			else
				next += strlen("smpq");

			printf("\n.SH SYNOPSIS\n.B smpq\n.I ");
			prints(next, end);
			putchar('\n');

		} else if ( strncmp(start, "SMPQ", 4) == 0 ) {

			printf("\n.SH NAME\n");
			prints(start, end);
			putchar('\n');

		} else if ( *(end - 1) == ':' ) {

			printf("\n.SH ");
			prints(start, end - 1);
			putchar('\n');

		} else if ( strncmp(start, "       ", 7) == 0 ) {

			const char * next = start;
			while ( *(++next) == ' ' && next < end );

			putchar('\n');
			prints(next, end);
			putchar('\n');

		} else if ( *start == ' ' ) {

			const char * next;
			const char * next2;
			const char * next3;

			next = start;
			while ( *(++next) == ' ' && next < end );

			next2 = strstr(next, "  ");
			if ( next2 == NULL || next2 > end )
				next2 = next;

			next3 = next2-1;
			while ( *(++next3) == ' ' && next3 < end );

			printf(".TP\n.B ");
			prints(next, next2);
			putchar('\n');
			prints(next3, end);
			putchar('\n');

		} else {

			prints(start, end);
			putchar('\n');

		}

		start = end + 2;

	}

	printf("\n.SH AUTHOR\nSMPQ is written by Pali Rohár <pali.rohar@gmail.com>\n\n.SH LICENSE\nSMPQ is distributed under GNU GPL v3\n");

	return 0;

}
