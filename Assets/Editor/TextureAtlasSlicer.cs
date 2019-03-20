using System;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class TextureAtlasSlicer : EditorWindow
{
	[MenuItem("CONTEXT/TextureImporter/Slice Sprite Using XML")]
	public static void SliceUsingXML(MenuCommand command)
	{
		TextureImporter textureImporter = command.context as TextureImporter;
		TextureAtlasSlicer window = CreateInstance<TextureAtlasSlicer>();
		window.importer = textureImporter;
		window.ShowUtility();
	}

	[MenuItem("Assets/Slice Sprite Using XML")]
	public static void TextureAtlasSlicerWindow()
	{
		TextureAtlasSlicer window = CreateInstance<TextureAtlasSlicer>();
		window.Show();
	}

	[MenuItem("CONTEXT/TextureImporter/Slice Sprite Using XML", true)]
	public static bool ValidateSliceUsingXML(MenuCommand command)
	{
		TextureImporter textureImporter = command.context as TextureImporter;
		return textureImporter && textureImporter.textureType == TextureImporterType.Sprite || textureImporter.textureType == TextureImporterType.Default;
	}

    struct SubTexture
    {
        public int width;
        public int height;
        public int x;
        public int y;
		public float frameX;
		public float frameY;
		public Vector2 pivot;
		public SpriteAlignment alignment;
        public string name;

		public SubTexture(XmlNode node)
		{
			width = Convert.ToInt32(node.Attributes["width"].Value);
			height = Convert.ToInt32(node.Attributes["height"].Value);
			x = Convert.ToInt32(node.Attributes["x"].Value);
			y = Convert.ToInt32(node.Attributes["y"].Value);
			frameX = node.Attributes["frameX"] == null ? 0 : Convert.ToSingle(node.Attributes["frameX"].Value);
			frameY = node.Attributes["frameY"] == null ? 0 : Convert.ToSingle(node.Attributes["frameY"].Value);
			pivot = Vector2.zero;
			alignment = SpriteAlignment.Center;
			name= node.Attributes["name"].Value;
			SetPivotAlignment(node, this);
		}
    }

	static Vector2 lastPivot;

    public TextureImporter importer;

	[SerializeField]
    TextAsset xmlAsset;
    Texture2D selectedTexture;
    SubTexture[] subTextures;
    int wantedWidth, wantedHeight;

    public TextureAtlasSlicer()
    {
        titleContent = new GUIContent("XML Slicer");
    }

    void UseSelectedTexture()
    {
        if (Selection.objects.Length > 1)
        {
            selectedTexture = null;
        }
        else
        {
            selectedTexture = Selection.activeObject as Texture2D;
        }

        if (selectedTexture != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedTexture);
            importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer)
            {
                string extension = Path.GetExtension(assetPath);
                string pathWithoutExtension = assetPath.Remove(assetPath.Length - extension.Length, extension.Length);
                string xmlPath = pathWithoutExtension + ".xml";
                UnityEngine.Object textAsset = AssetDatabase.LoadAssetAtPath(xmlPath, typeof(TextAsset));

                if (textAsset != null)
                {
                    xmlAsset = textAsset as TextAsset;
                }
                else
                {
                    xmlAsset = null;
                    subTextures = null;
                }
                ParseXML();
            }
            else
            {
                xmlAsset = null;
                subTextures = null;
            }
        }
        else
        {
            importer = null;
            xmlAsset = null;
            subTextures = null;
        }
        Repaint();
    }

    void ParseXML()
    {
        try
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlAsset.text);
            XmlElement root = document.DocumentElement;
            if (root == null || root.Name != "TextureAtlas")
            {
                return;
            }

            subTextures = root.ChildNodes
				.Cast<XmlNode>()
				.Where(childNode => childNode.Name == "SubTexture")
				.Select(childNode => new SubTexture(childNode))
					.ToArray();

            wantedWidth = 0;
            wantedHeight = 0;

            foreach (SubTexture subTexture in subTextures)
            {
                int right = subTexture.x + subTexture.width;
                int bottom = subTexture.y + subTexture.height;

                wantedWidth = Mathf.Max(wantedWidth, right);
                wantedHeight = Mathf.Max(wantedHeight, bottom);
            }
        }
        catch (Exception e)
        {
            subTextures = null;
			Debug.LogException(e);
        }
    }

	public void OnEnable()
	{
		UseSelectedTexture();
	}

	public void OnGUI()
	{
		if (importer == null)
		{
			EditorGUILayout.LabelField("Please select a texture to slice");
			return;
		}

		EditorGUI.BeginDisabledGroup(focusedWindow != this);
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ObjectField("Texture", Selection.activeObject, typeof (Texture), false);
			EditorGUI.EndDisabledGroup();

			if (importer.textureType != TextureImporterType.Sprite && importer.textureType != TextureImporterType.Default)
				EditorGUILayout.LabelField("The Texture Type needs to be Sprite or Advanced!");

			EditorGUI.BeginDisabledGroup((importer.textureType != TextureImporterType.Sprite && importer.textureType != TextureImporterType.Default));
			{
				EditorGUI.BeginChangeCheck();
				xmlAsset = EditorGUILayout.ObjectField("XML Source", xmlAsset, typeof (TextAsset), false) as TextAsset;
				if (EditorGUI.EndChangeCheck())
					ParseXML();

				bool needResize = wantedWidth > selectedTexture.width || wantedHeight > selectedTexture.height;

				if (xmlAsset != null && needResize)
				{
					EditorGUILayout.LabelField(string.Format("Texture size too small, it needs to be at least {0} by {1} pixels!", wantedWidth, wantedHeight));
					EditorGUILayout.LabelField("Try changing the Max Size property in the importer.");
				}

				if (subTextures == null || subTextures.Length == 0)
					EditorGUILayout.LabelField("Could not find any SubTextures in XML.");

				EditorGUI.BeginDisabledGroup(xmlAsset == null || needResize || subTextures == null || subTextures.Length == 0);
				if (GUILayout.Button("Slice"))
					PerformSlice();
				EditorGUI.EndDisabledGroup();
			}
			EditorGUI.EndDisabledGroup();
		}
		EditorGUI.EndDisabledGroup();
	}

    void PerformSlice()
    {
        if (importer == null)
        {
            return;
        }

        int textureHeight = selectedTexture.height;
        bool needsUpdate = false;

        if (importer.spriteImportMode != SpriteImportMode.Multiple)
        {
            needsUpdate = true;
            importer.spriteImportMode = SpriteImportMode.Multiple;
        }

        SpriteMetaData[] wantedSpriteSheet = (from subTexture in subTextures
                                 let actualY = textureHeight - (subTexture.y + subTexture.height)
                                 select new SpriteMetaData
                                 {
                                     alignment = (int)subTexture.alignment,
                                     border = new Vector4(),
                                     name = subTexture.name,
                                     pivot = subTexture.pivot,
                                     rect = new Rect(subTexture.x, actualY, subTexture.width, subTexture.height)
                                 }).ToArray();
        if (!needsUpdate && !importer.spritesheet.SequenceEqual(wantedSpriteSheet))
        {
            needsUpdate = true;
            importer.spritesheet = wantedSpriteSheet;
        }

        if (needsUpdate)
        {
            EditorUtility.SetDirty(importer);

            try
            {
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportAsset(importer.assetPath);

                EditorUtility.DisplayDialog("Success!", "The sprite was sliced successfully.", "OK");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Error", "There was an exception while trying to reimport the image." +
                                                     "\nPlease check the console log for details.", "OK");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Nope!", "The sprite is already sliced according to this XML file.", "OK");
        }
    }

	static void SetPivotAlignment(XmlNode node, SubTexture st)
	{
		if (node.Attributes["pivotX"] == null)
			st.pivot = lastPivot;
		else
		{
			st.pivot = new Vector2((Convert.ToSingle(node.Attributes["pivotX"].Value) + st.frameX) / st.width, 
				(Convert.ToSingle(node.Attributes["pivotY"].Value) + st.frameY) / st.height);
			lastPivot = st.pivot;
		}

		if (st.pivot.y == 0)
		{
			if (st.pivot.x == 0)
				st.alignment = SpriteAlignment.TopLeft;
			else if (st.pivot.x == 0.5f)
				st.alignment = SpriteAlignment.TopCenter;
			else if (st.pivot.x == 1)
				st.alignment = SpriteAlignment.TopRight;
		}
		else if (st.pivot.y == 0.5f)
		{
			if (st.pivot.x == 0)
				st.alignment = SpriteAlignment.LeftCenter;
			else if (st.pivot.x == 0.5f)
				st.alignment = SpriteAlignment.Center;
			else if (st.pivot.x == 1)
				st.alignment = SpriteAlignment.RightCenter;
		}
		else if (st.pivot.y == 1)
		{
			if (st.pivot.x == 0)
				st.alignment = SpriteAlignment.BottomLeft;
			else if (st.pivot.x == 0.5f)
				st.alignment = SpriteAlignment.BottomCenter;
			else if (st.pivot.x == 1)
				st.alignment = SpriteAlignment.BottomRight;
		}
		else
			st.alignment = SpriteAlignment.Custom;
	}
}