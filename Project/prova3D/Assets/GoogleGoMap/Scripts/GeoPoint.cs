using UnityEngine;
using System.Collections;

public class GeoPoint{

	private float lat_d_private; 
	public float lat_d {
		get { 
			return lat_d_private;
		}
		set {
			lat_d_private = value;
			lat_r = toRadians (value);
		}
	}

	private float lon_d_private; 
	public float lon_d {
		get { 
			return lon_d_private;
		}
		set { 
			lon_d_private = value;
			lon_r = toRadians (value);
		}
	}

	public GeoPoint(){
		lat_d = 0;
		lon_d = 0;
	}

	public GeoPoint(float lat_d, float lon_d){
		this.lat_d = lat_d;
		this.lon_d = lon_d;
	}

	public GeoPoint(GeoPoint P){
		lat_d = P.lat_d;
		lon_d = P.lon_d;
	}

	private float lat_r_private; 
	public float lat_r {
		get {
			return lat_r_private;
		}
		set { 
			lat_r_private = value;
			lat_d_private = toDegrees (value);
		}
	}


	private float lon_r_private; 
	public float lon_r {
		get { 
			return lon_r_private;
		}
		set { 
			lon_r_private = value;
			lon_d_private = toDegrees (value);
		}
	}

	public float toDegrees(float r){
		return r * 180.0f / Mathf.PI;
	}

	public float toRadians(float d){
		return d * Mathf.PI / 180.0f;
	}

	public void setLatLon_deg (float lat_deg, float lon_deg){
		this.lat_d = lat_deg;
		this.lon_d = lon_deg;
	}

	public void setLatLon_rad (float lat_rad, float lon_rad){
		this.lat_r = lat_rad;
		this.lon_r = lon_rad;
	}

	public bool isEqual (GeoPoint geo)
	{
		return (this.lat_d == geo.lat_d && this.lon_d == geo.lon_d);
	}

	public override string ToString ()
	{
		return string.Format ("[GeoPoint: lat_d={0}, lon_d={1}, lat_r={2}, lon_r={3}]", lat_d.ToString("R"), lon_d.ToString("R"), lat_r.ToString("R"), lon_r.ToString("R"));
	}

}
