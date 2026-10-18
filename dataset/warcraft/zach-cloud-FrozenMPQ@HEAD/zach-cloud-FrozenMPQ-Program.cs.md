---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/FrozenMPQ"
source_path: "Program.cs"
ref: HEAD
---

﻿using System;
using System.IO;
using StormLibSharp;

namespace FrozenMPQ
{
    /**
     * Frozen's CLI mpq editor
     */
    class FMPQ
    {

        /**
        * Runs the program
        */
        private void Run(String[] args)
        {
            // Verify argument count
            if(!CheckArguments(args))
            {
                return;
            }
            // Retrieve argument values and listfile, and check for existence
            String operationName = args[0];
            String mpqName = args[1];
            String listfileName = "listfile.txt";
            if (!File.Exists(mpqName))
            {
                Console.WriteLine("MPQ file does not exist: " + mpqName);
                return;
            }
            if(!File.Exists(listfileName))
            {
                Console.WriteLine("Listfile does not exist: " + listfileName);
                return;
            }
            // Determine what operation the user wanted
            Boolean extract;
            if (operationName.Equals("ext") || 
                operationName.Equals("exp") || 
                operationName.Equals("export") || 
                operationName.Equals("extract"))
            {
                extract = true;
            }
            else if (operationName.Equals("imp") || operationName.Equals("import"))
            {
                extract = false;
            }
            else
            {
                Console.WriteLine("Invalid operation: " + operationName);
                return;
            }
            // For every entered file, we either perform import/export
            using (MpqArchive archive = new MpqArchive(mpqName, FileAccess.ReadWrite))
            {
                // Add listfile.txt as external listfile
                int retval = archive.AddListFile(listfileName);
                Console.WriteLine("Added listfile: " + retval + " (0 is ok)");
                for (int i = 2; i < args.Length; i++)
                {
                    if (extract)
                    {
                        Extract(args[i], archive);
                    }
                    else
                    {
                        Import(args[i], archive);
                    }
                }
                Console.WriteLine("All desired operations completed. Flushing & closing.");
                // Close out resources
                archive.Compact(listfileName);
                archive.Flush();
                archive.Dispose();
            }
        }

        /**
         * Extracts file to disk
         * Overwrites disk file if needed
         * It extrats to "out/" folder
         */
        private void Extract(String filename, MpqArchive archive)
        {
            try
            {
                Console.WriteLine("Extracting file: " + filename);
                if (!archive.HasFile(filename))
                {
                    Console.WriteLine("File did not exist in MPQ archive: " + filename + " skipping. . .");
                    return;
                }
                string destination = filename.Substring(1 + filename.LastIndexOf("\\"));
                archive.ExtractFile(filename, destination);
                if (File.Exists("out/" + filename))
                {
                    File.Delete("out/" + filename);
                    Console.WriteLine("Overwrote existing file...");
                }
                System.IO.Directory.CreateDirectory("out/" + filename);
                System.IO.Directory.Delete("out/" + filename);
                if(!File.Exists(destination))
                {
                    Console.WriteLine("Expected that extracted file exists, but extraction failed!");
                    return;
                }
                File.Copy(destination, "out/" + filename);
                if(!File.Exists("out/" + filename))
                {
                    Console.WriteLine("Expected that copied file exists, but copy failed!");
                    return;
                }
                File.Delete(destination);
                if(File.Exists(filename))
                {
                    Console.WriteLine("Failed to delete origin file!");
                    return;
                }
                Console.WriteLine("Extracted successfully to: " + filename);
            } catch(Exception ex)
            {
                Console.WriteLine("Failed to extract file " + filename + ": " + ex.Message);
            }

        }

        /**
         * Imports a file to the archive.
         * Deletes/imports to overwrite.
         * It imports from "in/" folder
         */
        private void Import(String filename, MpqArchive archive)
        {
            try
            {
                Console.WriteLine("Importing file: in/" + filename);
                if (!File.Exists("in/" + filename))
                {
                    Console.WriteLine("File does not exist: in/" + filename + " skipping. . .");
                    return;
                }
                if (archive.HasFile(filename))
                {
                    // We need to delete because stormlib doesn't overwrite
                    Console.WriteLine("File already exists, deleting");
                    archive.DeleteFile(filename);
                }
                Console.WriteLine("Inserting file");
                archive.AddFileFromDisk("in/" + filename, filename);
                if(!archive.HasFile(filename))
                {
                    Console.WriteLine("Expected import file exists, but did not!");
                    return;
                }
                Console.WriteLine("Successfully imported in/" + filename + " as " + filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to import file " + filename + ": " + ex.Message);
            }
        }

        /**
         * Checks for argument validity.
         * Expects 3 or more arguments.
         */
        private Boolean CheckArguments(String[] args)
        {
            Console.WriteLine("Received " + args.Length + " arguments");
            if (args.Length <= 2)
            {
                // Invalid argument count, display details to user.
                Console.WriteLine("Usage: frozenmpq.exe <actionType> <fileName> <file1> <file2> ... <etc>");
                Console.WriteLine("Action type: either \"ext\" (extract) or \"imp\" (import)");
                Console.WriteLine("Filename: relative filename to MPQ file, including extension");
                Console.WriteLine("Files: A series of files to either export or import. For importing, path should be relative");
                Console.WriteLine("Extract action will extract requested files to \"out/\" directory");
                Console.WriteLine("Import action will import the requested files to the MPQ, overwriting.");
                Console.WriteLine("The file should exist in the \"in\" directory and be the same name as desired in the MPQ");
                Console.WriteLine("This application expects listfile.txt to exist in the same directory.");
                Console.WriteLine("EXAMPLE: frozenmpq.exe myMap.w3x ext scripts/war3map.j war3map.j war3map.w3o");
                Console.WriteLine("Don't include any spaces!");
                return false;
            }
            return true;
        }

        /**
         * Program entry point
         */
        static void Main(string[] args)
        {
            new FMPQ().Run(args);
        }
    }
}
