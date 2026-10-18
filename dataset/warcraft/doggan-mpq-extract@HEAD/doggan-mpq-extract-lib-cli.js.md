---
type: vendor-doc
source_repo: "https://github.com/doggan/mpq-extract"
source_path: "lib/cli.js"
ref: HEAD
---

#! /usr/bin/env node
'use strict';

var program = require('commander'),
    packageJSON = require('../package.json'),
    app = require('./index.js');

function parseArgs() {
    program
        .version(packageJSON.version)
        .usage('[options] <mpq> <listfile>')
        .option('-o, --dest <path>', 'the folder to extract files to (default: current directory)')
        .option('-f, --fullpath', 'preserve the full path hierarchy of the extracted files.')
        .option('-c, --lowercase', 'convert extracted file paths to lowercase.')
        .option('-v, --verbose', 'verbose output')
        .parse(process.argv);

    if (process.argv.length < 3) {
        console.error('Error: No MPQ file specified.');
        program.help();
    }
    if (process.argv.length < 4) {
        console.error('Error: No listfile specified.');
        program.help();
    }

    return {
        mpqPath: process.argv[2],
        listPath: process.argv[3],
        outPath: program.dest || './',
        useFullPath: program.fullpath || false,
        toLowercase: program.lowercase || false,
        verbose: program.verbose || false,
    };
}

var Spinner = require('cli-spinner').Spinner;
var spinner = new Spinner('%s');
spinner.start();

var options = parseArgs();

console.log('Extracting from: ' + options.mpqPath);
var hrstart = process.hrtime();
app.extract(options, function(results) {
    spinner.stop(true);

    var hrend = process.hrtime(hrstart);
    console.log('Extraction completed.');
    console.log('...Extracted: ' + results.extractedCount);
    console.log('...Failed   : ' + results.failedCount);
    console.log("...Time     : %ds", (hrend[0] + (hrend[1] / 1000000000)).toFixed(2));
});
