<p align="center">
<img height="120" src="https://github.com/tmscarla/FacebookPlaces3DTour/blob/master/Images/fbplace.png">
  <img height="120" src="https://github.com/tmscarla/FacebookPlaces3DTour/blob/master/Images/unitylogo.png">
</p>

# FacebookPlaces3DTour
Web app for a 3D tour in Unity through Facebook Places using GPS.

# Overview
This is a project that I developed for the Multimedia Design and Production course at the University of Florence during my Bachelor Degree in Computer Engineering. The assignment required to map a chosen type of data extracted from a social network and in a 3D environment in Unity.
I chose to create a coordinate-based and explorable 3D world and to populate it with 3D marks that stands for Facebook Places, enabling a player to navigate into the world and interact with them.

# Implementation
The implementation of the solution is the following: the user is enabled to control the player (make it walk, run or jump) through the keyboard arrows and can interact with a place just clicking on it. A pop-up will then appear with place-related information and a link to the correspondent Facebook page.

## APIs
In order to retrieve places information it was used the Facebook [API Place Graph](https://developers.facebook.com/docs/places), making a query for the nearest places in a radius of 5km.

## Places
<img height="250" align="right" src="https://github.com/tmscarla/FacebookPlaces3DTour/blob/master/Images/place.png">

* Places have been modeled with [Sketchup](http://www.sketchup.com/) in order to obtain a 3D object that represents faithfully a traditional place icon used in maps.
* Each place is permanently in rotation on its vertical axis, in order to be recognizable from every direction.
* Each place has a text label with its name on top.

### Place Relevance
The relevance of the place is predetermined according to number of checkins **nc** and the average number of ratings **r** from the Facebook users. A scale factor is applied to the place 3D object following these rules:

| nc                    |     c     |
| ----------------------|:---------:|
| nc < 10               |     0     |
| 10 ≤ nc ≤ 100         |     1     |
| 100 ≤ nc ≤ 1.000      |     2     |
| 1.000 ≤ nc ≤ 10.000   |     3     |
| 10.000 ≤ nc ≤ 100.000 |     4     |
| nc > 100.000          |     5     |

<p align="center">
<a href="https://www.codecogs.com/eqnedit.php?latex=scaleFactor&space;=&space;\alpha&space;*&space;c&space;&plus;&space;(1-\alpha&space;)*r" target="_blank"><img height="30" src="https://latex.codecogs.com/gif.latex?scaleFactor&space;=&space;\alpha&space;*&space;c&space;&plus;&space;(1-\alpha&space;)*r" title="scaleFactor = \alpha * c + (1-\alpha )*r" /></a>
</p>


## Map
For displaying the map on the ground with satellites images it was used the asset [Google Go Maps](https://www.assetstore.unity3d.com/en/#!/content/78642) from the Unity Store. This gave the possibility to avoid the implementation details of a web mercator projection at all. The conversion between real polar coordinates and cartesian coordinates on the plane is accomplished by the asset.

# User Interface

## Buttons
<img height="200" align="right" src="https://github.com/tmscarla/FacebookPlaces3DTour/blob/master/Images/buttons.png">

* Refresh: permits to reloads places from the current position.  
* Home: teleport the player to the initial position (marked with a green down arrow).
* Set Position: teleport the player to a specific position (lat, lon).

## Toggles
<img height="250" align="center" src="https://github.com/tmscarla/FacebookPlaces3DTour/blob/master/Images/menu.png">

* Enable/disable places of a specific category.
* Useful in highly concentrated zones.
* Places requests handled with priority for visible categories.

## Orientation

* **Compass:** to specific user orientation in the real world.
* **Current Position:** latitude and longitude.
* **Mini Map:** eagle-eye camera that follows the player.

# Demo
<p align="center">
<img src="https://github.com/tmscarla/FacebookPlaces3DTour/blob/master/Images/demo.gif">
</p>

## Requirements
| Software       | Verison        | Required |
| -------------- |:--------------:| --------:|
| **Unity**      |     >= 5.3     |    Yes   |

## Authors

* **Tommaso Scarlatti**
* **Simone Magistri**

## License
Licensed under the term of [MIT License](http://en.wikipedia.org/wiki/MIT_License). See attached file LICENSE.
