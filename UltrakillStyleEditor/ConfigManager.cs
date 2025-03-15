using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using PluginConfig.API.Functionals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using CharacterInfo = PluginConfig.API.Fields.CharacterInfo;

namespace UltrakillStyleEditor
{
    public static class ConfigManager
    {
        public static PluginConfigurator config;
        public static Dictionary<string, FormattedStringField> styleDic = new Dictionary<string, FormattedStringField>();

        public static ConfigPanel killStylePanel;
        public static ConfigPanel hurtStylePanel;
        public static ConfigPanel miscStylePanel;
        public static ConfigPanel enviroStylePanel;        

        public static ConfigPanel unknownStylePanel;

        public static StringField killPanelSearchbar;
        public static ButtonArrayField killPanelSearchButton;

        public static StringField hurtPanelSearchbar;
        public static ButtonArrayField hurtPanelSearchButton;

        public static StringField enviroPanelSearchbar;
        public static ButtonArrayField enviroPanelSearchButton;

        public static StringField miscPanelSearchbar;
        public static ButtonArrayField miscPanelSearchButton;
        public static ButtonField unknownPanelSearchButton;

		public static FormattedStringField GetOrCreateField(string id)
		{
			if (styleDic.TryGetValue(id, out FormattedStringField field))
				return field;

			if (string.IsNullOrEmpty(id))
				return null;

            string defaultRawValue = id;
            if (StyleHUD.Instance != null && StyleHUD.Instance.idNameDict.TryGetValue(id, out string text))
                defaultRawValue = text;

			var configField = new FormattedStringField(ConfigManager.unknownStylePanel, id, id, Utils.FormattedStringFromFormattedText(defaultRawValue), true);
			styleDic.Add(id, configField);
			AddValueChangeListener(id, configField);

			return configField;
		}

		public static void AddValueChangeListener(string id, FormattedStringField field)
        {
            field.onValueChange += (FormattedStringField.FormattedStringValueChangeEvent e) =>
            {
                FormattedStringBuilder builder = new FormattedStringBuilder();

                List<CharacterInfo> format = e.formattedString.GetFormat();
                string rawText = e.formattedString.rawString;

                for (int i = 0; i < format.Count; i++)
                {
                    if (rawText[i] == '+')
                        continue;
                    builder.currentFormat = format[i];
                    builder.Append(rawText[i]);
                }

                e.formattedString = builder.Build();

                if (StyleHUD.instance == null)
                    return;

                StyleHUD.instance.idNameDict[id] = e.formattedString.formattedString;
            };

            field.TriggerValueChangeEvent();
        }

        private static void MakeSearchBar(StringField searchBar, ButtonArrayField searchButton, ConfigPanel targetPanel)
        {
            searchButton.OnClickEventHandler(0).onClick += () =>
            {
                string filter = searchBar.value.ToLower();
                bool emptyFilter = string.IsNullOrWhiteSpace(filter);
                foreach (KeyValuePair<string, FormattedStringField> pair in styleDic)
                {
                    if (pair.Value.parentPanel != targetPanel)
                        continue;

                    if (emptyFilter)
                    {
                        pair.Value.hidden = false;
                        continue;
                    }

                    pair.Value.hidden = !pair.Key.ToLower().Contains(filter);
                }
            };
            searchButton.OnClickEventHandler(1).onClick += () =>
            {
                searchBar.value = "";
                foreach (KeyValuePair<string, FormattedStringField> pair in styleDic)
                {
                    if (pair.Value.parentPanel != targetPanel)
                        continue;

                    pair.Value.hidden = false;
                }
            };
        }

