Google Go Maps
---------------------------

Google Go Maps uses Google static maps API to query maps and "map" them to unity world.
This asset makes it easy to convert map positions to unity world positions and vice versa.

Support: tavakkol.sasan@gmail.com

Getting Started
---------------------------
Assign GoogleStaticMap.cs to an object (such as a quad) with a material assigned.
Call initialize() and then DrawMap(). 

Learn More
---------------------------
To make the most out of this asset, run the demo.scene and learn how every function is
being used. In this demo scene, we impelement a tiled mapping system and let the player
move on the map. A few objects are also available in the scene and we provide the sample 
codes to put this objects on the correct location on the map according to their (lat,lon)
location. Further more we provide scripts to query device location services if available.
