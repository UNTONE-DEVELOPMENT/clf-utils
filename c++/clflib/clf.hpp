/*
Cubey for Nintendo 3DS
clf.hpp
Header file for Matthew5pl's C++ CLF Implementation
©UNTONE 2021-2022
*/

#pragma once

#include <iostream>
#include <fstream>
#include <sstream>
#include <regex>
#include <vector>

using namespace std;

class clf
{
    public:
        class vector3
        {
            public:
                float axis_x;
                float axis_y;
                float axis_z;
                vector3(float x, float y, float z);
        };

        class vector2
        {
            public:
                float axis_x;
                float axis_y;
                vector2(float x, float y);
        };

        class color32clf
        {
            public:
                int red;
                int green;
                int blue;
                int alpha;
                color32clf(int r, int g, int b, int a);
        };

        class objects
        {
            public:
                struct block
                {
                    public:
                        int block_id;
                        int data_id;
                        vector3 position;
                        vector2 scale;
                        float rotation;
                        color32clf colour;
                };
        };

        class segments
        {
            public:
                class general
                {
                    public:
                        string name;
                        string description;
                        string creator;
                        string tags;
                        general(string nm, string dsc, string crt, string tgs);
						general() = default;
                };
                struct blocks 
                { 
                    public:
                        vector<objects::block> blockList;
                };
                struct visual
                {
                    public:
                        string bgType;
                        color32clf colour1;
                        color32clf colour2;
                };
        };

        class clfFile
        {
            public:
                static string path;
                static string raw;
                static clf::segments::general general;
                static clf::segments::blocks blocks;
                static clf::segments::visual visual;
        };

        static vector<string> string_split_clf(string input, char split);

        class clfUtils 
        {
            public:
                static clfFile* loadClfFromString(vector<string> data);
                static clfFile* loadClfFromFile(string path);
                static clfFile newClfFile();
                static vector<string> readClfLine(string line);
                static vector<string> getSection(string sectionName, vector<string> data);
				static clfFile* testClf();
        };
};
