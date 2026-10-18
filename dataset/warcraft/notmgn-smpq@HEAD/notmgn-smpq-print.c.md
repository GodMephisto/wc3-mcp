---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "print.c"
ref: HEAD
---

/*
    print.c - StormLib MPQ archiving utility
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

#include <stdio.h>
#include <string.h>
#include <stdarg.h>

#include "common.h"

void printError(const char * archive, const char * message, const char * file, int errnum) {

	const char * error = NULL;

	if ( errnum < 105 )
		error = strerror(errnum);
	else if ( errnum == 105 )
		error = "Bad format";
	else if ( errnum == 106 )
		error = "No more files";
	else if ( errnum == 107 )
		error = "Handle EOF";
	else if ( errnum == 108 )
		error = "Cannot compile";
	else if ( errnum == 109 )
		error = "File corrupted";

	fprintf(stderr, "%s: %s: Error: %s `%s': %s\n", app, archive, message, file, error);
	fflush(stderr);

}

void printVerbose(const char * archive, const char * message, const char * file) {

	printf("%s: %s: %s `%s' ...\n", app, archive, message, file);
	fflush(stdout);

}

void printMessage(const char * message, ...) {

	va_list ap;

	va_start(ap, message);
	vprintf(message, ap);
	va_end(ap);
	putchar('\n');
	fflush(stdout);

}
