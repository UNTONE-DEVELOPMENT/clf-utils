using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace clf
{
    public class segments
    {
        public class general
        {
            public string name;
            public string description;
            public string creator;
            public string tags;

            public general(string name, string description, string creator, string tags = "")
            {
                this.name = name;
                this.description = description;
                this.creator = creator;
                this.tags = tags;
            }
        }

        // BLOCKID,XPOS,YPOS,ZPOS,XSCALE,YSCALE,ROTATION,RED,GREEN,BLUE...
        public struct blocks
        {
            public objects.block[] blockList;
        }

        public struct visual
        {
            public string bgType;
            public Color32 colour1; // used in solid & as first colour in gradient
            public Color32 colour2; // second colour in gradient
            public Texture2D sprite;
        }
    }

    public class objects
    {
        public struct block
        {
            public int block_id;
            public int data_id;
            public Vector3 position;
            public Vector2 scale;
            public float rotation;
            public Color32 colour;
        }
        // public class block
        // {
        //     public int block_id;
        //     public int data_id;
        //     public Vector3 position;
        //     public Vector2 scale;
        //     public float rotation;
        //     public Color32 colour;
        // 
        //     public block(int block_id , int data_id, Vector3 position , Vector2 scale, float rotation , Color32 colour)
        //     {
        //         this.block_id = block_id;
        //         this.data_id = data_id;
        //         this.position = position;
        //         this.scale = scale;
        //         this.rotation = rotation;
        //         this.colour = colour;
        //     }
        // }
    }

    public class clfFile
    {
        public string path;
        public string raw;
        public segments.general general;
        public segments.blocks blocks;
        public segments.visual visual;
    }

    public static class clfUtils
    {
        public static clfFile loadDynamic(string[] clf)
        {
            if(clf[0].StartsWith("CLF 0.3"))
            {
                return convertLegacy(clf);
            }
            else
            {
                return loadClfFromString(clf);
            }
        }

        public static clfFile loadClfFromFile(string path)
        {
            return loadClfFromString(File.ReadAllLines(path));
        }

        public static clfFile loadDynamicFromPath(string path)
        {
            return loadDynamic(File.ReadAllLines(path));
        }

        public static clfFile convertLegacyFromFile(string path)
        {
            return convertLegacy(File.ReadAllLines(path));
        }

        public static clfFile convertLegacy(string[] clf)
        {
            Debug.Log("starting conversion...");

            var watch = new System.Diagnostics.Stopwatch();

            string[] meta = getSection("META", clf);
            clfFile file = newClfFile();

            foreach (string x in meta)
            {
                string[] line = readClfLegacyLine(x);
                if (line[0] == "name")
                {
                    file.general.name = line[1];
                }
                if (line[0] == "creator")
                {
                    file.general.creator = line[1];
                }
            }

            file.general.description = "Converted from CLF 1.0";

            string[] blocks = getSection("LEVEL", clf);
            file.blocks = new segments.blocks();
            file.blocks.blockList = new objects.block[blocks.Length];

            for (int i = 0; i < blocks.Length; i++)
            {
                // BLOCKID:DATAID,XPOS,YPOS,ZPOS,XSCALE,YSCALE,ROTATION,RED,GREEN,BLUE,ALPHA...

                string[] split = blocks[i].Split(',');

                int block_id;
                int data_id;

                switch (int.Parse(split[3])) 
                {
                    case 0:
                        block_id = 0;
                        data_id = 0;
                        break;
                    case 1:
                        block_id = 1;
                        data_id = 0;
                        Debug.Log("just got a key at " + split[0] + ":" + split[1]);
                        break;
                    case 2:
                        block_id = 2;
                        data_id = 0;
                        break;
                    case 3:
                        block_id = 3;
                        data_id = 0;
                        break;
                    case 4:
                        block_id = 4;
                        data_id = 0;
                        break;
                    case 5:
                        block_id = 4;
                        data_id = 1;
                        break;
                    case 6:
                        block_id = 16;
                        data_id = 0;
                        break;
                    case 7:
                        block_id = 5;
                        data_id = 0;
                        break;
                    case 16:
                        block_id = 6;
                        data_id = 0;
                        break;
                    case 17:
                        block_id = 7;
                        data_id = 0;
                        break;
                    case 18:
                        block_id = 5;
                        data_id = 1;
                        break;
                    case 19:
                        block_id = 8;
                        data_id = 0;
                        break;
                    case 20:
                        block_id = 0;
                        data_id = 0;
                        break;
                    case 21:
                        block_id = 9;
                        data_id = 0;
                        break;
                    case 22:
                        block_id = 10;
                        data_id = 0;
                        break;
                    case 23:
                        block_id = 11;
                        data_id = 0;
                        break;
                    case 24:
                        block_id = 12;
                        data_id = 0;
                        break;
                    case 25:
                        block_id = 11;
                        data_id = 1;
                        break;
                    case 26:
                        block_id = 12;
                        data_id = 1;
                        break;
                    case 27:
                        block_id = 11;
                        data_id = 2;
                        break;
                    case 28:
                        block_id = 12;
                        data_id = 2;
                        break;
                    case 29:
                        block_id = 13;
                        data_id = 0;
                        break;
                    case 33:
                        block_id = 17;
                        data_id = 0;
                        break;
                    case 34:
                        block_id = 14;
                        data_id = 0;
                        break;
                    case 35:
                        block_id = 15;
                        data_id = 0;
                        break;
                    default:
                        block_id = 0;
                        data_id = 0;
                        break;
                }


                if (int.Parse(split[3]) == 2) {
                    Debug.Log("started with " + split[3] + " and ended with " + block_id + ":" + data_id);
                }

                objects.block blk = new objects.block();

                blk.block_id = block_id;
                blk.data_id = data_id;
                blk.position = new Vector3(float.Parse(split[0]), -float.Parse(split[1]), 0f);
                blk.scale = new Vector2(1, 1);
                blk.colour = new Color32(255, 255, 255, 255);

                file.blocks.blockList[i] = blk;
                //Debug.LogError("Legacy loading currnetly disabled.");
            }

            file.visual.bgType = "gradient";
            file.visual.colour1 = new Color32(63, 183, 147, 255);
            file.visual.colour2 = new Color32(46, 89, 101, 255);

            Debug.Log("Conversion Finished");

            Debug.Log("Converted : Execution Time: " + watch.ElapsedMilliseconds + " ms");

            return file;
        }

        public static string[] readClfLegacyLine(string line)
        {
            line = line.Replace(": ", "%");
            line = line.Replace("'", "");
            char split = '%';
            string[] splitted = line.Split(split);
            return splitted;
        }

        public static clfFile newClfFile()
        {
            clfFile file = new clfFile();
            file.general = new segments.general("unknown", "unknown", "unknown");
            return file;
        }

        public static clfFile loadClfFromString(string[] data)
        {
            clfFile file = newClfFile();

            if (data[0].StartsWith("CLF 2"))
            {
                string[] general = getSection("General", data); // gives us all the lines for this section
                foreach (string x in general)
                {
                    string[] line = readClfLine(x);
                    if (line[0] == "name")
                    {
                        file.general.name = line[1];
                    }
                    if (line[0] == "description")
                    {
                        file.general.description = line[1];
                    }
                    if (line[0] == "creator")
                    {
                        file.general.creator = line[1];
                    }
                    if (line[0] == "tags")
                    {
                        file.general.tags = line[1];
                    }
                }

                bool visualExists = false;
                string[] visual = new string[0];

                try
                {
                    visual = getSection("Visual", data);
                    visualExists = true;
                }
                catch
                {
                    Debug.Log("Visual section does not exist. Creating.");
                }

                if(visualExists == false || visual.Length < 1)
                {
                    file.visual.bgType = "gradient";
                    file.visual.colour1 = new Color32(63, 183, 147, 255);
                    Debug.Log("no visual section. creating");
                    file.visual.colour2 = new Color32(46, 89, 101, 255);
                }
                else
                {
                    Debug.Log("visualesection found! reading.");
                    foreach(string x in visual)
                    {
                        string[] line = readClfLine(x);
                        if (line[0] == "bgType")
                        {
                            file.visual.bgType = line[1];
                        }
                        if (line[0] == "background")
                        {
                            Debug.Log("found background");
                            if(file.visual.bgType == "solid")
                            {
                                string[] split = line[1].Split(',');
                                file.visual.colour1 = new Color32(byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]), 255);
                            }
                            if(file.visual.bgType == "gradient")
                            {
                                string[] split = line[1].Split(';');

                                string[] c1 = split[0].Split(',');
                                string[] c2 = split[1].Split(',');

                                file.visual.colour1 = new Color32(byte.Parse(c1[0]), byte.Parse(c1[1]), byte.Parse(c1[2]), 255);
                                Debug.Log("color 1 : " + file.visual.colour1.ToString());
                                file.visual.colour2 = new Color32(byte.Parse(c2[0]), byte.Parse(c2[1]), byte.Parse(c2[2]), 255);
                            }
                            if(file.visual.bgType == "image")
                            {
                                file.visual.bgType = "solid";
                                file.visual.colour1 = new Color32(255, 0, 0, 255);
                            }
                        }
                    }
                }
                

                string[] blocks = getSection("Blocks", data);

                file.blocks = new segments.blocks();
                file.blocks.blockList = new objects.block[blocks.Length];

                for (int i = 0; i < blocks.Length; i++)
                {
                    // BLOCKID:DATAID,XPOS,YPOS,ZPOS,XSCALE,YSCALE,ROTATION,RED,GREEN,BLUE,ALPHA...

                    string[] split = blocks[i].Split(',');

                    file.blocks.blockList[i].block_id = int.Parse(split[0].Split(':')[0]);
                    file.blocks.blockList[i].data_id = int.Parse(split[0].Split(':')[1]);
                    
                    file.blocks.blockList[i].position = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
                    file.blocks.blockList[i].scale = new Vector2(float.Parse(split[4]), float.Parse(split[5]));
                    file.blocks.blockList[i].rotation = float.Parse(split[6]);
                    
                    byte a = 255;
                    
                    if (split[7] != null)
                    {
                        a = (byte)float.Parse(split[7]);
                    }
                    
                    split[9] = split[9].Replace(";", "");

                    file.blocks.blockList[i].colour = new Color32((byte)float.Parse(split[7]), (byte)float.Parse(split[8]), (byte)float.Parse(split[9]), a);
                }

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

                return file;
            }
            else
            {
                Debug.LogError("[CLF UTILS] <loadClfFromString> Requested string is not CLF 2.x! You should use clfUtils.convertLegacy() instead.");
                return null;
            }
        }

        public static string[] readClfLine(string line)
        {
            line = line.Replace(" = ", "%");
            char split = '%';
            string[] splitted = line.Split(split);
            return splitted;
        }

        public static string[] getSection(string sectionName, string[] data)
        {
            List<string> section = new List<string>();
            bool readingSection = false;
            foreach (string x in data)
            {
                if (x == "[" + sectionName + "]")
                {
                    readingSection = true;
                }
                else if (x.StartsWith("]") && x.EndsWith("]"))
                {
                    // we're now about to go into another section. we gotta stop reading
                    readingSection = false;
                }
                if (x != "" && readingSection == true && !x.StartsWith("#") && x != "[" + sectionName + "]")
                {
                    section.Add(x);
                }
            }
            return section.ToArray();
        }

        public static string saveClfFile(segments.general general, segments.blocks blocks, segments.visual visual)
        {
            return baseSave(general, blocks, visual);
        }

        public static string saveClfFile(string name, string description, string creator, segments.blocks blockList, segments.visual visual)
        {
            return baseSave(new segments.general(name, description, creator), blockList, visual);
        }

        public static string saveClfFile(clfFile file)
        {
            return baseSave(file.general, file.blocks, file.visual);
        }

        public static string baseSave(segments.general general, segments.blocks blocks, segments.visual visual)
        {
            // BLOCKID:DATAID,XPOS,YPOS,ZPOS,XSCALE,YSCALE,ROTATION,RED,GREEN,BLUE,ALPHA...

            string backgroundString = "255,255,255;255,255,255";
            if(visual.bgType == "solid")
            {
                backgroundString = visual.colour1.r + "," + visual.colour1.g + "," + visual.colour1.b;
            }
            if(visual.bgType == "gradient")
            {
                backgroundString = visual.colour1.r + "," + visual.colour1.g + "," + visual.colour1.b + ";" + visual.colour2.r + "," + visual.colour2.g + "," + visual.colour2.b;
            }
            if(visual.bgType == "image")
            {
                backgroundString = "255,0,0";
            }

            string clf = @"CLF 2.0


[General]
name = " + general.name + @"
description = " + general.description + @"
creator = " + general.creator + @"
tags = " + general.tags + @"

[Visual]
bgType = " + visual.bgType + @"
background = " + backgroundString + @"

[Blocks]
";

            foreach (objects.block blk in blocks.blockList)
            {
                // this is shit

                clf += blk.block_id + ":" + blk.data_id + ","
                    + blk.position.x + "," + blk.position.y + "," + blk.position.z + ","
                    + blk.scale.x + "," + blk.scale.y + ","
                    + blk.rotation + "," +
                    blk.colour.r + "," +
                    blk.colour.g + "," +
                    blk.colour.b + "," + blk.colour.a + ";\n";
            }

            return clf;
        }
    }
}