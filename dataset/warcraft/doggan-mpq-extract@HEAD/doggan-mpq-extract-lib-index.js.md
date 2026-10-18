---
type: vendor-doc
source_repo: "https://github.com/doggan/mpq-extract"
source_path: "lib/index.js"
ref: HEAD
---

'use strict';

var mpq = require('mech-mpq'),
    fs = require('fs'),
    path = require('path'),
    mkdirp = require('mkdirp'),
    async = require('async'),
    map = require('lodash.map');

var RESULTS;

function extractFile(archive, fileName) {
    var file = archive.openFile(fileName);
    if (!file) {
        return null;
    }

    var fileContents = file.read();
    file.close();
    return fileContents;
}

function writeFile(options, buffer, fileName, onFinished) {
    if (options.useFullPath) {
        fileName = fileName.replace(/\\/g, '/');
    } else {
        var parts = fileName.split("\\");
        if (parts.length > 1) {
            fileName = parts[parts.length - 1];
        }
    }
    if (options.toLowercase) {
        fileName = fileName.toLowerCase();
    }

    var outPath = path.join(options.outPath, fileName);
    mkdirp(path.dirname(outPath), function() {
        fs.writeFile(outPath, buffer, function(err) {
            if (err) {
                console.error('Unable to write: ' + outPath);
            }
            onFinished();
        });
    });
}

function extractAndWriteFiles(options, archive, fileNames, onFinished) {
    var buildWorker = function(archive, fileName) {
        return function(cb) {
            var contents = extractFile(archive, fileName);
            if (contents === null) {
                RESULTS.failedCount++;
                if (options.verbose) {
                    console.error('Failed to extract: ' + fileName);
                }
                cb();
            } else {
                RESULTS.extractedCount++;
                writeFile(options, contents, fileName, cb);
            }
        };
    };

    var workers = [];
    for (var i in fileNames) {
        workers.push(buildWorker(archive, fileNames[i]));
    }

    async.parallel(workers, function() {
        onFinished();
    });
}

/**
 * Extract a list of files from an MPQ archive.
 */
exports.extract = function(options, onFinished) {
    var archive = mpq.openArchive(options.mpqPath);

    var lines = fs.readFileSync(options.listPath).toString().split('\n');
    lines = map(lines, function(str) {
        return str.trim();
    });

    RESULTS = {
        extractedCount: 0,
        failedCount: 0
    };

    extractAndWriteFiles(options, archive, lines, function() {
        if (onFinished) {
            onFinished(RESULTS);
        }
    });
};
