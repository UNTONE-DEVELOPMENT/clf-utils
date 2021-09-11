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
                                    
                                }
                            }
                        }
                    }
                }
            }
    };
}
