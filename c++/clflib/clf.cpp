/*
Cubey for Nintendo 3DS
clf.cpp
C++ CLF Implementation
©UNTONE 2021-2022
*/

#include <iostream>
#include <fstream>
#include <sstream>
#include <regex>
#include <vector>
#include "clf.hpp"

using namespace std;

clf::segments::general clf::clfFile::general;
clf::segments::visual clf::clfFile::visual{"solid", color32clf(46, 89, 101, 255), color32clf(46, 89, 101, 255)};
clf::segments::blocks clf::clfFile::blocks;
string clf::clfFile::path;
string clf::clfFile::raw;

clf::vector3::vector3(float x, float y, float z)
{
    axis_x = x;
    axis_y = y;
    axis_z = z;
}

clf::vector2::vector2(float x, float y)
{
    axis_x = x;
    axis_y = y;
}

clf::color32clf::color32clf(int r, int g, int b, int a)
{
    clf::color32clf::red = r;
    green = g;
    blue = b;
    alpha = a;
}

clf::segments::general::general(string nm, string dsc, string crt, string tgs)
{
    name = nm;
    description = dsc;
    creator = crt;
    tags = tgs;
}

vector<string> clf::string_split_clf(string input, char split) 
{
    vector<string> lol;
    istringstream stm(input);
    string cts;
    cts.push_back(split);
    while(stm >> cts)
    {
        size_t index = input.find(split);
        lol.push_back(input.substr(0, index));
        lol.push_back(input.substr(index + 1, input.size()));
    }
    return lol;
}

clf::clfFile* clf::clfUtils::loadClfFromString(vector<string> data)
{
    clf::clfFile* file;
    if(data[0].rfind("CLF 2", 0) == 0)
    {
        vector<string> general = getSection("General", data);
        for(int i = 0; i < general.size(); i++)
        {
            vector<string> line = readClfLine(general[i]);
            if(line[0] == "name")
            {
                file->general.name = line[1];
            }
            if(line[0] == "description")
            {
                file->general.description = line[1];
            }
            if(line[0] == "creator")
            {
                file->general.creator = line[1];
            }
            if(line[0] == "tags")
            {
                file->general.tags = line[1];
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
            file->visual.bgType = "gradient";
            file->visual.colour1 = color32clf(63, 183, 147, 255);
            cout << "no visual section. creating" << endl;
            file->visual.colour2 = color32clf(46, 89, 101, 255);
        }
        else
        {
            cout << "visual section found! reading." << endl;
            for(int i = 0; i < visual.size(); i++)
            {
                vector<string> line = readClfLine(visual[i]);
                if(line[0] == "bgType")
                {
                    file->visual.bgType = line[1];
                }
                if(line[0] == "background")
                {
                    cout << "found background" << endl;
                    if(file->visual.bgType == "solid")
                    {
                        vector<string> split = clf::string_split_clf(line[1], ';');
                        file->visual.colour1 = color32clf(stoi(split[0]), stoi(split[1]), stoi(split[2]), 255);
                    }
                    if(file->visual.bgType == "gradient")
                    {
                        vector<string> split = clf::string_split_clf(line[1], ';');
                        vector<string> c1 = clf::string_split_clf(split[0], ',');
						vector<string> c11 = clf::string_split_clf(c1[1], ',');
                        vector<string> c2 = clf::string_split_clf(split[1], ',');
						vector<string> c22 = clf::string_split_clf(split[1], ',');
                        file->visual.colour1 = color32clf(stoi(c1[0]), stoi(c11[0]), stoi(c11[1]), 255);
                        file->visual.colour2 = color32clf(stoi(c2[0]), stoi(c22[0]), stoi(c22[1]), 255);
                    }
                    if(file->visual.bgType == "image")
                    {
                        file->visual.bgType == "solid";
                        file->visual.colour1 = color32clf(255, 0, 0, 255);
                    }
                }
            }
        }
		return file;
        /*vector<string> blocks = getSection("Blocks", data);
        file->blocks = segments::blocks();
        for(int i = 0; i < blocks.size(); i++)
        {
            vector<string> split = clf::string_split_clf(blocks[i], ',');
            file->blocks.blockList[i].block_id = stoi(clf::string_split_clf(split[0], ':')[0]);
            file->blocks.blockList[i].data_id = stoi(clf::string_split_clf(split[0], ':')[1]);
            file->blocks.blockList[i].position = vector3(stof(split[1]), stof(split[2]), stof(split[3]));
            file->blocks.blockList[i].scale = vector2(stof(split[4]), stof(split[5]));
            file->blocks.blockList[i].rotation = stof(split[6]);
            int8_t a = 255;
            if(!split[7].empty())
            {
                a = (int8_t) stof(split[7]);
            }
            split[9] = split[9].replace(split[9].begin(), split[9].end(), ';', '\0');
            file->blocks.blockList[i].colour = color32clf((int8_t) stof(split[7]), (int8_t) stof(split[8]), (int8_t) stof(split[9]), a);
        }*/
    }
}

clf::clfFile* clf::clfUtils::loadClfFromFile(string path)
{
    ifstream file(path);
    stringstream buffer;
    buffer << file.rdbuf();
    string output;
    vector<string> data;
	while(getline(buffer, output))
	{
		data.push_back(output);
	}
    return loadClfFromString(data);
};

clf::clfFile clf::clfUtils::newClfFile()
{
    clf::clfFile file;
    file.general = segments::general("unknown", "unknown", "unknown", "unknown");
    return file;
};

vector<string> clf::clfUtils::readClfLine(string line)
{
	string toReplace(" = ");
	size_t found = line.find(toReplace);
	if(found != string::npos)
	{
		vector<string> array;
		line = line.replace(found, toReplace.length(), "%");
		istringstream ssline(line);
		string token;
		while(getline(ssline, token, '%')) {
			array.push_back(token);
		}
		return array;
	}
}

vector<string> clf::clfUtils::getSection(string sectionName, vector<string> data)
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
