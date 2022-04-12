using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace EngiShotgun.Assets
{
	// Token: 0x02000008 RID: 8
	public static class Assets
	{
		// Token: 0x06000028 RID: 40 RVA: 0x000030D4 File Offset: 0x000012D4
		/*public static Texture2D LoadTexture2D(byte[] resourceBytes)
		{
			bool flag = resourceBytes == null;
			bool flag2 = flag;
			if (flag2)
			{
				throw new ArgumentNullException("resourceBytes");
			}
			Texture2D texture2D = new Texture2D(128, 128, DefaultFormat.HDR, TextureCreationFlags.None);
			ImageConversion.LoadImage(texture2D, resourceBytes, false);
			return texture2D;
		}*/

		// Token: 0x06000029 RID: 41 RVA: 0x00003118 File Offset: 0x00001318
		public static Sprite TexToSprite(Texture2D tex)
		{
			return Sprite.Create(tex, new Rect(0f, 0f, (float)tex.width, (float)tex.height), new Vector2(0.5f, 0.5f));
		}
	}
}
