---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "kio_smpq.cpp"
ref: HEAD
---

/*
    kio_smpq.cpp - KDE4 KIO plugin for StormLib MPQ archiving utility
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

#include <QCoreApplication>
#include <QDir>
#include <QFile>
#include <QFileInfo>
#include <QTemporaryFile>
#include <QByteArray>
#include <QVarLengthArray>
#include <QString>
#include <QStringList>
#include <QSet>
#include <QDateTime>

#include <KComponentData>
#include <KDebug>
#include <KMimeType>
#include <kde_file.h>

#include <StormLib.h>

#if !defined(WIN32) && !defined(_MSC_VER) && STORMLIB_VERSION > 0x091E
#define GetLastError SErrGetLastError
#endif

#include "kio_smpq.h"

#ifdef Q_OS_UNIX
#define LISTPATH "/usr/share/stormlib"
#else
#define LISTPATH KDEDIR "/share/stormlib"
#endif //Q_OS_UNIX

extern "C" {

int KDE_EXPORT kdemain(int argc, char * argv[]) {

	kDebug(KIO_SMPQ);

	QCoreApplication app(argc, argv);
	KComponentData componentData("kio_smpq");

	if ( argc != 4 ) {

		kDebug(KIO_SMPQ) << "Usage: kio_smpq protocol domain-socket1 domain-socket2";
		return -1;

	}

	SMPQSlave slave(argv[1], argv[2], argv[3]);
	slave.dispatchLoop();

	return 0;

}

}

struct SMPQSlavePrivate
{

	SMPQSlavePrivate() : SArchive(NULL), flags(0), SFile(NULL) { }

	HANDLE SArchive;
	QString archive;
	unsigned int flags;
	QDateTime modified;

	HANDLE SFile;
	QByteArray file;
	KUrl url;

};

SMPQSlave::SMPQSlave(const QByteArray &protocol, const QByteArray &pool_socket, const QByteArray &app_socket) : KIO::SlaveBase(protocol, pool_socket, app_socket) {

	kDebug(KIO_SMPQ);

	p = new SMPQSlavePrivate;

}

SMPQSlave::~SMPQSlave() {

	kDebug(KIO_SMPQ);

	delete p;

}

bool SMPQSlave::parseUrl(const KUrl &url, QString &fileName, QByteArray &archivePath) {

	kDebug(KIO_SMPQ);

	QString path = url.path();

	bool appended = false;

	if ( path.at(path.size() - 1) != KDIR_SEPARATOR ) {

		path.append(KDIR_SEPARATOR);
		appended = true;

	}

	int pos = 0;
	int nextPos = 0;

	while ( ( nextPos = path.indexOf(KDIR_SEPARATOR, pos + 1) ) != -1 ) {

		if ( ! QFileInfo(QFile::encodeName(path.left(nextPos))).exists() )
			break;

		pos = nextPos;

	}

	if ( pos == 0 )
		return false;

	if ( appended )
		path.chop(1);

	fileName = QFile::encodeName(path.left(pos));

	if ( ! QFileInfo(fileName).isFile() )
		return false;

	toArchivePath(archivePath, path.mid(pos+1, -1));

	return true;

}

bool SMPQSlave::openArchive(const QString &archive, unsigned int flags) {

	kDebug(KIO_SMPQ);

	if ( p->archive != archive || p->flags != flags || ! p->SArchive || QFileInfo(archive).lastModified() > p->modified ) {

		closeArchive();

		if ( archive.endsWith(".mpqe", Qt::CaseInsensitive) )
			flags |= STREAM_PROVIDER_MPQE;

		if ( ! SFileOpenArchive(archive.toUtf8(), 0, flags, &p->SArchive) )
			return false;

		p->archive = archive;
		p->flags = flags;
		p->modified = QFileInfo(archive).lastModified();

		QDir dir(LISTPATH);
		dir.setFilter(QDir::Files | QDir::Hidden);
		dir.setNameFilters(QStringList() << "*.txt" << "*.TXT");
		QStringList files = dir.entryList();

		for ( QStringList::Iterator it = files.begin(); it != files.end(); ++it )
			SFileAddListFile(p->SArchive, dir.absoluteFilePath(*it).toUtf8());

		SFileAddListFile(p->SArchive, NULL);

	}

	return true;

}

void SMPQSlave::closeArchive() {

	kDebug(KIO_SMPQ);

	if ( p->SArchive )
		SFileCloseArchive(p->SArchive);

	p->archive.clear();
	p->SArchive = NULL;
	p->flags = 0;

	p->file.clear();
	p->SFile = NULL;

}

void SMPQSlave::toArchivePath(QByteArray &to, const QString &from) {

#if KDIR_SEPARATOR == '\\'
	to = from.toUtf8();
#else
	to = from.toUtf8().replace(KDIR_SEPARATOR, '\\');
#endif // KDIR_SEPARATOR == '\\'

}

void SMPQSlave::fromArchivePath(QString &to, const QByteArray &from) {

#if KDIR_SEPARATOR == '\\'
	to = QString::fromUtf8(from);
#else
	to = QString::fromUtf8(from).replace('\\', KDIR_SEPARATOR);
#endif // KDIR_SEPARATOR == '\\'

}

#define OFFSET 116444736000000000ULL // Number of 100 ns units between 01/01/1601 and 01/01/1970
#define NSEC 10000000ULL // Convert 100 ns to sec

void SMPQSlave::toFileTime(quint64 &to, const quint64 &from) {

	if ( from == 0 )
		to = 0;
	else
		to = from * NSEC + OFFSET;

}

bool SMPQSlave::fromFileTime(quint64 &to, const quint64 &from) {

	if ( from < OFFSET )
		return false;

	to = ( from - OFFSET ) / NSEC;

	return true;

}

#undef OFFSET
#undef NSEC

///

//void SMPQSlave::openConnection() {}
//void SMPQSlave::closeConnection() {}

///

void SMPQSlave::get(const KUrl &url) {

	kDebug(KIO_SMPQ);

	QString fileName;
	QByteArray archivePath;

	if ( ! parseUrl(url, fileName, archivePath) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	if ( ! openArchive(fileName, STREAM_FLAG_READ_ONLY) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	HANDLE SFile = NULL;

	if ( ! SFileOpenFileEx(p->SArchive, archivePath, SFILE_OPEN_FROM_MPQ, &SFile) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	unsigned int low;
	unsigned int high;
	low = SFileGetFileSize(p->SFile, &high);

	totalSize(((KIO::filesize_t)high << 31) | low);

	bool eof = false;
	unsigned int bytes = 1024;
	QVarLengthArray <char> buffer(bytes);

	if ( QString::fromUtf8(archivePath).endsWith(".mpq", Qt::CaseInsensitive) )
		mimeType("application/x-mpq");
	else if ( QString::fromUtf8(archivePath).endsWith(".mpqe", Qt::CaseInsensitive) )
		mimeType("application/x-mpqe");
	else {

		SFileReadFile(SFile, buffer.data(), buffer.size(), &bytes, NULL);

		QByteArray fileData = QByteArray::fromRawData(buffer.data(), bytes);
		KMimeType::Ptr fileMimeType = KMimeType::findByNameAndContent(url.fileName(), fileData);
		mimeType(fileMimeType->name());

	}

	int dummy = 0;
	SFileSetFilePointer(SFile, 0, &dummy, FILE_BEGIN);

	buffer.resize(0x10000);

	KIO::filesize_t processedBytes = 0;

	while ( true ) {

		if ( ! SFileReadFile(SFile, buffer.data(), buffer.size(), &bytes, NULL) ) {

			eof = GetLastError() == ERROR_HANDLE_EOF;

			if ( ! eof ) {

				SFileCloseFile(SFile);
				error(KIO::ERR_COULD_NOT_READ, url.prettyUrl());
				return;

			}

		}

		processedBytes += bytes;

		data(QByteArray::fromRawData(buffer.data(), bytes));
		processedSize(processedBytes);

		if ( eof )
			break;

	}

	SFileCloseFile(SFile);

	data(QByteArray());
	finished();

}

void SMPQSlave::put(const KUrl &url, int, KIO::JobFlags flags) {

	kDebug(KIO_SMPQ);

	QString fileName;
	QByteArray archivePath;

	if ( ! parseUrl(url, fileName, archivePath) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	if ( ! openArchive(fileName) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	if ( archivePath.size() == 16 && archivePath.left(4) == "File" && archivePath.at(12) == '.' ) {

		error(KIO::ERR_WRITE_ACCESS_DENIED, url.prettyUrl());
		return;

	}

	if ( archivePath == "(listfile)" || archivePath == "(signature)" || archivePath == "(attributes)" || archivePath.contains("(patch_metadata)") ) {

		error(KIO::ERR_WRITE_ACCESS_DENIED, url.prettyUrl());
		return;

	}

	QTemporaryFile file;

	if ( ! file.open() ) {

		error(KIO::ERR_DISK_FULL, QString());
		return;

	}

	qint64 bytes;
	KIO::filesize_t totalBytes = 0;
	QByteArray buffer;

	while ( true ) {

		dataReq();
		bytes = readData(buffer);

		if ( bytes <= 0 )
			break;

		file.write(buffer);
		totalBytes += bytes;
		processedSize(totalBytes/2);

	}

	quint64 fileSize = file.size();

	quint64 SFileTime = 0;
	quint64 fileTime = 0;
	const QString metaDataTime = metaData("modified");

	if ( ! metaDataTime.isEmpty() )
		fileTime = QDateTime::fromString(metaDataTime, Qt::ISODate).toTime_t();

	toFileTime(SFileTime, fileTime);

	unsigned int SFlags = MPQ_FILE_COMPRESS;

	if ( flags & KIO::Overwrite )
		SFlags |= MPQ_FILE_REPLACEEXISTING;

	HANDLE SFile;

	if ( ! SFileCreateFile(p->SArchive, archivePath, SFileTime, fileSize, 0, SFlags, &SFile) ) {

		if ( GetLastError() == ERROR_ALREADY_EXISTS )
			error(KIO::ERR_FILE_ALREADY_EXIST, url.prettyUrl());
		else
			error(KIO::ERR_COULD_NOT_WRITE, url.prettyUrl());

		return;

	}

	file.seek(0);
	totalBytes = 0;

	while ( ( buffer = file.read(0x10000) ).size() > 0 ) {

		if ( ! SFileWriteFile(SFile, buffer, buffer.size(), MPQ_COMPRESSION_ZLIB) ) {

			error(KIO::ERR_COULD_NOT_WRITE, url.prettyUrl());
			return;

		}

		totalBytes += buffer.size();
		processedSize(fileSize/2+totalBytes/2);

	}

	processedSize(fileSize);

	SFileFinishFile(SFile);
	SFileFlushArchive(p->SArchive);

	p->modified = QFileInfo(p->archive).lastModified();

	finished();

}

void SMPQSlave::del(const KUrl &url, bool isfile) {

	kDebug(KIO_SMPQ);

	QString fileName;
	QByteArray archivePath;

	if ( ! parseUrl(url, fileName, archivePath) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	if ( ! openArchive(fileName) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	// Skip internal files in MPQ archive
	if ( archivePath == "(listfile)" || archivePath == "(signature)" || archivePath == "(attributes)" || archivePath.contains("(patch_metadata)") ) {

		error(KIO::ERR_WRITE_ACCESS_DENIED, url.prettyUrl());
		return;

	}

	if ( ! isfile ) {

		QByteArray mask = archivePath;

		if ( mask.at(mask.size()-1) != '\\' )
			mask.append("\\*");
		else
			mask.append("*");

		SFILE_FIND_DATA SFileFindData;
		HANDLE SFileFind = SFileFindFirstFile(p->SArchive, mask, &SFileFindData, NULL);

		// MPQ archives does not support directory structure
		// There are no files in this directory, so directory is empty
		// Only simulate deleting directory
		if ( ! SFileFind ) {

			finished();
			return;

		}

		SFileFindClose(SFileFind);

		// TODO: KIO::ERR_COULD_NOT_RMDIR or KIO::ERR_CANNOT_DELETE ?
		error(KIO::ERR_CANNOT_DELETE, url.prettyUrl());
		return;

	}

	if ( ! SFileRemoveFile(p->SArchive, archivePath, SFILE_OPEN_FROM_MPQ) ) {

		error(KIO::ERR_CANNOT_DELETE, url.prettyUrl());
		return;

	}

	SFileCompactArchive(p->SArchive, NULL, 0);
	SFileFlushArchive(p->SArchive);

	p->modified = QFileInfo(p->archive).lastModified();

	finished();

}

void SMPQSlave::rename(const KUrl &src, const KUrl &dest, KIO::JobFlags flags) {

	kDebug(KIO_SMPQ);

	QString srcFileName;
	QByteArray srcArchivePath;

	QString destFileName;
	QByteArray destArchivePath;

	if ( ! parseUrl(src, srcFileName, srcArchivePath) ) {

		error(KIO::ERR_DOES_NOT_EXIST, src.prettyUrl());
		return;

	}

	if ( ! parseUrl(dest, destFileName, destArchivePath) ) {

		error(KIO::ERR_DOES_NOT_EXIST, dest.prettyUrl());
		return;

	}

	if ( srcFileName != destFileName ) {

		error(KIO::ERR_UNSUPPORTED_ACTION, QString());
		return;

	}

	if ( srcArchivePath.isEmpty() || srcArchivePath.at(srcArchivePath.size() - 1) == '\\' ) {

		error(KIO::ERR_UNSUPPORTED_ACTION, QString());
		return;

	}

	if ( ! openArchive(srcFileName) ) {

		error(KIO::ERR_DOES_NOT_EXIST, src.prettyUrl());
		return;

	}

	// Skip internal files in MPQ archive
	if ( destArchivePath == "(listfile)" || destArchivePath == "(signature)" || destArchivePath == "(attributes)" || destArchivePath.contains("(patch_metadata)") ) {

		error(KIO::ERR_DOES_NOT_EXIST, dest.prettyUrl());
		return;

	}

	HANDLE SFile;
	bool found;

	if ( ( found = SFileOpenFileEx(p->SArchive, destArchivePath, SFILE_OPEN_FROM_MPQ, &SFile) ) )
		SFileCloseFile(SFile);

	if ( found && ! ( flags & KIO::Overwrite ) ) {

		error(KIO::ERR_FILE_ALREADY_EXIST, dest.prettyUrl());
		return;

	}

	if ( found && ( flags & KIO::Overwrite ) ) {

		if ( ! SFileRemoveFile(p->SArchive, destArchivePath, SFILE_OPEN_FROM_MPQ) ) {

			error(KIO::ERR_CANNOT_RENAME, src.prettyUrl());
			return;

		}

		SFileCompactArchive(p->SArchive, NULL, 0);
		SFileFlushArchive(p->SArchive);

		p->modified = QFileInfo(p->archive).lastModified();

	}

	if ( ! SFileRenameFile(p->SArchive, srcArchivePath, destArchivePath) ) {

		error(KIO::ERR_CANNOT_RENAME, src.prettyUrl());
		return;

	}

	SFileFlushArchive(p->SArchive);

	p->modified = QFileInfo(p->archive).lastModified();

	finished();

}

void SMPQSlave::listDir(const KUrl &url) {

	kDebug(KIO_SMPQ);

	QString fileName;
	QByteArray archivePath;

	if ( ! parseUrl(url, fileName, archivePath) ) {

		if ( QFileInfo(url.path()).exists() ) {

			redirection(KUrl(url.path()));
			finished();
			return;

		}

		error(KIO::ERR_CANNOT_ENTER_DIRECTORY, url.prettyUrl());
		return;

	}

	if ( ! archivePath.isEmpty() && archivePath.at(archivePath.size() - 1) != '\\' )
		archivePath.append('\\');

	if ( ! openArchive(fileName) ) {

		error(KIO::ERR_CANNOT_ENTER_DIRECTORY, url.prettyUrl());
		return;

	}

	if ( archivePath.isEmpty() ) {

		KIO::UDSEntry entry;
		entry.insert(KIO::UDSEntry::UDS_NAME, ".");
		entry.insert(KIO::UDSEntry::UDS_FILE_TYPE, S_IFDIR);
		entry.insert(KIO::UDSEntry::UDS_ACCESS, (S_IRWXU | S_IRWXG | S_IRWXO));
		listEntry(entry, false);

	}

	QSet <QByteArray> directories;

	SFILE_FIND_DATA SFileFindData;
	HANDLE SFileFind = SFileFindFirstFile(p->SArchive, archivePath + '*', &SFileFindData, NULL);

	if ( ! SFileFind ) {

		error(KIO::ERR_CANNOT_ENTER_DIRECTORY, url.prettyUrl());
		return;

	}

	while ( true ) {

		QByteArray filePath = SFileFindData.cFileName;
		QByteArray fileName;

		if ( archivePath.isEmpty() )
			fileName = filePath;
		else
			fileName = filePath.mid(archivePath.size(), -1);

		if ( fileName.contains('\\') ) {

			QByteArray dirName = fileName.split('\\').first();

			if ( ! directories.contains(dirName) ) {

				KIO::UDSEntry entry;
				entry.insert(KIO::UDSEntry::UDS_NAME, QFile::decodeName(dirName));
				entry.insert(KIO::UDSEntry::UDS_FILE_TYPE, S_IFDIR);
				entry.insert(KIO::UDSEntry::UDS_ACCESS, (S_IRWXU | S_IRWXG | S_IRWXO));
				listEntry(entry, false);

				directories.insert(dirName);

			}

		} else {

			quint64 fileTime = 0;
			quint64 SFileTime = SFileFindData.dwFileTimeLo | ( (quint64)SFileFindData.dwFileTimeHi << 32 );

			fromFileTime(fileTime, SFileTime);

			KIO::UDSEntry entry;
			entry.insert(KIO::UDSEntry::UDS_NAME, QFile::decodeName(fileName));
			entry.insert(KIO::UDSEntry::UDS_FILE_TYPE, S_IFREG);
			entry.insert(KIO::UDSEntry::UDS_SIZE, SFileFindData.dwFileSize);
			entry.insert(KIO::UDSEntry::UDS_MODIFICATION_TIME, fileTime);
			entry.insert(KIO::UDSEntry::UDS_ACCESS, (S_IRWXU | S_IRWXG | S_IRWXO));

			if ( QFile::decodeName(fileName).endsWith(".mpq", Qt::CaseInsensitive) )
				entry.insert(KIO::UDSEntry::UDS_MIME_TYPE, "application/x-mpq");
			else if ( QFile::decodeName(fileName).endsWith(".mpqe", Qt::CaseInsensitive) )
				entry.insert(KIO::UDSEntry::UDS_MIME_TYPE, "application/x-mpqe");

			listEntry(entry, false);

		}

		if ( ! SFileFindNextFile(SFileFind, &SFileFindData) )
			break;

	}

	SFileFindClose(SFileFind);

	listEntry(KIO::UDSEntry(), true);
	finished();

}


void SMPQSlave::stat(const KUrl &url) {

	kDebug(KIO_SMPQ);

	QString fileName;
	QByteArray archivePath;

	if ( ! parseUrl(url, fileName, archivePath) ) {

		if ( QFileInfo(url.path()).exists() ) {

			redirection(KUrl(url.path()));
			finished();
			return;

		}

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	if ( archivePath.isEmpty() )
		archivePath = "*";

	if ( archivePath.at(archivePath.size() - 1) == '\\' )
		archivePath.append('*');

	if ( ! openArchive(fileName) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	SFILE_FIND_DATA SFileFindData;
	HANDLE SFileFind;
	HANDLE SFile;
	bool found = false;
	bool dir = false;

	if ( ! found ) {

		SFileFind = SFileFindFirstFile(p->SArchive, archivePath, &SFileFindData, NULL);

		if ( SFileFind ) {

			found = true;
			dir = false;
			SFileFindClose(SFileFind);

		}

	}

	if ( ! found ) {

		QByteArray mask = archivePath;

		if ( mask.at(mask.size()-1) != '\\' )
			mask.append("\\*");
		else
			mask.append("*");

		SFileFind = SFileFindFirstFile(p->SArchive, mask, &SFileFindData, NULL);

		if ( SFileFind ) {

			found = true;
			dir = true;
			SFileFindClose(SFileFind);

		}

	}

	if ( ! found ) {

		if ( SFileOpenFileEx(p->SArchive, archivePath, SFILE_OPEN_FROM_MPQ, &SFile) ) {

			found = true;
			dir = false;

			SFileFindData.dwFileTimeLo = 0;
			SFileFindData.dwFileTimeHi = 0;

			unsigned int high = 0;
			unsigned int low = SFileGetFileSize(SFile, &high);

			SFileFindData.dwFileSize = low | ( (quint64)high << 32 );

			SFileCloseFile(SFile);

		}

	}

	if ( ! found ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	KIO::UDSEntry entry;
	entry.insert(KIO::UDSEntry::UDS_NAME, url.path());
	entry.insert(KIO::UDSEntry::UDS_ACCESS, (S_IRWXU | S_IRWXG | S_IRWXO));

	if ( ! dir ) {

		quint64 fileTime = 0;
		quint64 SFileTime = SFileFindData.dwFileTimeLo | ( (quint64)SFileFindData.dwFileTimeHi << 32 );

		fromFileTime(fileTime, SFileTime);

		entry.insert(KIO::UDSEntry::UDS_FILE_TYPE, S_IFREG);
		entry.insert(KIO::UDSEntry::UDS_SIZE, SFileFindData.dwFileSize);
		entry.insert(KIO::UDSEntry::UDS_MODIFICATION_TIME, fileTime);

		if ( QString::fromUtf8(archivePath).endsWith(".mpq", Qt::CaseInsensitive) )
			entry.insert(KIO::UDSEntry::UDS_MIME_TYPE, "application/x-mpq");
		else if ( QString::fromUtf8(archivePath).endsWith(".mpqe", Qt::CaseInsensitive) )
			entry.insert(KIO::UDSEntry::UDS_MIME_TYPE, "application/x-mpqe");

	} else {

		entry.insert(KIO::UDSEntry::UDS_FILE_TYPE, S_IFDIR);

	}

	statEntry(entry);
	finished();

}

void SMPQSlave::mkdir(const KUrl &, int) { 

	kDebug(KIO_SMPQ);

	// MPQ archives does not support directory structure
	// Only simulate creating directory

	finished();

}

///

void SMPQSlave::slave_status() {

	kDebug(KIO_SMPQ);

	slaveStatus(QString(), (bool)p->SArchive);

}

///
// KIO::FileJob interface

void SMPQSlave::open(const KUrl &url, QIODevice::OpenMode mode) {

	kDebug(KIO_SMPQ);

	QString fileName;
	QByteArray archivePath;

	if ( ! parseUrl(url, fileName, archivePath) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	unsigned int flags = 0;

	// 0 - read only
	// 1 - write only
	// 2 - append
	// 3 - read + write
	unsigned int myMode = -1;

	if ( mode & QIODevice::ReadWrite )
		myMode = 3;
	else if ( mode & QIODevice::Append )
		myMode = 2;
	else if ( mode & QIODevice::ReadOnly )
		myMode = 0;
	else if ( mode & QIODevice::WriteOnly )
		myMode = 1;

	if ( myMode == 0 ) {

		flags |= STREAM_FLAG_READ_ONLY;

	} else if ( myMode == 1 ) {

		// TODO: Add support for write only mode
		error(KIO::ERR_CANNOT_OPEN_FOR_WRITING, url.prettyUrl());
		return;

	} else if ( myMode == 2 ) {

		// TODO: Add support for append mode
		error(KIO::ERR_CANNOT_OPEN_FOR_WRITING, url.prettyUrl());
		return;

	} else if ( myMode == 3 ) {

		// TODO: Add support for read + write mode
		// Currently read mode is used
		flags |= STREAM_FLAG_READ_ONLY;

	} else {

		error(KIO::ERR_UNSUPPORTED_ACTION, url.prettyUrl());
		return;

	}

	if ( ! openArchive(fileName, flags) ) {

		error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
		return;

	}

	// Skip internal files in MPQ archive
	if ( myMode != 0 ) {

		if ( archivePath == "(listfile)" || archivePath == "(signature)" || archivePath == "(attributes)" || archivePath.contains("(patch_metadata)") ||
			( archivePath.size() == 16 && archivePath.left(4) == "File" && archivePath.at(12) == '.' ) ) {

			error(KIO::ERR_WRITE_ACCESS_DENIED, url.prettyUrl());
			return;

		}

	}

	if ( myMode == 0 ) {

		if ( ! SFileOpenFileEx(p->SArchive, archivePath, SFILE_OPEN_FROM_MPQ, &p->SFile) ) {

			error(KIO::ERR_DOES_NOT_EXIST, url.prettyUrl());
			return;

		}

	}

	// TODO: open file for writing

	p->file = archivePath;
	p->url = url;

	if ( myMode == 0 || myMode == 3 ) {

		if ( QString::fromUtf8(archivePath).endsWith(".mpq", Qt::CaseInsensitive) )
			mimeType("application/x-mpq");
		else if ( QString::fromUtf8(archivePath).endsWith(".mpqe", Qt::CaseInsensitive) )
			mimeType("application/x-mpqe");
		else {

			unsigned int bytes = 1024;
			QVarLengthArray <char> buffer(bytes);

			SFileReadFile(p->SFile, buffer.data(), buffer.size(), &bytes, NULL);

			QByteArray fileData = QByteArray::fromRawData(buffer.data(), bytes);
			KMimeType::Ptr fileMimeType = KMimeType::findByNameAndContent(url.fileName(), fileData);
			mimeType(fileMimeType->name());

			int dummy = 0;
			SFileSetFilePointer(p->SFile, 0, &dummy, FILE_BEGIN);

		}

	}

	unsigned int low;
	unsigned int high;
	low = SFileGetFileSize(p->SFile, &high);

	totalSize(((KIO::filesize_t)high << 31) | low);
	position(0);
	opened();

}

void SMPQSlave::close() {

	kDebug(KIO_SMPQ);

	SFileCloseFile(p->SFile);

	if ( p->SFile )
		p->SFile = NULL;

	p->file.clear();
	p->url.clear();

}

void SMPQSlave::read(KIO::filesize_t size) {

	kDebug(KIO_SMPQ);

	bool eof = false;
	unsigned int bytes = size;
	QVarLengthArray <char> buffer(bytes);

	if ( ! SFileReadFile(p->SFile, buffer.data(), buffer.size(), &bytes, NULL) ) {

		eof = GetLastError() == ERROR_HANDLE_EOF;

		if ( ! eof ) {

			close();
			error(KIO::ERR_COULD_NOT_READ, p->url.prettyUrl());
			return;

		}

	}

	data(QByteArray::fromRawData(buffer.data(), bytes));

	if ( eof )
		data(QByteArray());

}

/*void SMPQSlave::write(const QByteArray &data) {

	kDebug(KIO_SMPQ);

	// TODO

}*/

void SMPQSlave::seek(KIO::filesize_t offset) {

	kDebug(KIO_SMPQ);

	int low = offset & ( ( 1ULL << 32 ) - 1 );
	int high = offset >> 32;

	unsigned int ret;

	if ( ( ret = SFileSetFilePointer(p->SFile, low, &high, FILE_BEGIN) ) == SFILE_INVALID_SIZE ) {

		error(KIO::ERR_COULD_NOT_SEEK, p->url.prettyUrl());
		return;

	}

	position(((KIO::filesize_t)high << 32) | ret);

}
