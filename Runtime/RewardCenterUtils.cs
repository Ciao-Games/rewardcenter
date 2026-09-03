using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CiaoGames.RewardCenter
{
    public class RewardCenterUtils
    {
        /// <summary>
        /// Parses a URL query string ("?a=1&amp;b=2" or "a=1&amp;b=2") into a dictionary.
        /// Values are URL-decoded.
        /// </summary>
        public static Dictionary<string, string> ParseQuery(string query)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(query)) return result;
            if (query.StartsWith("?")) query = query.Substring(1);
 
            foreach (var pair in query.Split('&'))
            {
                var kv = pair.Split(new[] { '=' }, 2);
                if (kv.Length == 2)
                {
                    result[Uri.UnescapeDataString(kv[0])] = Uri.UnescapeDataString(kv[1]);
                }
            }
            return result;
        }
        
        public static Sprite LoadSpriteFromDisk(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
            var bytes = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2);
            if (!tex.LoadImage(bytes)) return null;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}