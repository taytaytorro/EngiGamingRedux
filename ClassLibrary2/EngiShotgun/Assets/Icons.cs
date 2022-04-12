using UnityEngine;
using System.IO;

//Static class for ease of access
public static class Icons
{
	//The mod's AssetBundle
	public static AssetBundle icons;
	//A constant of the AssetBundle's name.
	public const string bundleName = "icons";
	// Not necesary, but useful if you want to store the bundle on its own folder.
	public const string assetBundleFolder = "assetbundles";

	//The direct path to your AssetBundle
	public static string AssetBundlePath
	{
		get
		{
			//This returns the path to your assetbundle assuming said bundle is on the same folder as your DLL. If you have your bundle in a folder, you can uncomment the statement below this one.
			//return Path.Combine(EngiShotgu.Engiplugin.PInfo.Location, bundleName);
			return Path.Combine(EngiShotgu.Engiplugin.PInfo.Location, assetBundleFolder, bundleName);
		}
	}

	public static void Init()
	{
		//Loads the assetBundle from the Path, and stores it in the static field.
		icons = AssetBundle.LoadFromFile(AssetBundlePath);
	}
}
