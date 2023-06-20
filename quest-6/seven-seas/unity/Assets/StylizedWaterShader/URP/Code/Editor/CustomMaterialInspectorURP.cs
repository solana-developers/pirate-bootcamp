using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public class CustomMaterialInspectorURP : MaterialEditor {
	

	//EditorVariables
	static bool customEditor = true;
	static bool vis = false;
	static bool vertexOffset = true;
	static bool reflections = true;
	float grad1, grad2;
	static bool s1= true,s2= true,s3= true,s4= true,s5 = true;
	Material mat;

	#pragma warning disable 0618
	public override void OnInspectorGUI()
	{
		base.serializedObject.Update();
		var theShader = serializedObject.FindProperty ("m_Shader");

		if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null )
		{
			if (customEditor) {
				GUIStyle t = new GUIStyle ();
				t.alignment = TextAnchor.MiddleCenter;
				Texture title = Resources.Load ("Icons/thumbnail") as Texture;
				GUILayout.Label (title, t);
			}

			Shader shader = theShader.objectReferenceValue as Shader;


			mat = target as Material;

			if (mat.GetFloat ("_VertexOffset") == 1) {
				vertexOffset = true;
			} else {
				vertexOffset = false;
			}


		
						
			mat.globalIlluminationFlags = (MaterialGlobalIlluminationFlags)EditorGUILayout.EnumPopup( "Emission GI", mat.globalIlluminationFlags);

			GUILayout.Space(6);

			//CUSTOM EDITOR SECTIONS
			customEditor = GUILayout.Toggle (customEditor, "Use custom material editor.");
			if (customEditor) {
				GUILayout.BeginHorizontal ();
				Texture coloricon = Resources.Load ("Icons/colorgradient") as Texture;
				GUILayout.Label (coloricon,labelStyle());
				if (foldButton (s1))
					s1 = !s1;
				GUILayout.EndHorizontal ();
				if(s1)
				ColorGradientSection ();

				GUILayout.BeginHorizontal ();
				Texture mainfoamicon = Resources.Load ("Icons/mainFoam") as Texture;
				GUILayout.Label (mainfoamicon,labelStyle());
				if (foldButton (s2))
					s2 = !s2;
				GUILayout.EndHorizontal ();
				if(s2)
				FoamSection ();

				GUILayout.BeginHorizontal ();
				Texture wavesicon = Resources.Load ("Icons/waves") as Texture;
				GUILayout.Label (wavesicon,labelStyle());
				if (foldButton (s3))
					s3 = !s3;
				GUILayout.EndHorizontal ();
				if(s3)
				WavesSection ();
				

				GUILayout.BeginHorizontal ();
				Texture texturesicon = Resources.Load ("Icons/textures") as Texture;
				GUILayout.Label (texturesicon,labelStyle());
				if (foldButton (s5))
					s5 = !s5;
				GUILayout.EndHorizontal ();
				if (s5)
					TexturesSection ();



			} else {
				//DEFAULT EDITOR
				if (this.PropertiesGUI ())
					this.PropertiesChanged ();
			}
		

			GUILayout.Box("Stylized Water Shader created by Marc Sureda, 2021");
		}
	}

	GUIStyle labelStyle(){
		GUIStyle label = new GUIStyle ();
		label.fixedHeight = 25;
		label.alignment = TextAnchor.LowerLeft;
		return label;
	}

	GUIStyle headerStyle(){
		GUIStyle header = new GUIStyle ();
		header.alignment = TextAnchor.MiddleCenter;
		header.fontStyle = FontStyle.Bold;
		header.fontSize = 11;
		return header;
	}


	public void ColorGradientSection(){
		
		
		GUILayout.BeginHorizontal ();
		//ColorPickerHDRConfig hdr = new ColorPickerHDRConfig(0,1,0,1);
		Color c1 = EditorGUILayout.ColorField (GUIContent.none, mat.GetColor ("_DepthGradient1"), false, false, false, GUILayout.MaxWidth(50),GUILayout.MinHeight(30));
		Color c2 = EditorGUILayout.ColorField (GUIContent.none, mat.GetColor ("_DepthGradient2"), false, false, false, GUILayout.MaxWidth(50),GUILayout.MinHeight(30));
		Color c3 = EditorGUILayout.ColorField (GUIContent.none, mat.GetColor ("_DepthGradient3"), false, false, false, GUILayout.MaxWidth(50),GUILayout.MinHeight(30));
		grad1 = mat.GetFloat ("_GradientPosition1");
		grad2 = mat.GetFloat ("_GradientPosition2");
		mat.SetColor("_DepthGradient1", c1);
		mat.SetColor("_DepthGradient2", c2);
		mat.SetColor("_DepthGradient3", c3);

		GUILayout.BeginVertical ();

		grad1 = GUILayout.HorizontalSlider (grad1, 0, 10);
		grad2 = GUILayout.HorizontalSlider (grad2, grad1, 30);
		mat.SetFloat ("_GradientPosition1", grad1);
		mat.SetFloat ("_GradientPosition2", grad2);
		GUILayout.EndVertical ();

		GUILayout.EndHorizontal ();

		GUILayout.Label ("Fresnel");
		GUILayout.BeginHorizontal ();
		Color fr = EditorGUILayout.ColorField (GUIContent.none, mat.GetColor ("_FresnelColor"), false, false, false, GUILayout.MaxWidth(50),GUILayout.MinHeight(20));
		mat.SetColor("_FresnelColor", fr);
		float frexp = mat.GetFloat("_FresnelExp");
		frexp = GUILayout.HorizontalSlider (frexp, 0, 5);
		mat.SetFloat ("_FresnelExp", frexp);
		GUILayout.EndHorizontal ();

		RangeProperty("_LightColorIntensity", "Light Color Intensity",0,1);
		RangeProperty("_Roughness", "Roughness",0,1);
		GUILayout.BeginHorizontal ();

		GUILayout.EndHorizontal ();
		GUILayout.Space (5);
		RangeProperty("_Opacity", "Water Opacity",0,1);
		FloatProperty("_OpacityDepth", "Opacity Depth");
		GUILayout.Space (5);
	}

	public void FoamSection(){

		
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Foam Color");
		//ColorPickerHDRConfig hdr = new ColorPickerHDRConfig(0,1,0,1);
		Color foamcolor = EditorGUILayout.ColorField (GUIContent.none, mat.GetColor ("_FoamColor"), false, false, false, GUILayout.MaxWidth(50),GUILayout.MinHeight(20));
		mat.SetColor("_FoamColor", foamcolor);
		GUILayout.EndHorizontal();
		GUILayout.BeginVertical ();
		GUILayout.Label ("Main Foam", headerStyle());
		RangeProperty("_MainFoamOpacity", "Opacity",0,1);
		FloatProperty("_MainFoamScale", "Scale");
		FloatProperty("_MainFoamIntensity", "Intensity");
		FloatProperty("_MainFoamSpeed", "Speed");
		GUILayout.EndVertical ();

		GUILayout.BeginVertical ();
		GUILayout.Label ("Secondary Foam", headerStyle());
		RangeProperty("_SecondaryFoamOpacity", "Opacity",0,1);
		FloatProperty("_SecondaryFoamScale", "Scale");
		FloatProperty("_SecondaryFoamIntensity", "Intensity");

		vis = EditorGUILayout.Toggle ("Always Visible", vis);
		if (vis) {
			mat.SetFloat ("_SecondaryFoamAlwaysVisible", 1);
		} else {
			mat.SetFloat ("_SecondaryFoamAlwaysVisible", 0);
		}
			
		GUILayout.EndVertical ();
		GUILayout.Space (5);
	}






	public void TexturesSection(){
		GUILayout.Space (10);
		MaterialProperty tex = MaterialEditor.GetMaterialProperty (targets, "_DistortionTexture");
		TextureProperty (tex, "Distortion Texture (Multi Channel)");

		MaterialProperty tex2 = MaterialEditor.GetMaterialProperty (targets, "_FoamTexture");
		TextureProperty (tex2, "Foam Texture");
	}


	public void WavesSection(){
		FloatProperty ("_WavesAmplitude", "Amplitude");
		FloatProperty ("_WavesSpeed", "Speed");
		FloatProperty ("_WavesIntensity", "Intensity");
		FloatProperty ("_WaveDistortionIntensity", "Wave Distortion");
		FloatProperty ("_TurbulenceDistortionIntesity", "Turbulence Distortion");
		FloatProperty ("_TurbulenceScale", "Turbulence Scale");

		GUILayout.BeginHorizontal ();
//		float direction;
//		direction = mat.GetFloat ("_WavesDirection");
		RangeProperty ("_WavesDirection", "Direction",0,360);
		GUILayout.EndHorizontal ();

	
		vertexOffset = EditorGUILayout.Toggle ("Vertex Offset", vertexOffset);
		if (vertexOffset) {
			mat.SetFloat ("_VertexOffset", 1);
		} else {
			mat.SetFloat ("_VertexOffset", 0);
		}
		GUILayout.Space (5);

	}
		

	public bool foldButton(bool b){
		string path = "Icons/fold";
		if (b)
			path = "Icons/unfold";
				
		Texture fold = Resources.Load (path) as Texture;

		Color c = GUI.backgroundColor;
		GUI.backgroundColor = Color.clear;
		bool v = GUILayout.Button (fold, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25));
		GUI.backgroundColor = c;
		return v;
	}

		#pragma warning restore 0618
}
