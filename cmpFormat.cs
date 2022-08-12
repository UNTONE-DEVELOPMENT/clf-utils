using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace clf
{
    namespace cmp
    {
        [System.Serializable]
        public struct cmpFile
        {
            public string folderPath;
            public int netId;
            public string name;
            public string description;
            public string longDescription;
            public string creator;
            public string mapper;
            public string backgroundPath;
            public int soundtrackId;
            public string[] maps;
        }

        public static class cmpUtils
        {
            public static cmpFile initCmp()
            {
                cmpFile file = new cmpFile();
                file.folderPath = "-1 unknown"; // do NOT change this directly. saveCmpFIle() changes it for you
                file.netId = -1;
                file.name = "New Mappack";
                file.description = "";
                file.longDescription = "";
                file.creator = "";
                file.mapper = "";
                file.backgroundPath = "";
                file.soundtrackId = 0;
                file.maps = new string[0];
                return file;
            }

            public static string BaseSave(cmpFile file)
            {
                string content = "";
                content += "CMP 1.0\n\n";
                content += "[General]\n";
                // general content
                content += "id = " + file.netId + "\n";
                content += "name = " + file.name + "\n";
                content += "description = " + file.description + "\n";
                content += "longDescription = " + file.longDescription + "\n";
                content += "creator = " + file.creator + "\n";
                content += "mapper = " + file.mapper + "\n";
                content += "background = " + file.backgroundPath + "\n";
                content += "sountrack = " + file.soundtrackId + "\n";
                content += "\n[Maps]\n";
                // maps content//
                content += "maps = " + String.Join(",", file.maps);
                return content;
            }

            // this needs to be done in cubey itself, can't be here, needs to pull cubey variables
            // public static cmpFile saveMappack(cmpFile file)
            // {
            //     string old_location = file.folderPath;
            //     file.folderPath = file.netId + " " + file.name;
            // 
            // 
            // 
            //     return file;
            // }

            public static cmpFile loadCmpFromFile(string path)
            {
                return loadCmp(File.ReadAllLines(path), path);
            }

            public static cmpFile loadCmp(string[] data, string path)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

                cmpFile file = new cmpFile();
                string[] general = getSection("General", data);
                foreach (string x in general)
                {
                    string[] line = readCmpLine(x);
                    switch (line[0])
                    {
                        case "id":
                            file.netId = int.Parse(line[1]);
                            break;
                        case "name":
                            file.name = line[1];
                            break;
                        case "description":
                            file.description = line[1];
                            break;
                        case "longDescription":
                            file.longDescription = line[1];
                            break;
                        case "creator":
                            file.creator = line[1];
                            break;
                        case "mapper":
                            file.mapper = line[1];
                            break;
                        case "background":
                            file.backgroundPath = line[1];
                            break;
                        case "soundtrack":
                            file.soundtrackId = int.Parse(line[1]);
                            break;
                        default:
                            break;
                    }
                }
                file.folderPath = path.Substring(0, path.LastIndexOf("/") + 1);


                string[] maps = getSection("Maps", data);
                Debug.Log("attempting to split " + maps[0]);
                string[] allMaps = readCmpLine(maps[0])[1].Split(',');
                file.maps = allMaps;

                return file;
            }

            public static string[] readCmpLine(string x)
            {
                return clfUtils.readClfLine(x); // can reuse
            }

            public static string[] getSection(string section, string[] data)
            {
                return clfUtils.getSection(section, data); // we can reuse the clf func here
            }
        }
    }
}