using System.Collections;
using System.Collections.Generic;
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

            public general(string name, string description, string creator)
            {
                this.name = name;
                this.description = description;
                this.creator = creator;
            }
        }

        // BLOCKID,XPOS,YPOS,ZPOS,XSCALE,YSCALE,ROTATION,RED,GREEN,BLUE...
        public class blocks
        {
            public List<objects.block> blockList;

            public blocks(List<objects.block> blockList)
            {
                this.blockList = blockList;
            }
        }
    }

    public class objects
    {
        public class block
        {
            public int block_id;
            public int data_id;
            public Vector3 position;
            public Vector2 scale;
            public float rotation;
            public Color32 colour;

            public block(int block_id , int data_id, Vector3 position , Vector2 scale, float rotation , Color32 colour)
            {
                this.block_id = block_id;
                this.data_id = data_id;
                this.position = position;
                this.scale = scale;
                this.rotation = rotation;
                this.colour = colour;
            }
        }
    }

    public class clfFile
    {
        public string path;
        public string raw;
        public segments.general general;
        public segments.blocks blocks;
    }

    public static class clfUtils
    {
        public static clfFile newClfFile()
        {
            clfFile file = new clfFile();
            file.general = new segments.general("unknown", "unknown", "unknown");
            file.blocks = new segments.blocks(new List<objects.block>());
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
                }

                string[] blocks = getSection("Blocks", data);
                foreach(string blockobj in blocks)
                {
                    // BLOCKID:DATAID,XPOS,YPOS,ZPOS,XSCALE,YSCALE,ROTATION,RED,GREEN,BLUE,ALPHA...

                    string[] split = blockobj.Split(',');
                    objects.block blk = parseBlockString(split);
                    file.blocks.blockList.Add(blk);
                }

                return null;
            }
            else
            {
                Debug.LogError("[CLF UTILS] <loadClfFromString> Requested string is not CLF 2.x! You should use clfUtils.convertLegacy() instead.");
                return null;
            }
        }

        public static objects.block parseBlockString(string[] split)
        {
            int block_id;
            int data_id;
            Vector3 position;
            Vector2 scale;
            float rotation;
            Color32 colour;

            block_id = int.Parse(split[0].Split(':')[0]);
            data_id = int.Parse(split[0].Split(':')[1]);

            position = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
            scale = new Vector2(float.Parse(split[4]), float.Parse(split[5]));
            rotation = float.Parse(split[6]);

            byte a = 255;

            try
            {
                a = (byte)float.Parse(split[7]);
            }
            catch
            {

            }

            colour = new Color32((byte)float.Parse(split[7]), (byte)float.Parse(split[8]), (byte)float.Parse(split[9]), a);

            objects.block blk = new objects.block(block_id, data_id, position, scale, rotation, colour);

            return blk;
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
                if (x != "" && readingSection == true && !x.StartsWith("#"))
                {
                    section.Add(x);
                }
            }
            return section.ToArray();
        }
    }
}