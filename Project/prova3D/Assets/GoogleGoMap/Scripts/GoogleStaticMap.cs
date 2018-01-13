using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoogleStaticMap : MonoBehaviour {

	private const int TILE_SIZE = 256;  // default world map size in google static map.
	private const int MAX_PIXEL = 640;
	const float initialPixelToMercator = 2.0f / ((float) TILE_SIZE); 
	private float curPixelToMercator;
	private static int countOfMapRequests = 0;

	// To prevent unintentionally sending to many map request to the server
	// and paying the fee (for instance in a update loop). Set it to zero to
	// remove the restriction.
	private const  int MAX_MAP_REQUEST_NUM = 50;
	[HideInInspector]
	public bool isDrawn = false;

	public enum MapType
	{
		RoadMap,
		Satellite,
		Terrain,
		Hybrid
	}

	// Map center lat and lon
	// Setting map center in lat, lon, automatically sets the value of center in web Mercator.
	private GeoPoint _centerLatLon;
	public GeoPoint centerLatLon {
		set {
			_centerLatLon = value;
			_centerMercator = geoPointToMercatorPoint (value);
		}
		get {
			return _centerLatLon;
		}
	}

	// Map center in web mercator
	// Setting map center in web mercator automatically sets the value of center in lat, lon.
	private MyPoint _centerMercator;
	public MyPoint centerMercator {
		set { 
			_centerMercator = value;
			_centerLatLon = mercatorPointToGeoPoint (value);
		}	
		get { 
			return _centerMercator;
		}
	}
		
	// Choose zoom level between 0 to 20.
	public int zoom = 15;

	// Sets the overlap percentage between two adjacent tiles of map.
	private float overlap = 0.25f;
	public MapType mapType;

	// Max size for free access is 640.
	public int horizontalSize = 512;
	public int verticalSize = 512;

	public Vector2 realWorldtoUnityWorldScale;

	// Premium accounts may enjoy a 4-time higher resolution, but for free access "double" is the max.
	// If you are using a permium account, you must edit the code and use an integer instead of bool.
	public bool doubleResolution = false;

	public bool defaultStyle = true;

	public bool hideLabels = false;

	public Color landscapeColor, pointOfInterestsColor, highwaysFill, highwaysStroke, arterialRoadsFill, 
				arterialRoadsStroke, localRoadsFill, localRoadsStroke, transitRoadsFill, waterColor;

	// Set your API KEY here
	private string _apiKey = "AIzaSyCX9jO7LZcH0mkITnbl6Jd3EfuHKeJ0WkI";

	public string ApiKey {
		set { 
			_apiKey = value;
		}
	}

	[HideInInspector]
	// Defines a rectangale in Unity world for the map in use.
	public MapRectangle mapRectangle;

	private float widthMercatorX;
	private float heightMercatorY;

	// Call this function before drawing the map.
	public void initialize () {
		if (horizontalSize > MAX_PIXEL)
			horizontalSize = MAX_PIXEL;
		if (verticalSize > MAX_PIXEL)
			verticalSize = MAX_PIXEL;

		const float initialPixelToMercator = 2.0f / ((float) TILE_SIZE); 

		curPixelToMercator = initialPixelToMercator / Mathf.Pow (2.0f, (float)zoom) / (this.doubleResolution ? 2.0f : 1.0f);
	
		widthMercatorX = (horizontalSize * (this.doubleResolution ? 2.0f : 1.0f)) * curPixelToMercator;
		heightMercatorY = (verticalSize * (this.doubleResolution ? 2.0f : 1.0f)) * curPixelToMercator;

		// You can initialize realWorldtoUnityWorldScale to another value of your choice.
		realWorldtoUnityWorldScale = new Vector2 (0.1f, 0.1f); 
	}


	// Call to draw map on an object (with material assigned)
	public void DrawMap() {	
		isDrawn = false;
		StartCoroutine(DrawMapCoroutine());
	}

	IEnumerator DrawMapCoroutine ()
	{
		if (MAX_MAP_REQUEST_NUM != 0 && countOfMapRequests < MAX_MAP_REQUEST_NUM) {
			string baseURL = "https://maps.googleapis.com/maps/api/staticmap?";
			string location = "center=" + _centerLatLon.lat_d.ToString () + "," + _centerLatLon.lon_d.ToString ();
			string parameters = "&zoom=" + zoom.ToString ();
			parameters += "&size=" + horizontalSize.ToString () + "x" + verticalSize.ToString ();
			parameters += "&scale=" + (doubleResolution ? "2" : "1");
			parameters += "&maptype=" + mapType.ToString ().ToLower ();
			string style = doStyles ();

			string url = "";
			url = baseURL + location + parameters + style + (_apiKey == "" ? "" : "&key=" + _apiKey);
			findCorners ();

			WWW www = new WWW (url);
			countOfMapRequests++;
			// Wait for download to complete
			yield return www;

			Renderer renderer = GetComponent<Renderer> ();
			// assign texture
			renderer.material.mainTexture = www.textureNonReadable;
			isDrawn = true;
		} else {
			Debug.Log ("Too many map requests. Restart the game.");
		}

	}

	// Call to draw map on a GUI element with RawImage.
	public void DrawMapGUI() {	
		isDrawn = false;
		StartCoroutine(DrawMapCoroutineGUI());
	}

	IEnumerator DrawMapCoroutineGUI ()
	{
		if (MAX_MAP_REQUEST_NUM != 0 && countOfMapRequests < MAX_MAP_REQUEST_NUM) {
			string baseURL = "https://maps.googleapis.com/maps/api/staticmap?";
			string location = "center=" + _centerLatLon.lat_d.ToString () + "," + _centerLatLon.lon_d.ToString ();
			string parameters = "&zoom=" + zoom.ToString ();
			parameters += "&size=" + horizontalSize.ToString () + "x" + verticalSize.ToString ();
			parameters += "&scale=" + (doubleResolution ? "2" : "1");
			parameters += "&maptype=" + mapType.ToString ().ToLower ();
			string style = doStyles ();

			string url = "";
			url = baseURL + location + parameters + style + (_apiKey == "" ? "" : "&key=" + _apiKey);
			findCorners ();

			WWW www = new WWW (url);
			countOfMapRequests++;
			//Debug.Log (url);
			// Wait for download to complete
			yield return www;

			// assign texture
			gameObject.GetComponent<RawImage>().texture = www.textureNonReadable;;
			isDrawn = true;
		} else {
			Debug.Log ("Too many map requests. Restart the game.");
		}

	}


	// IMPORTANT NOTE: As of today, 12/16/2016, Google Static Maps API has an issue with "road strokes"
	// This issue will be likely solved at some point. But the developer is recommended to keep styles 
	// upto data by comparing them to:
	// https://developers.google.com/maps/documentation/static-maps/styling

	public string doStyles() {
		string style = (hideLabels ? "&style=feature:all|element:labels|visibility:off" : "");
		if (defaultStyle) {
			return style;
		} else {
			style += "&style=feature:landscape|element:all|color:" + "0x" + ColorUtility.ToHtmlStringRGB (landscapeColor);
			style += "&style=feature:poi|element:geometry|color:" + "0x" + ColorUtility.ToHtmlStringRGB (pointOfInterestsColor);
			style += "&style=feature:road.highway|element:geometry.fill|color:" + "0x" + ColorUtility.ToHtmlStringRGB (highwaysFill);
			style += "&style=feature:road.highway|element:geometry.stroke|color:" + "0x" + ColorUtility.ToHtmlStringRGB (highwaysStroke);
			style += "&style=feature:road.arterial|element:geometry.fill|color:" + "0x" + ColorUtility.ToHtmlStringRGB (arterialRoadsFill);
			style += "&style=feature:road.arterial|element:geometry.stroke|color:" + "0x" + ColorUtility.ToHtmlStringRGB (arterialRoadsStroke);
			style += "&style=feature:road.local|element:geometry|color:" + "0x" + ColorUtility.ToHtmlStringRGB (localRoadsFill);
			style += "&style=feature:road.local|element:geometry.stroke|color:" + "0x" + ColorUtility.ToHtmlStringRGB (localRoadsStroke);
			style += "&style=feature:transit|element:all|color:" + "0x" + ColorUtility.ToHtmlStringRGB (transitRoadsFill);
			style += "&style=feature:water|element:all|color:" + "0x" + ColorUtility.ToHtmlStringRGB (waterColor);
			style += "&style=feature:road|element:geometry.stroke|color:" + "0x000000";
			return (style.Replace ("|", "%7c")).Replace (":", "%3a");
		}
	}

	private void findCorners(){
		mapRectangle = new MapRectangle ();
	
		MyPoint SW_Mercator = new MyPoint (centerMercator.x - widthMercatorX / 2.0f, centerMercator.y - heightMercatorY / 2.0f);

		MyPoint NE_Mercator = new MyPoint (centerMercator.x + widthMercatorX / 2.0f, centerMercator.y + heightMercatorY / 2.0f);
		MyPoint mapCenterMeters = new MyPoint (gameObject.transform.position.x, gameObject.transform.position.z);
		mapRectangle.setCorners (SW_Mercator, NE_Mercator, mapCenterMeters);
	}

	// Converts given lat in WGS84 Datum to Y in Spherical Mercator EPSG:900913. Y is in (-1,1) corresponding to +- 85.051129° latittude.
	// NOTE: input must be in radians.
	private static float latToMecatorY_r (float lat_rad) {
		return Mathf.Log(Mathf.Tan(Mathf.PI/4.0f + lat_rad/2.0f)) / Mathf.PI;
	}

	// Converts given lon in WGS84 Datum to X in Spherical Mercator EPSG:900913. X is in (-1,1) corresponding to +- 180° longitude.
	// NOTE: input must be in radians.
	private static float lonToMecatorX_r (float lon_rad) {
		return lon_rad / Mathf.PI;
	}

	// Converts given X in Spherical Mercator EPSG:900913 to lon in WGS84 Datum to . X is in (-1,1) corresponding to +- 85.051129° latittude.
	// NOTE: output is in radians.
	private static float mercatorYtoLat_r (float y) {
		return 2.0f * Mathf.Atan(Mathf.Exp(Mathf.PI * y)) - Mathf.PI/2.0f;

	}

	// Converts given X in Spherical Mercator EPSG:900913 to lon in WGS84 Datum to . X is in (-1,1) corresponding to +- 180° longitude.
	// NOTE: output is in radians.
	private static float mercatorXtoLon_r (float x) {
		return x * Mathf.PI;
	}

	private GeoPoint mercatorPointToGeoPoint (MyPoint merc){
		GeoPoint geo = new GeoPoint ();
		geo.setLatLon_rad (mercatorYtoLat_r (merc.y), mercatorXtoLon_r (merc.x));
		return geo;
	}
	private MyPoint geoPointToMercatorPoint (GeoPoint geo){
		return new MyPoint (lonToMecatorX_r (geo.lon_r), latToMecatorY_r (geo.lat_r));
	}
	// return distance between two GeoPoints
	private static float haversine_dist(GeoPoint loc1, GeoPoint loc2){
		const float R = 6371008;  // Earth's radius in m
		float delta_lat = loc1.lat_r - loc2.lat_r;
		float delta_lon = loc1.lon_r - loc2.lon_r;
		float a = Mathf.Sin (delta_lat / 2.0f) * Mathf.Sin (delta_lat / 2.0f)
		                + Mathf.Cos (loc1.lat_r) * Mathf.Cos (loc2.lat_r) * Mathf.Sin (delta_lon / 2.0f) * Mathf.Sin (delta_lon / 2.0f);
		float c = 2.0f * Mathf.Atan2 (Mathf.Sqrt (a), Mathf.Sqrt (1.0f - a));
		return R * c;          
	}

	public class MapRectangle {
		private GeoPoint NE_LatLon, NW_LatLon, SE_LatLon, SW_LatLon;
		private MyPoint NE_Mercator, NW_Mercator, SE_Mercator, SW_Mercator;
		private MyPoint NE_Meters, NW_Meters, SE_Meters, SW_Meters;
		private float width_meters, height_meters;
		public MapRectangle(){
			NE_LatLon = new GeoPoint();
			NW_LatLon = new GeoPoint();
			SE_LatLon = new GeoPoint();
			SW_LatLon = new GeoPoint();
		}
		public enum SetCorner{
			NE,
			SW
		}

		public enum GetCorner{
			NE,
			NW,
			SE,
			SW
		}

		// sets SW and NE corners of a mapRectangle.
		public void setCorners (MyPoint SW , MyPoint NE, MyPoint mapCenterMeters) {
			setCorner (SetCorner.SW, SW.x, SW.y);
			setCorner (SetCorner.NE, NE.x, NE.y);

			width_meters = (
				haversine_dist (NW_LatLon, NE_LatLon) +
				haversine_dist (SW_LatLon, SE_LatLon)
			) * 0.5f;


			// heights of both sides are same for horizontal rectangle, and there is no need for averaging. Done for consistency!
			height_meters = (
				haversine_dist (NW_LatLon, SW_LatLon) +
				haversine_dist (NE_LatLon, SE_LatLon)
			) * 0.5f;

			SW_Meters.setPoint (mapCenterMeters.x - width_meters * 0.5f, mapCenterMeters.y - height_meters * 0.5f);
			NE_Meters.setPoint (mapCenterMeters.x + width_meters * 0.5f, mapCenterMeters.y + height_meters * 0.5f);
			SE_Meters.setPoint (mapCenterMeters.x + width_meters * 0.5f, mapCenterMeters.y - height_meters * 0.5f);
			NW_Meters.setPoint (mapCenterMeters.x - width_meters * 0.5f, mapCenterMeters.y + height_meters * 0.5f);

		}

		private void setCorner (SetCorner corner, float x, float y){
			float lon_r = GoogleStaticMap.mercatorXtoLon_r (x);
			float lat_r = GoogleStaticMap.mercatorYtoLat_r (y);
			switch (corner) {

			case SetCorner.NE:
				NE_LatLon.setLatLon_rad (lat_r, lon_r);
				NW_LatLon.lat_r = lat_r;
				SE_LatLon.lon_r = lon_r;

				NE_Mercator.x = x;
				NE_Mercator.y = y;
				NW_Mercator.y = y;
				SE_Mercator.x = x;
				break;
			case SetCorner.SW:
				SW_LatLon.setLatLon_rad (lat_r, lon_r);
				NW_LatLon.lon_r = lon_r;
				SE_LatLon.lat_r = lat_r;

				SW_Mercator.x = x;
				SW_Mercator.y = y;
				NW_Mercator.x = x;
				SE_Mercator.y = y;
				break;
			default:
				throw new System.ArgumentException("Invalid corner request. Only NE and SW corners can be set explicity. NW and SE are set internally.", "original");
			}
		}

		public GeoPoint getCornerLatLon (GetCorner corner){
			switch (corner) {
			case GetCorner.NE:
				return NE_LatLon;
			case GetCorner.NW:
				return NW_LatLon;
			case GetCorner.SE:
				return SE_LatLon;
			case GetCorner.SW:
				return SW_LatLon;
			default:
				throw new System.ArgumentException("Invalid corner request.", "corner");
			}
		}

		public MyPoint getCornerMercator (GetCorner corner){
			switch (corner) {
			case GetCorner.NE:
				return NE_Mercator;
			case GetCorner.NW:
				return NW_Mercator;
			case GetCorner.SE:
				return SE_Mercator;
			case GetCorner.SW:
				return SW_Mercator;
			default:
				throw new System.ArgumentException("Invalid corner request.", "corner");
			}
		}

		public MyPoint getCornerMeters (GetCorner corner){
			switch (corner) {
			case GetCorner.NE:
				return NE_Meters;
			case GetCorner.NW:
				return NW_Meters;
			case GetCorner.SE:
				return SE_Meters;
			case GetCorner.SW:
				return SW_Meters;
			default:
				throw new System.ArgumentException("Invalid corner request.", "corner");
			}
		}
		public float getHeightMeters() {
			return (NE_Meters.y - SE_Meters.y);
		}
		public float getWidthMeters() {
			return (NE_Meters.x - NW_Meters.x);
		}
		public float getHeightLat_deg() {
			return ((NE_LatLon.lat_d - SE_LatLon.lat_d) + (NW_LatLon.lat_d - SW_LatLon.lat_d)) * 0.5f;
		}
		public float getWidthLon_deg() {
			return ((NE_LatLon.lon_d - NW_LatLon.lon_d) + (SE_LatLon.lon_d - SW_LatLon.lon_d)) * 0.5f;
		}

		public float getHeightMeterLatRatio_deg() {
			return (getHeightMeters() / getHeightLat_deg ());
		}

		public float getWidthMeterLonRatio_deg() {
			return (getWidthMeters () / getWidthLon_deg ());
		}

	}

	// Return the position of an object in the unity world given its position on map (lat, lon)
	public Vector2 getPositionOnMap(GeoPoint point) {
		
		float delta_lon = point.lon_d - this._centerLatLon.lon_d;
		float delta_lat = point.lat_d - this._centerLatLon.lat_d;
	
		return Vector2.Scale(
			new Vector2 (gameObject.transform.position.x / this.realWorldtoUnityWorldScale.x + this.mapRectangle.getWidthMeterLonRatio_deg () * delta_lon,
				gameObject.transform.position.z / this.realWorldtoUnityWorldScale.y + this.mapRectangle.getHeightMeterLatRatio_deg () * delta_lat),
			this.realWorldtoUnityWorldScale);
	}

	// Return the position of an object on the map (lat, lon) given its position in the unity world.
	public GeoPoint getPositionOnMap(Vector2 point) {
		
		float delta_x = (point.x - gameObject.transform.position.x) / this.realWorldtoUnityWorldScale.x;
		float delta_y = (point.y - gameObject.transform.position.z) / this.realWorldtoUnityWorldScale.y;
		GeoPoint geo = new GeoPoint ();
		geo.setLatLon_deg (this.centerLatLon.lat_d + delta_y / this.mapRectangle.getHeightMeterLatRatio_deg (), this.centerLatLon.lon_d + delta_x / this.mapRectangle.getWidthMeterLonRatio_deg ());
		return geo;
	}


	// Given the GeoPoint point, calculate the center of a tile which includes point.
	public MyPoint tileCenterMercator (GeoPoint geo){
		MyPoint merc = geoPointToMercatorPoint (geo);
		float tileWidthMercatorX = (1.0f - overlap) * this.widthMercatorX;
		float tileHeightMercatorY = (1.0f - overlap) * this.heightMercatorY;

		float tileCenterMercatorX = (Mathf.Floor (merc.x / tileWidthMercatorX) + 0.5f) * tileWidthMercatorX;
		float tileCenterMercatorY = (Mathf.Floor (merc.y / tileHeightMercatorY) + 0.5f) * tileHeightMercatorY;
		return new MyPoint (tileCenterMercatorX, tileCenterMercatorY);
	}

	public struct MyPoint {
		public float x,y;
		public MyPoint(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public void setPoint(float x, float y) {
			this.x = x;
			this.y = y;
		}
		public bool isEqual (MyPoint point)
		{
			return (this.x == point.x && this.y == point.y);
		}
		public override string ToString ()
		{
			return string.Format ("[MyPoint: x={0}, y={1}]", x.ToString("R"), y.ToString("R"));
		}
	}
}