#include <iostream>
#include "3ds.h"
#include "citro2d.h"
#include <fstream>
#include <sstream>
#include <regex>
#include <vector>

using namespace std;

namespace clf 
{
    class vector3
    {
        public:
            float axis_x;
            float axis_y;
            float axis_z;
            vector3(float x, float y, float z)
            {
                axis_x = x;
                axis_y = y;
                axis_z = z;
            }
    };

    class vector2
    {
        public:
            float axis_x;
            float axis_y;
            vector2(float x, float y)
            {
                axis_x = x;
                axis_y = y;
            }
            vector2() = default;
    };

    class color32clf
    {
        public:
            int red;
            int green;
            int blue;
            int alpha;
            color32clf(int r, int g, int b, int a)
            {
                red = r;
                green = g;
                blue = b;
                alpha = a;
            }
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
                    general(string nm, string dsc, string crt, string tgs = "")
                    {
                        name = nm;
                        description = dsc;
                        creator = crt;
                        tags = tgs;
                    }
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
                    C2D_Sprite sprite;
            };
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

    class clfFile
    {
        public:
            static string path;
            static string raw;
            static segments::general general;
            static segments::blocks blocks;
            static segments::visual visual;
    };
    
    vector<string> string_split(string input, char split) 
    {
        vector<string> array;
        input = input.replace(input.begin(), input.end(), ' = ', '%');
        istringstream ssline(input);
        string token;
        while(getline(ssline, token, split)) {
            array.push_back(token);
        }
        return array;
    }

    class clfUtils 
    {
        public:
            static clfFile loadClfFromFile(string path)
            {
                ifstream file(path);
                stringstream buffer;
                buffer << file.rdbuf();
                string output;
                getline(buffer, output);
                vector<string> data;
                data.push_back(output);
                return loadClfFromString(data);
            };
            static clfFile newClfFile()
            {
                clfFile file;
                file.general = segments::general("unknown", "unknown", "unknown");
                return file;
            };
            static vector<string> readClfLine(string line)
            {
                vector<string> array;
                line = line.replace(line.begin(), line.end(), ' = ', '%');
                istringstream ssline(line);
                string token;
                while(getline(ssline, token, '%')) {
                    array.push_back(token);
                }
                return array;
            }

            static vector<string> getSection(string sectionName, vector<string> data)
            {
                vector<string> section;
                bool readingSection = false;
                for(int i = 0; i < data.size(); i++)
                {
                    if(data[i] == "[" + sectionName + "]")
                    {
                        readingSection = true;
                    }
                    else if(data[i][0] == ']' && data[i][data[i].size()] == ']') 
                    {
                        readingSection = false;
                    }
                    if(data[i] != "" && readingSection == true && data[i][0] != '#' && data[i] != "[" + sectionName + "]")
                    {
                        section.push_back(data[i]);
                    }
                }
                return section;
            }

            static clfFile loadClfFromString(vector<string> data)
            {
                clfFile file;
                if(data[0].rfind("CLF 2", 0) == 0)
                {
                    vector<string> general = getSection("General", data);
                    for(int i = 0; i < data.size(); i++)
                    {
                        vector<string> line = readClfLine(data[i]);
                        if(line[0] == "name")
                        {
                            file.general.name = line[1];
                        }
                        if(line[0] == "description")
                        {
                            file.general.description = line[1];
                        }
                        if(line[0] == "creator")
                        {
                            file.general.creator = line[1];
                        }
                        if(line[0] == "tags")
                        {
                            file.general.tags = line[1];
                        }
                    }

                    bool visualExists = false;
                    vector<string> visual;
                    try
                    {
                        visual = getSection("Visual", data);
                        visualExists = true;
                    }
                    catch(const std::exception& e)
                    {
                        cout << "Visual section does not exist. Creating" << endl;
                    }

                    if(visualExists == false || visual.size() < 1)
                    {
                        file.visual.bgType = "gradient";
                        file.visual.colour1 = color32clf(63, 183, 147, 255);
                        cout << "no visual section. creating" << endl;
                        file.visual.colour2 = color32clf(46, 89, 101, 255);
                    }
                    else
                    {
                        cout << "visual section found! reading." << endl;
                        for(int i = 0; i < visual.size(); i++)
                        {
                            vector<string> line = readClfLine(visual[i]);
                            if(line[0] == "bgType")
                            {
                                file.visual.bgType = line[1];
                            }
                            if(line[0] == "background")
                            {
                                cout << "found background" << endl;
                                if(file.visual.bgType == "solid")
                                {
                                    vector<string> split = string_split(line[1], ';');
                                    file.visual.colour1 = color32clf(stoi(split[0]), stoi(split[1]), stoi(split[2]), 255);
                                }
                                if(file.visual.bgType == "gradient")
                                {
                                    vector<string> split = string_split(line[1], ';');
                                    vector<string> c1 = string_split(split[0], ',');
                                    vector<string> c2 = string_split(split[1], ',');
                                    file.visual.colour1 = color32clf(stoi(c1[0]), stoi(c1[1]), stoi(c1[2]), 255);
                                    file.visual.colour2 = color32clf(stoi(c2[0]), stoi(c2[1]), stoi(c2[2]), 255);
                                }
                                if(file.visual.bgType == "image")
                                {
                                    file.visual.bgType == "solid";
                                    file.visual.colour1 = color32clf(255, 0, 0, 255);
                                }
                            }
                        }
                    }

                    vector<string> blocks = getSection("Blocks", data);

                    file.blocks = segments::blocks();
                    objects::block arrayBl[blocks.size()] = {};
                    for(int i = 0; i < blocks.size(); i++)
                    {
                        vector<string> split = string_split(blocks[i], ',');

                        file.blocks.blockList[i].block_id = stoi(string_split(split[0], ':')[0]);
                        file.blocks.blockList[i].data_id = stoi(string_split(split[0], ':')[1]);

                        file.blocks.blockList[i].position = vector3(stof(split[1]), stof(split[2]), stof(split[3]));
                        file.blocks.blockList[i].scale = vector2(stof(split[4]), stof(split[5]));
                        file.blocks.blockList[i].rotation = stof(split[6]);

                        int8_t a = 255;

                        if(!split[7].empty())
                        {
                            a = (int8_t) stof(split[7]);
                        }

                        split[9] = split[9].replace(split[9].begin(), split[9].end(), ';', '\0');

                        file.blocks.blockList[i].colour = color32clf((int8_t) stof(split[7]), (int8_t) stof(split[8]), (int8_t) stof(split[9]), a);

                        return file;
                    }
                }
            }
    };
}
