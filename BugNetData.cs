﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace BugCatching
{
    public class BugNetData
    {
        public List<CritterEntry> AllCritters { get; set; } = new List<CritterEntry>();
    }
    public class DataInjector : IAssetEditor
    {
        internal IModHelper Helper;
        public static Texture2D ToolsSprites;

        public DataInjector(IModHelper helper)
        {
            Helper = helper;
            ToolsSprites = Helper.Content.Load<Texture2D>("Assets/bugnet.png");
            Helper.Content.AssetEditors.Add(this);
            //ToolsLoader = new ToolsLoader(ToolsSprites, Helper.Content.Load<Texture2D>("tools/MenuTiles.png"), Helper.Content.Load<Texture2D>("common/CustomLetterBG.png"));  
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\ObjectInformation") || asset.AssetNameEquals("TileSheets\\tools");
        }
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\ObjectInformation"))
            {
                var data = asset.AsDictionary<int, string>().Data;
                
                foreach (BugModel bugModel in BugCatchingMod.AllBugs) {
                    data.Add(bugModel.ParentSheetIndex, bugModel.QuickItemDataString);
                    
                }
            }
            if (asset.AssetNameEquals("TileSheets\\tools"))
            {
                Texture2D toolSpriteSheet = asset.AsImage().Data;
                Color[] originalTools = new Color[toolSpriteSheet.Width * toolSpriteSheet.Height];
                toolSpriteSheet.GetData<Color>(originalTools);

                Texture2D bugNetToolSpriteSheet = ToolsSprites;
                Color[] addonTools = new Color[bugNetToolSpriteSheet.Width * bugNetToolSpriteSheet.Height];
                bugNetToolSpriteSheet.GetData<Color>(addonTools);

                Texture2D newSpriteSheet = new Texture2D(Game1.game1.GraphicsDevice, toolSpriteSheet.Width, toolSpriteSheet.Height + bugNetToolSpriteSheet.Height, false, SurfaceFormat.Color);
                var allTools = new Color[originalTools.Length + addonTools.Length];
                originalTools.CopyTo(allTools, 0);
                addonTools.CopyTo(allTools, originalTools.Length);
                newSpriteSheet.SetData(allTools);
                asset.ReplaceWith(newSpriteSheet);
            }
        }
    }
}
