#include <iostream>
#include "../clf.hpp"

using namespace std;

int main()
{
    clf::clfFile* load = clf::clfUtils::loadClfFromFile("../test.clf");
    cout << load->general.name << endl;
	cout << load->visual.colour1.green << endl;
	cout << "klol" << endl;
    return 0;
}