        private static bool inited = false;
        public static void Init()
        {
            if (inited)
                return;
            inited = true;

            config = PluginConfigurator.Create("Style Editor", Plugin.PLUGIN_GUID);
            string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconPath = Path.Combine(pluginPath, "icon.png");
            config.SetIconWithURL("file://" + iconPath);

            // ROOT PANEL
            new ConfigHeader(config.rootPanel, "Ultrakill Styles");
            killStylePanel = new ConfigPanel(config.rootPanel, "Kill Related Styles", "killStylePanel");
            hurtStylePanel = new ConfigPanel(config.rootPanel, "Hurt Related Styles", "hurtStylePanel");
            enviroStylePanel = new ConfigPanel(config.rootPanel, "Environmental Styles", "enviroStylePanel");
            miscStylePanel = new ConfigPanel(config.rootPanel, "Misc Styles", "miscStylePanel");

            new ConfigHeader(config.rootPanel, "Additional Styles");
            unknownStylePanel = new ConfigPanel(config.rootPanel, "Unknown Styles", "unknownStylePanel");

            //styleDic.Add("", new FormattedStringField(killStylePanel, "", "", Utils.FormattedStringFromFormattedText(""), true));

            // KILL RELATED STYLES
            killPanelSearchbar = new StringField(killStylePanel, "Search Bar", "killPanelSearchbar", "", true, false);
            killPanelSearchButton = new ButtonArrayField(killStylePanel, "killPanelSearchButton", 2, new float[] {0.5f, 0.5f}, new string[] {"Search", "Clear Filter"});
            MakeSearchBar(killPanelSearchbar, killPanelSearchButton, killStylePanel);

            styleDic.Add("ultrakill.kill", new FormattedStringField(killStylePanel, "KILL", "ultrakill.kill", Utils.FormattedStringFromFormattedText("KILL"), true));
            styleDic.Add("ultrakill.doublekill", new FormattedStringField(killStylePanel, "<color=orange>DOUBLE KILL</color>", "ultrakill.doublekill", Utils.FormattedStringFromFormattedText("<color=orange>DOUBLE KILL</color>"), true));
            styleDic.Add("ultrakill.triplekill", new FormattedStringField(killStylePanel, "<color=orange>TRIPLE KILL</color>", "ultrakill.triplekill", Utils.FormattedStringFromFormattedText("<color=orange>TRIPLE KILL</color>"), true));
            styleDic.Add("ultrakill.multikill", new FormattedStringField(killStylePanel, "<color=orange>MULTIKILL</color>", "ultrakill.multikill", Utils.FormattedStringFromFormattedText("<color=orange>MULTIKILL</color>"), true));
            styleDic.Add("ultrakill.arsenal", new FormattedStringField(killStylePanel, "<color=cyan>ARSENAL</color>", "ultrakill.arsenal", Utils.FormattedStringFromFormattedText("<color=cyan>ARSENAL</color>"), true));
            styleDic.Add("ultrakill.instakill", new FormattedStringField(killStylePanel, "<color=lime>INSTAKILL</color>", "ultrakill.instakill", Utils.FormattedStringFromFormattedText("<color=lime>INSTAKILL</color>"), true));
            styleDic.Add("ultrakill.interruption", new FormattedStringField(killStylePanel, "<color=lime>INTERRUPTION</color>", "ultrakill.interruption", Utils.FormattedStringFromFormattedText("<color=lime>INTERRUPTION</color>"), true));
            styleDic.Add("ultrakill.headshot", new FormattedStringField(killStylePanel, "HEADSHOT", "ultrakill.headshot", Utils.FormattedStringFromFormattedText("HEADSHOT"), true));
            styleDic.Add("ultrakill.bigheadshot", new FormattedStringField(killStylePanel, "BIG HEADSHOT", "ultrakill.bigheadshot", Utils.FormattedStringFromFormattedText("BIG HEADSHOT"), true));        
            styleDic.Add("ultrakill.limbhit", new FormattedStringField(killStylePanel, "LIMB HIT", "ultrakill.limbhit", Utils.FormattedStringFromFormattedText("LIMB HIT"), true));   
            styleDic.Add("ultrakill.bigkill", new FormattedStringField(killStylePanel, "BIG KILL", "ultrakill.bigkill", Utils.FormattedStringFromFormattedText("BIG KILL"), true));
            styleDic.Add("ultrakill.bigfistkill", new FormattedStringField(killStylePanel, "BIG FIST KILL", "ultrakill.bigfistkill", Utils.FormattedStringFromFormattedText("BIG FISTKILL"), true));
            styleDic.Add("ultrakill.criticalpunch", new FormattedStringField(killStylePanel, "CRITICAL PUNCH", "ultrakill.criticalpunch", Utils.FormattedStringFromFormattedText("CRITICAL PUNCH"), true));
            styleDic.Add("ultrakill.iconoclasm", new FormattedStringField(killStylePanel, "ICONOCLASM", "ultrakill.iconoclasm", Utils.FormattedStringFromFormattedText("ICONOCLASM"), true));
            styleDic.Add("ultrakill.groundslam", new FormattedStringField(killStylePanel, "GROUND SLAM", "ultrakill.groundslam", Utils.FormattedStringFromFormattedText("GROUND SLAM"), true));
            styleDic.Add("ultrakill.airslam", new FormattedStringField(killStylePanel, "<color=cyan>AIR SLAM</color>", "ultrakill.airslam", Utils.FormattedStringFromFormattedText("<color=cyan>AIR SLAM</color>"), true));
            styleDic.Add("ultrakill.airshot", new FormattedStringField(killStylePanel, "<color=cyan>AIRSHOT</color>", "ultrakill.airshot", Utils.FormattedStringFromFormattedText("<color=cyan>AIRSHOT</color>"), true));
            styleDic.Add("ultrakill.fireworks", new FormattedStringField(killStylePanel, "<color=cyan>FIREWORKS</color>", "ultrakill.fireworks", Utils.FormattedStringFromFormattedText("<color=cyan>FIREWORKS</color>"), true));
            styleDic.Add("ultrakill.splattered", new FormattedStringField(killStylePanel, "SPLATTERED", "ultrakill.splattered", Utils.FormattedStringFromFormattedText("SPLATTERED"), true));
            styleDic.Add("ultrakill.exploded", new FormattedStringField(killStylePanel, "EXPLODED", "ultrakill.exploded", Utils.FormattedStringFromFormattedText("EXPLODED"), true));
            styleDic.Add("ultrakill.fried", new FormattedStringField(killStylePanel, "FRIED", "ultrakill.fried", Utils.FormattedStringFromFormattedText("FRIED"), true));
            styleDic.Add("ultrakill.finishedoff", new FormattedStringField(killStylePanel, "<color=cyan>FINISHED OFF</color>", "ultrakill.finishedoff", Utils.FormattedStringFromFormattedText("<color=cyan>FINISHED OFF</color>"), true));
            styleDic.Add("ultrakill.mauriced", new FormattedStringField(killStylePanel, "MAURICED", "ultrakill.mauriced", Utils.FormattedStringFromFormattedText("MAURICED"), true));
            styleDic.Add("ultrakill.roundtrip", new FormattedStringField(killStylePanel, "ROUND TRIP", "ultrakill.roundtrip", Utils.FormattedStringFromFormattedText("<color=lime>ROUND TRIP</color>"), true));
            styleDic.Add("ultrakill.overkill", new FormattedStringField(killStylePanel, "OVERKILL", "ultrakill.overkill", Utils.FormattedStringFromFormattedText("OVERKILL"), true));
            styleDic.Add("GROOVY", new FormattedStringField(killStylePanel, "GROOVY", "GROOVY", Utils.FormattedStringFromFormattedText("GROOVY"), true));
            styleDic.Add("NO-NO", new FormattedStringField(killStylePanel, "NO-NO", "NO-NO", Utils.FormattedStringFromFormattedText("NO-NO"), true));
            styleDic.Add("RE-NO-NO", new FormattedStringField(killStylePanel, "RE-NO-NO", "RE-NO-NO", Utils.FormattedStringFromFormattedText("RE-NO-NO"), true));
            styleDic.Add("UNCHAINEDSAW", new FormattedStringField(killStylePanel, "UNCHAINEDSAW", "UNCHAINEDSAW", Utils.FormattedStringFromFormattedText("UNCHAINEDSAW"), true));
            styleDic.Add("ultrakill.attripator", new FormattedStringField(killStylePanel, "<color=cyan>ATTRAPTOR</color>", "ultrakill.attripator", Utils.FormattedStringFromFormattedText("<color=cyan>ATTRAPTOR</color>"), true));
            styleDic.Add("ultrakill.bipolar", new FormattedStringField(killStylePanel, "BIPOLAR", "ultrakill.bipolar", Utils.FormattedStringFromFormattedText("BIPOLAR"), true));
            styleDic.Add("ultrakill.catapulted", new FormattedStringField(killStylePanel, "<color=cyan>CATAPULTED</color>", "ultrakill.catapulted", Utils.FormattedStringFromFormattedText("<color=cyan>CATAPULTED</color>"), true));
            styleDic.Add("ultrakill.nailbombed", new FormattedStringField(killStylePanel, "NAILBOMBED", "ultrakill.nailbombed", Utils.FormattedStringFromFormattedText("NAILBOMBED"), true));
            styleDic.Add("SCREWED", new FormattedStringField(killStylePanel, "SCREWED", "SCREWED", Utils.FormattedStringFromFormattedText("SCREWED"), true));
            styleDic.Add("ultrakill.drillpunchkill", new FormattedStringField(killStylePanel, "<color=green>GIGA DRILL BREAK</color>", "ultrakill.drillpunchkill", Utils.FormattedStringFromFormattedText("<color=green>GIGA DRILL BREAK</color>"), true));
            styleDic.Add("ultrakill.cannonballed", new FormattedStringField(killStylePanel, "CANNONBALLED", "ultrakill.cannonballed", Utils.FormattedStringFromFormattedText("CANNONBALLED"), true));
            styleDic.Add("ultrakill.halfoff", new FormattedStringField(killStylePanel, "<color=cyan>HALF OFF</color>", "ultrakill.halfoff", Utils.FormattedStringFromFormattedText("<color=cyan>HALF OFF</color>"), true));
            styleDic.Add("ultrakill.hammerhitgreen", new FormattedStringField(killStylePanel, "BLUNT FORCE", "ultrakill.hammerhitgreen", Utils.FormattedStringFromFormattedText("BLUNT FORCE"), true));
            styleDic.Add("ultrakill.hammerhityellow", new FormattedStringField(killStylePanel, "HEAVY HITTER", "ultrakill.hammerhityellow", Utils.FormattedStringFromFormattedText("HEAVY HITTER"), true));
            styleDic.Add("ultrakill.hammerhitred", new FormattedStringField(killStylePanel, "FULL IMPACT", "ultrakill.hammerhitred", Utils.FormattedStringFromFormattedText("FULL IMPACT"), true));            
            styleDic.Add("ultrakill.compressed", new FormattedStringField(killStylePanel, "COMPRESSED", "ultrakill.compressed", Utils.FormattedStringFromFormattedText("COMPRESSED"), true));
            
            // HURT RELATED STYLES
            hurtPanelSearchbar = new StringField(hurtStylePanel, "Search Bar", "hurtPanelSearchbar", "", true, false);
            hurtPanelSearchButton = new ButtonArrayField(hurtStylePanel, "hurtPanelSearchButton", 2, new float[] {0.5f, 0.5f}, new string[] {"Search", "Clear Filter"});
            MakeSearchBar(hurtPanelSearchbar, hurtPanelSearchButton, hurtStylePanel);

            styleDic.Add("ultrakill.headshotcombo", new FormattedStringField(hurtStylePanel, "<color=cyan>HEADSHOT COMBO</color>", "ultrakill.headshotcombo", Utils.FormattedStringFromFormattedText("<color=cyan>HEADSHOT COMBO</color>"), true));
            styleDic.Add("ultrakill.ricoshot", new FormattedStringField(hurtStylePanel, "<color=cyan>RICOSHOT</color>", "ultrakill.ricoshot", Utils.FormattedStringFromFormattedText("<color=cyan>RICOSHOT</color>"), true));
            styleDic.Add("ultrakill.fistfullofdollar", new FormattedStringField(hurtStylePanel, "<color=cyan>FISTFUL OF DOLLAR</color>", "ultrakill.fistfullofdollar", Utils.FormattedStringFromFormattedText("<color=cyan>FISTFUL OF DOLLAR</color>"), true));
            styleDic.Add("ultrakill.disrespect", new FormattedStringField(hurtStylePanel, "DISRESPECT", "ultrakill.disrespect", Utils.FormattedStringFromFormattedText("DISRESPECT"), true));
            styleDic.Add("ultrakill.projectileboost", new FormattedStringField(hurtStylePanel, "<color=lime>PROJECTILE BOOST</color>", "ultrakill.projectileboost", Utils.FormattedStringFromFormattedText("<color=lime>PROJECTILE BOOST</color>"), true));
            styleDic.Add("ultrakill.fireworksweak", new FormattedStringField(hurtStylePanel, "<color=cyan>JUGGLE</color>", "ultrakill.fireworksweak", Utils.FormattedStringFromFormattedText("<color=cyan>JUGGLE</color>"), true));
            styleDic.Add("ultrakill.nailbombedalive", new FormattedStringField(hurtStylePanel, "<color=grey>NAILBOMBED</color>", "ultrakill.nailbombedalive", Utils.FormattedStringFromFormattedText("<color=grey>NAILBOMBED</color>"), true));
            styleDic.Add("ultrakill.conductor", new FormattedStringField(hurtStylePanel, "<color=cyan>CONDUCTOR</color>", "ultrakill.conductor", Utils.FormattedStringFromFormattedText("<color=cyan>CONDUCTOR</color>"), true));
            styleDic.Add("ultrakill.drillpunch", new FormattedStringField(hurtStylePanel, "<color=green>CORKSCREW BLOW</color>", "ultrakill.drillpunch", Utils.FormattedStringFromFormattedText("<color=green>CORKSCREW BLOW</color>"), true));
            styleDic.Add("ultrakill.cannonboost", new FormattedStringField(hurtStylePanel, "<color=green>CANNONBOOST</color>", "ultrakill.cannonboost", Utils.FormattedStringFromFormattedText("<color=green>CANNONBOOST</color>"), true));
            styleDic.Add("ultrakill.cannonballedfrombounce", new FormattedStringField(hurtStylePanel, "<color=green>DUNKED</color>", "ultrakill.cannonballedfrombounce", Utils.FormattedStringFromFormattedText("<color=green>DUNKED</color>"), true));
            styleDic.Add("ultrakill.insurrknockdown", new FormattedStringField(hurtStylePanel, "<color=green>TIME OUT</color>", "ultrakill.insurrknockdown", Utils.FormattedStringFromFormattedText("<color=green>TIME OUT</color>"), true));
            styleDic.Add("ultrakill.hammerhitheavy", new FormattedStringField(hurtStylePanel, "BLASTING AWAY", "ultrakill.hammerhitheavy", Utils.FormattedStringFromFormattedText("BLASTING AWAY"), true));
            styleDic.Add("ultrakill.friendlyfire", new FormattedStringField(hurtStylePanel, "FRIENDLY FIRE", "ultrakill.friendlyfire", Utils.FormattedStringFromFormattedText("FRIENDLY FIRE"), true));
            styleDic.Add("ultrakill.homerun", new FormattedStringField(hurtStylePanel, "HOMERUN", "ultrakill.homerun", Utils.FormattedStringFromFormattedText("HOMERUN"), true));
            styleDic.Add("ultrakill.strike", new FormattedStringField(hurtStylePanel, "<color=cyan>STRIKE!</color>", "ultrakill.strike", Utils.FormattedStringFromFormattedText("<color=cyan>STRIKE!</color>"), true));
            styleDic.Add("<color=green>GUARD BREAK</color>", new FormattedStringField(hurtStylePanel, "<color=green>GUARD BREAK</color>", "<color=green>GUARD BREAK</color>", Utils.FormattedStringFromFormattedText("<color=green>GUARD BREAK"), true));
            styleDic.Add("ultrakill.serve", new FormattedStringField(hurtStylePanel, "<color=cyan>SERVED</color>", "ultrakill.serve", Utils.FormattedStringFromFormattedText("<color=cyan>SERVED</color>"), true));
            styleDic.Add("ultrakill.landyours", new FormattedStringField(hurtStylePanel, "<color=green>LANDYOURS</color>", "ultrakill.landyours", Utils.FormattedStringFromFormattedText("<color=green>LANDYOURS</color>"), true));
            styleDic.Add("ultrakill.rocketreturn", new FormattedStringField(hurtStylePanel, "<color=cyan>ROCKET RETURN</color>", "ultrakill.rocketreturn", Utils.FormattedStringFromFormattedText("<color=cyan>ROCKET RETURN</color>"), true));
            styleDic.Add("ultrakill.shotgunhit", new FormattedStringField(hurtStylePanel, "SHOTGUN HIT", "ultrakill.shotgunhit", Utils.FormattedStringFromFormattedText(""), true));
            styleDic.Add("ultrakill.nailhit", new FormattedStringField(hurtStylePanel, "NAIL HIT", "ultrakill.nailhit", Utils.FormattedStringFromFormattedText(""), true));
            styleDic.Add("ultrakill.drillhit", new FormattedStringField(hurtStylePanel, "DRILL HIT", "ultrakill.drillhit", Utils.FormattedStringFromFormattedText(""), true));
            styleDic.Add("ultrakill.hammerhit", new FormattedStringField(hurtStylePanel, "HAMMER HIT", "ultrakill.hammerhit", Utils.FormattedStringFromFormattedText(""), true));
            styleDic.Add("ultrakill.zapperhit", new FormattedStringField(hurtStylePanel, "ZAPPER HIT", "ultrakill.zapperhit", Utils.FormattedStringFromFormattedText(""), true));
            styleDic.Add("ultrakill.explosionhit", new FormattedStringField(hurtStylePanel, "EXPLOSION HIT", "ultrakill.explosionhit", Utils.FormattedStringFromFormattedText(""), true));
            styleDic.Add("ultrakill.firehit", new FormattedStringField(hurtStylePanel, "FIRE HIT", "ultrakill.firehit", Utils.FormattedStringFromFormattedText(""), true));
            
            // ENVIRONMENTAL RELATED STYLES
            enviroPanelSearchbar = new StringField(enviroStylePanel, "Search Bar", "enviroPanelSearchbar", "", true, false);
            enviroPanelSearchButton = new ButtonArrayField(enviroStylePanel, "enviroPanelSearchButton", 2, new float[] {0.5f, 0.5f}, new string[] {"Search", "Clear Filter"});
            MakeSearchBar(enviroPanelSearchbar, enviroPanelSearchButton, enviroStylePanel);

            styleDic.Add("MINCED", new FormattedStringField(enviroStylePanel, "MINCED", "MINCED", Utils.FormattedStringFromFormattedText("MINCED"), true));
            styleDic.Add("SHREDDED", new FormattedStringField(enviroStylePanel, "SHREDDED", "SHREDDED", Utils.FormattedStringFromFormattedText("SHREDDED"), true));
            styleDic.Add("CRUSHED", new FormattedStringField(enviroStylePanel, "CRUSHED", "CRUSHED", Utils.FormattedStringFromFormattedText("CRUSHED"), true));
            styleDic.Add("FALL", new FormattedStringField(enviroStylePanel, "FALL", "FALL", Utils.FormattedStringFromFormattedText("FALL"), true));
            styleDic.Add("ZAPPED", new FormattedStringField(enviroStylePanel, "ZAPPED", "ZAPPED", Utils.FormattedStringFromFormattedText("ZAPPED"), true));
            styleDic.Add("TRAMPLED", new FormattedStringField(enviroStylePanel, "TRAMPLED", "TRAMPLED", Utils.FormattedStringFromFormattedText("TRAMPLED"), true));
            styleDic.Add("ROADKILL", new FormattedStringField(enviroStylePanel, "ROADKILL", "ROADKILL", Utils.FormattedStringFromFormattedText("ROADKILL"), true));
            styleDic.Add("FOR THEE", new FormattedStringField(enviroStylePanel, "FOR THEE", "FOR THEE", Utils.FormattedStringFromFormattedText("FOR THEE"), true));
            styleDic.Add("BOILED", new FormattedStringField(enviroStylePanel, "BOILED", "BOILED", Utils.FormattedStringFromFormattedText("BOILED"), true));
            styleDic.Add("PANCAKED", new FormattedStringField(enviroStylePanel, "PANCAKED", "PANCAKED", Utils.FormattedStringFromFormattedText("PANCAKED"), true));
            styleDic.Add("SLIPPED", new FormattedStringField(enviroStylePanel, "SLIPPED", "SLIPPED", Utils.FormattedStringFromFormattedText("SLIPPED"), true));
            styleDic.Add("LOST", new FormattedStringField(enviroStylePanel, "LOST", "LOST", Utils.FormattedStringFromFormattedText("LOST"), true));
            styleDic.Add("LONG WAY DOWN", new FormattedStringField(enviroStylePanel, "LONG WAY DOWN", "LONG WAY DOWN", Utils.FormattedStringFromFormattedText("LONG WAY DOWN"), true));
            styleDic.Add("M.A.D.", new FormattedStringField(enviroStylePanel, "M.A.D.", "M.A.D.", Utils.FormattedStringFromFormattedText("M.A.D."), true));
            styleDic.Add("TRASHED", new FormattedStringField(enviroStylePanel, "TRASHED", "TRASHED", Utils.FormattedStringFromFormattedText("TRASHED"), true));
            styleDic.Add("ENVIROKILL", new FormattedStringField(enviroStylePanel, "Envirokill", "ENVIROKILL", Utils.FormattedStringFromFormattedText("ENVIROKILL"), true));
            styleDic.Add("SCRONGLED", new FormattedStringField(enviroStylePanel, "SCRONGLED", "SCRONGLED", Utils.FormattedStringFromFormattedText("SCRONGLED"), true));
            styleDic.Add("SCRONGBONGLED", new FormattedStringField(enviroStylePanel, "SCRONGBONGLED", "SCRONGBONGLED", Utils.FormattedStringFromFormattedText("SCRONGBONGLED"), true));
            styleDic.Add("SCRINDONGULODED", new FormattedStringField(enviroStylePanel, "SCRINDONGULODED", "SCRINDONGULODED", Utils.FormattedStringFromFormattedText("SCRINDONGULODED"), true));
            styleDic.Add("why are you even spawning enemies here", new FormattedStringField(enviroStylePanel, "why are you even spawning enemies here", "why are you even spawning enemies here", Utils.FormattedStringFromFormattedText("why are you even spawning enemies here"), true));
            styleDic.Add("OUT OF BOUNDS", new FormattedStringField(enviroStylePanel, "OUT OF BOUNDS", "OUT OF BOUNDS", Utils.FormattedStringFromFormattedText("OUT OF BOUNDS"), true));

            // MISC STYLES
            miscPanelSearchbar = new StringField(miscStylePanel, "Search Bar", "miscPanelSearchbar", "", true, false);
            miscPanelSearchButton = new ButtonArrayField(miscStylePanel, "miscPanelSearchButton", 2, new float[] {0.5f, 0.5f}, new string[] {"Search", "Clear Filter"});
            MakeSearchBar(miscPanelSearchbar, miscPanelSearchButton, miscStylePanel);

            styleDic.Add("ultrakill.secret", new FormattedStringField(miscStylePanel, "<color=cyan>SECRET</color>", "ultrakill.secret", Utils.FormattedStringFromFormattedText("<color=cyan>SECRET</color>"), true));
            styleDic.Add("ultrakill.parry", new FormattedStringField(miscStylePanel, "<color=lime>PARRY</color>", "ultrakill.parry", Utils.FormattedStringFromFormattedText("<color=lime>PARRY</color>"), true));
            styleDic.Add("ultrakill.quickdraw", new FormattedStringField(miscStylePanel, "<color=cyan>QUICKDRAW</color>", "ultrakill.quickdraw", Utils.FormattedStringFromFormattedText("<color=cyan>QUICKDRAW</color>"), true));
            styleDic.Add("ultrakill.ultra", new FormattedStringField(miscStylePanel, "<color=orange>ULTRA</color>", "ultrakill.ultra", Utils.FormattedStringFromFormattedText("<color=orange>ULTRA</color>"), true));
            styleDic.Add("ultrakill.counter", new FormattedStringField(miscStylePanel, "<color=red>COUNTER</color>", "ultrakill.counter", Utils.FormattedStringFromFormattedText("<color=red>COUNTER</color>"), true));
            styleDic.Add("ultrakill.enraged", new FormattedStringField(miscStylePanel, "<color=red>ENRAGED</color>", "ultrakill.enraged", Utils.FormattedStringFromFormattedText("<color=red>ENRAGED</color>"), true));
            styleDic.Add("ultrakill.chargeback", new FormattedStringField(miscStylePanel, "CHARGEBACK", "ultrakill.chargeback", Utils.FormattedStringFromFormattedText("CHARGEBACK"), true));
            styleDic.Add("ultrakill.downtosize", new FormattedStringField(miscStylePanel, "<color=cyan>DOWN TO SIZE</color>", "ultrakill.downtosize", Utils.FormattedStringFromFormattedText("<color=cyan>DOWN TO SIZE</color>"), true));
            styleDic.Add("<color=white>PAWN CAPTURE</color>", new FormattedStringField(miscStylePanel, "PAWN CAPTURE", "<color=white>PAWN CAPTURE</color>", Utils.FormattedStringFromFormattedText("<color=white>PAWN CAPTURE</color>"), true));
            styleDic.Add("<color=green>KNIGHT CAPTURE</color>", new FormattedStringField(miscStylePanel, "<color=green>KNIGHT CAPTURE</color>", "<color=green>KNIGHT CAPTURE</color>", Utils.FormattedStringFromFormattedText("<color=green>KNIGHT CAPTURE</color>"), true));
            styleDic.Add("<color=green>BISHOP CAPTURE</color>", new FormattedStringField(miscStylePanel, "<color=green>BISHOP CAPTURE</color>", "<color=green>BISHOP CAPTURE</color>", Utils.FormattedStringFromFormattedText("<color=green>BISHOP CAPTURE</color>"), true));
            styleDic.Add("<color=orange>ROOK CAPTURE</color>", new FormattedStringField(miscStylePanel, "<color=green>ROOK CAPTURE</color>", "<color=green>ROOK CAPTURE</color>", Utils.FormattedStringFromFormattedText("<color=green>ROOK CAPTURE</color>"), true));
            styleDic.Add("<color=red>QUEEN CAPTURE</color>", new FormattedStringField(miscStylePanel, "<color=green>QUEEN CAPTURE</color>", "<color=green>QUEEN CAPTURE</color>", Utils.FormattedStringFromFormattedText("<color=green>QUEEN CAPTURE</color>"), true));
            styleDic.Add("<color=green>KNIGHT PROMOTION</color>", new FormattedStringField(miscStylePanel, "<color=green>KNIGHT PROMOTION</color>", "<color=green>KNIGHT PROMOTION</color>", Utils.FormattedStringFromFormattedText("<color=green>KNIGHT PROMOTION</color>"), true));
            styleDic.Add("<color=green>BISHOP PROMOTION</color>", new FormattedStringField(miscStylePanel, "<color=green>BISHOP PROMOTION</color>", "<color=green>BISHOP PROMOTION</color>", Utils.FormattedStringFromFormattedText("<color=green>BISHOP PROMOTION</color>"), true));
            styleDic.Add("<color=green>ROOK PROMOTION</color>", new FormattedStringField(miscStylePanel, "<color=green>ROOK PROMOTION</color>", "<color=green>ROOK PROMOTION</color>", Utils.FormattedStringFromFormattedText("<color=green>ROOK PROMOTION</color>"), true));
            styleDic.Add("<color=green>QUEEN PROMOTION</color>", new FormattedStringField(miscStylePanel, "<color=green>QUEEN PROMOTION</color>", "<color=green>QUEEN PROMOTION</color>", Utils.FormattedStringFromFormattedText("<color=green>QUEEN PROMOTION</color>"), true));
            styleDic.Add("<color=cyan>EN PASSANT</color>", new FormattedStringField(miscStylePanel, "<color=cyan>EN PASSANT</color>", "<color=cyan>EN PASSANT</color>", Utils.FormattedStringFromFormattedText("<color=cyan>EN PASSANT</color>"), true));
            styleDic.Add("<color=cyan>CASTLED</color>", new FormattedStringField(miscStylePanel, "<color=cyan>CASTLED</color>", "<color=cyan>CASTLED</color>", Utils.FormattedStringFromFormattedText("<color=cyan>CASTLED</color>"), true));
            styleDic.Add("<color=green>BONGCLOUD</color>", new FormattedStringField(miscStylePanel, "<color=green>BONGCLOUD</color>", "<color=green>BONGCLOUD</color>", Utils.FormattedStringFromFormattedText("<color=green>BONGCLOUD</color>"), true));
            styleDic.Add("<color=red>FOOLS MATE</color>", new FormattedStringField(miscStylePanel, "<color=red>FOOLS MATE</color>", "<color=red>FOOLS MATE</color>", Utils.FormattedStringFromFormattedText("<color=red>FOOLS MATE</color>"), true));
            styleDic.Add("<color=orange>WHITE WINS</color>", new FormattedStringField(miscStylePanel, "<color=orange>WHITE WINS</color>", "<color=orange>WHITE WINS</color>", Utils.FormattedStringFromFormattedText("<color=orange>WHITE WINS</color>"), true));
            styleDic.Add("<color=orange>BLACK WINS</color>", new FormattedStringField(miscStylePanel, "<color=orange>BLACK WINS</color>", "<color=orange>BLACK WINS</color>", Utils.FormattedStringFromFormattedText("<color=orange>BLACK WINS</color>"), true));
            styleDic.Add("<color=red>ULTRAVICTORY</color>", new FormattedStringField(miscStylePanel, "<color=red>ULTRAVICTORY</color>", "<color=red>ULTRAVICTORY</color>", Utils.FormattedStringFromFormattedText("<color=red>ULTRAVICTORY</color>"), true));

            // UNKNOWNS
            unknownPanelSearchButton = new ButtonField(unknownStylePanel, "Search For Unknown Styles", "unknownPanelSearchButton");
            unknownPanelSearchButton.onClick += () =>
            {
                List<KeyValuePair<string, string>> toProcess = new List<KeyValuePair<string, string>>();

                if (StyleHUD.Instance != null)
                {
                    foreach (KeyValuePair<string, string> pair in StyleHUD.Instance.idNameDict)
                    {
                        if (styleDic.ContainsKey(pair.Key) || pair.Key.StartsWith("customcorpse."))
                            continue;

                        toProcess.Add(pair);
                    }
                }

                foreach (var pair in toProcess)
                {
                    var field = new FormattedStringField(unknownStylePanel, pair.Key, pair.Key, Utils.FormattedStringFromFormattedText(pair.Value), true);
                    styleDic.Add(pair.Key, field);
                    AddValueChangeListener(pair.Key, field);
                }

                toProcess.Clear();

                foreach (GameObject rootObj in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    foreach (DeathZone deathZone in rootObj.GetComponentsInChildren<DeathZone>(true))
                    {
                        string id = deathZone.deathType;
                        if (string.IsNullOrEmpty(id))
                            continue;

						if (styleDic.ContainsKey(id) || id.StartsWith("customcorpse."))
							continue;

                        string styleText = id;
                        if (StyleHUD.Instance != null && StyleHUD.Instance.idNameDict.TryGetValue(id, out string formattedStyle))
                            styleText = formattedStyle;

						toProcess.Add(new KeyValuePair<string, string>(id, styleText));
					}
                }

                foreach (var pair in toProcess)
                {
                    var field = new FormattedStringField(unknownStylePanel, pair.Key, pair.Key, Utils.FormattedStringFromFormattedText(pair.Value), true);
                    styleDic.Add(pair.Key, field);
                    AddValueChangeListener(pair.Key, field);
                }
            };

            foreach (KeyValuePair<string, FormattedStringField> pair in styleDic)
            {
                AddValueChangeListener(pair.Key, pair.Value);
            }
        }
    }
}
