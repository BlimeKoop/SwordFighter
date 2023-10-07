using UnityEngine;
using UnityEditor;

public class AssetImportSettings : AssetPostprocessor
{
    void OnPreprocessModel()
	{
		ModelImporter importer = assetImporter as ModelImporter;
		
		// importer.bakeAxisConversion = true;
		importer.isReadable = true;
	}
}
