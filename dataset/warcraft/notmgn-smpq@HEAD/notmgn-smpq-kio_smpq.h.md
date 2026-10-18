---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "kio_smpq.h"
ref: HEAD
---

/*
    kio_smpq.h - KDE4 KIO plugin for StormLib MPQ archiving utility
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

#ifndef KIO_SMPQ_H
#define KIO_SMPQ_H

#define KIO_SMPQ 0

#include <KIO/SlaveBase>
#include <KIO/FileJob>

struct SMPQSlavePrivate;

class SMPQSlave : public KIO::SlaveBase
{

	private:
		SMPQSlavePrivate * p;
		bool openArchive(const QString &archive, unsigned int flags = 0);
		void closeArchive();
		bool parseUrl(const KUrl &url, QString &fileName, QByteArray &archivePath);
		void toArchivePath(QByteArray &to, const QString &from);
		void fromArchivePath(QString &to, const QByteArray &from);
		void toFileTime(quint64 &to, const quint64 &from);
		bool fromFileTime(quint64 &to, const quint64 &from);

	public:
		SMPQSlave(const QByteArray &protocol, const QByteArray &pool_socket, const QByteArray &app_socket);
		virtual ~SMPQSlave();

//		virtual void openConnection();
//		virtual void closeConnection();

		virtual void get(const KUrl &url);
		virtual void put(const KUrl &url, int permissions, KIO::JobFlags flags);
		virtual void del(const KUrl &url, bool isfile);
		virtual void rename(const KUrl &src, const KUrl &dest, KIO::JobFlags flags);
		virtual void listDir(const KUrl &url);
		virtual void stat(const KUrl &url);
		virtual void mkdir(const KUrl &url, int permissions);

		virtual void slave_status();

		// KIO::FileJob interface
		virtual void open(const KUrl &url, QIODevice::OpenMode mode);
		virtual void close();
		virtual void read(KIO::filesize_t size);
		//virtual void write(const QByteArray &data);
		virtual void seek(KIO::filesize_t offset);

};

#endif //KIO_SMPQ_H
