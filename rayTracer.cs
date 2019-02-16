/* 
 * Emily Wilson
 * Project 2: Ray Tracer
 * CS 325
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class rayTracer : MonoBehaviour {

	private Texture2D myPicture;
	int row, col;
	Color c;
	List<sphere> sphereList = new List <sphere>();
	public string inFileName = "test.cli"; //initial input file

	public class sphere
	{
		public float radius, rVal, gVal, bVal;
		public float xCor, yCor, zCor;
		public Vector3 center; 
		public Color spCol;

		public sphere(float rad, float x, float y, float z, float r, float g, float b)
		{
			radius = rad;
			xCor = x;
			yCor = y;
			zCor = z;
			rVal = r;
			gVal = g;
			bVal = b;
			center = new Vector3(xCor, yCor, zCor);
			spCol = new Color(r, g, b);
		}

		/* findhit()
	 	* Input: Ray ray, double t0, double t1, record r
	 	* Output: true if a hit was found, false otherwise
	 	* Finds if a sphere was hit
    	*/
		public bool findhit(Ray ray, double t0, double t1, record r) 
		{
			double A, B, C;
			A = Vector3.Dot (ray.direction, ray.direction);
			B = 2 * (Vector3.Dot (ray.direction, ray.origin - center));
			C = Vector3.Dot (ray.origin - center, ray.origin - center) - (radius * radius);
			double discrim = (B * B) - (4 * A * C);
			if (discrim > 0) 
			{
				double sqrtDiscrim = Math.Sqrt(discrim);
				double t = (-B - sqrtDiscrim) / (2 * A);
				if (t < t0)
					t = ((-1 * B) + sqrtDiscrim) / (2 * A);
				if (t < t0 || t > t1)
					return false;
				r.t = t;
				r.color = spCol;
				return true;
			}
			else
				return false;
		}
	}

	public class record
	{
		public Color color;
		public double t;
	}

	// Use this for initialization
	void Start () {
		readFile ();
	}

	/* readFile()
 	* Input: none
 	* Output: none
 	* Reads the input file and writes it to the output file
	*/
	void readFile()
	{
		string line;
		using (StreamReader sr = new StreamReader (inFileName)) {
			using (StreamWriter sw = new StreamWriter ("Output.ppm")) {
				while ((line = sr.ReadLine ()) != null) 
				{
					sw.WriteLine (line);
				}
			}
		}
		setInfo ();
	}

	/* setInfo()
 	* Input: none
 	* Output: none
 	* Parses the input file and edits the background, picture size, and spheres accordingly.
	*/
	void setInfo()
	{
		using (StreamReader sr = new StreamReader ("Output.ppm")) 
		{
			sphereList.Clear ();
			string line;
			while ((line = sr.ReadLine ()) != null) 
			{
				line = line.Replace ("−", "-");
				string[] newLine = line.Split (new [] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);

				if (newLine [0] == "screen") 
				{
					row = int.Parse(newLine[1]);
					col = int.Parse (newLine [2]);
					myPicture = new Texture2D ((int)(row), (int)(col));
				} 

				else if (newLine [0] == "background") 
				{
					float r = float.Parse (newLine [1]);
					float g = float.Parse (newLine [2]); 
					float b = float.Parse (newLine [3]);

					for (int x = 0; x < myPicture.width; x++) 
					{
						for (int y = 0; y < myPicture.height; y++) 
						{
							c = new Color (r, g, b);
							myPicture.SetPixel (x, y, c);
						}
					}
					myPicture.Apply ();
				} 

				else //newLine[0] == "sphere"
				{
					float rad = int.Parse (newLine [1]);
					float x = int.Parse (newLine [2]);
					float y = int.Parse (newLine [3]);
					float z = (int.Parse (newLine [4]));
					float r = int.Parse (newLine [5]);
					float g = int.Parse (newLine [6]);
					float b = int.Parse (newLine [7]);

					sphere newSphere = new sphere(rad, x, y, z, r, g, b);
					sphereList.Add(newSphere);
				}
			}

			for (int x = 0; x < sphereList.Count; x++) //sorts sphereList from smallest z value to largest
			{
				for (int y = 0; y < sphereList.Count - 1; y++) 
				{
					if (sphereList [y].zCor > sphereList [y + 1].zCor) 
					{
						sphere temp = sphereList [y + 1];
						sphereList [y + 1] = sphereList [y];
						sphereList [y] = temp;
					}
				}
			}
		}

		RayTraceImage ();
	}
		
	/* RayTraceImage()
 	* Input: none
 	* Output: none
 	* Renders the image using the ray tracer
	*/
	void RayTraceImage()
	{
		for (int i = 0; i < myPicture.width; i++) 
		{
			for (int j = 0; j < myPicture.height; j++) 
			{
				Ray newRay = new Ray (new Vector3 (i, j, 0), new Vector3 (0, 0, -1));
				Color pixColor = trace(newRay); 
				myPicture.SetPixel(i, j, pixColor);
			}
		}
		myPicture.Apply ();
	}

	/* trace()
 	* Input: Ray ray
 	* Output: a Color
 	* Returns the Color of the closest sphere
	*/
	Color trace(Ray ray)
	{
		double t0 = .0001;
		double t1 = 1000000;
		bool hit = false;
		record myHit = new record(); 
		myHit.t = 1;
		double currT = t1;
		myHit.color = new Color (0, 0, 0);

		foreach (sphere s in sphereList) 
		{ 
//			t0 = .0001;
//			t1 = 1000000;
//			hit = false;
//			//record myHit = new record(); 
//			myHit.t = 1;
//			currT = t1;
//			//myHit.color = new Color (0, 0, 0);

			if (s.findhit (ray, t0, t1, myHit)) 
			{
				if (myHit.t < currT) 
				{
					hit = true;
					currT = myHit.t;
				}
			}
		}

		if (hit == true)
			return myHit.color;
		else
			return c;

	}

	/* OnGUI()
 	* Input: none
 	* Output: none
	*/
	void OnGUI() {
		
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), myPicture);

		inFileName = GUI.TextField (new Rect (550, 10, 200, 20), inFileName);

		if (GUI.Button (new Rect (550, 90, 200, 20), "Update Input File")) 
		{
			readFile ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
