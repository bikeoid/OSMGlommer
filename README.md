The purpose of OSM Glommer is to concatenate all segments of a street into a single way having identical tags.    This is most often useful when importing TIGER or municipal data.

NOTE: Although it recognizes relations, relations are not updated when ways are combined - the result could be dangling relation references.  Caution is required before applying to existing OSM data.

Input: OSM format XML file
Output: OSM format XML file

Requires: Microsoft Visual Studio.NET 2019 16.7.4 or higher; uses .NET Core 3.1 and requires Windows to run

