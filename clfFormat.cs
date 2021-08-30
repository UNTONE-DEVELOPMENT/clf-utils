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
            public Quaternion rotation;
            public Color32 colour;

            public block(int block_id, int data_id, Vector3 position, Vector2 scale, Quaternion rotation, Color32 colour)
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
        public static clfFile loadClfFromString(string[] data)
        {
            if(data[0].StartsWith("CLF 2"))
            {
                return null;
            }
            else
            {
                Debug.LogError("[CLF UTILS] <loadClfFromString> Requested string is not CLF 2.x! You should use clfUtils.convertLegacy() instead.");
                return null;
            }
        }
    }
}