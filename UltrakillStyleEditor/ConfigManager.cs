using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using PluginConfig.API.Functionals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace UltrakillStyleEditor
{
    public static class ConfigManager
    {
        public static PluginConfigurator config;
        public static Dictionary<string, FormattedStringField> styleDic = new Dictionary<string, FormattedStringField>();

        public static ConfigPanel killStylePanel;
        public static ConfigPanel hurtStylePanel;
        public static ConfigPanel miscStylePanel;

        public static ConfigPanel unknownStylePanel;

        public static StringField killPanelSearchbar;
        public static ButtonArrayField killPanelSearchButton;

        public static ButtonField unknownPanelSearchButton;

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
            miscStylePanel = new ConfigPanel(config.rootPanel, "Misc Styles", "miscStylePanel");

            new ConfigHeader(config.rootPanel, "Additional Styles");
            unknownStylePanel = new ConfigPanel(config.rootPanel, "Unknown Styles", "unknownStylePanel");

            //styleDic.Add("", new FormattedStringField(killStylePanel, "", "", Utils.FormattedStringFromFormattedText(""), true));

            // KILL RELATED STYLES
            killPanelSearchbar = new StringField(killStylePanel, "Search Bar", "killPanelSearchbar", "", true, false);
            killPanelSearchButton = new ButtonArrayField(killStylePanel, "killPanelSearchButton", 2, new float[] {0.5f, 0.5f}, new string[] {"Search", "Clear Filter"});
            MakeSearchBar(killPanelSearchbar, killPanelSearchButton, killStylePanel);
            styleDic.Add("ultrakill.kill", new FormattedStringField(killStylePanel, "Kill", "ultrakill.kill", Utils.FormattedStringFromFormattedText("KILL"), true));
            styleDic.Add("ultrakill.doublekill", new FormattedStringField(killStylePanel, "Double kill", "ultrakill.doublekill", Utils.FormattedStringFromFormattedText("<color=orange>DOUBLE KILL</color>"), true));
            styleDic.Add("ultrakill.triplekill", new FormattedStringField(killStylePanel, "Triple kill", "ultrakill.triplekill", Utils.FormattedStringFromFormattedText("<color=orange>TRIPLE KILL</color>"), true));
            styleDic.Add("ultrakill.multikill", new FormattedStringField(killStylePanel, "Multikill", "ultrakill.multikill", Utils.FormattedStringFromFormattedText("<color=orange>MULTIKILL</color>"), true));
            styleDic.Add("ultrakill.instakill", new FormattedStringField(killStylePanel, "Instakill", "ultrakill.instakill", Utils.FormattedStringFromFormattedText("<color=lime>INSTAKILL</color>"), true));
            styleDic.Add("ultrakill.interruption", new FormattedStringField(killStylePanel, "Interruption", "ultrakill.interruption", Utils.FormattedStringFromFormattedText("<color=lime>INTERRUPTION</color>"), true));
            styleDic.Add("ultrakill.bigkill", new FormattedStringField(killStylePanel, "Big kill", "ultrakill.bigkill", Utils.FormattedStringFromFormattedText("BIG KILL"), true));
            styleDic.Add("ultrakill.bigfistkill", new FormattedStringField(killStylePanel, "Big fist kill", "ultrakill.bigfistkill", Utils.FormattedStringFromFormattedText("BIG FISTKILL"), true));
            styleDic.Add("ultrakill.criticalpunch", new FormattedStringField(killStylePanel, "Critical punch", "ultrakill.criticalpunch", Utils.FormattedStringFromFormattedText("CRITICAL PUNCH"), true));
            styleDic.Add("ultrakill.arsenal", new FormattedStringField(killStylePanel, "Arsenal", "ultrakill.arsenal", Utils.FormattedStringFromFormattedText("<color=cyan>ARSENAL</color>"), true));
            styleDic.Add("ultrakill.splattered", new FormattedStringField(killStylePanel, "Splattered", "ultrakill.splattered", Utils.FormattedStringFromFormattedText("SPLATTERED"), true));
            styleDic.Add("ultrakill.groundslam", new FormattedStringField(killStylePanel, "Ground slam", "ultrakill.groundslam", Utils.FormattedStringFromFormattedText("GROUND SLAM"), true));
            styleDic.Add("ultrakill.airslam", new FormattedStringField(killStylePanel, "Air slam", "ultrakill.airslam", Utils.FormattedStringFromFormattedText("<color=cyan>AIR SLAM</color>"), true));
            styleDic.Add("ultrakill.airshot", new FormattedStringField(killStylePanel, "Airshot", "ultrakill.airshot", Utils.FormattedStringFromFormattedText("<color=cyan>AIRSHOT</color>"), true));
            styleDic.Add("ultrakill.fireworks", new FormattedStringField(killStylePanel, "Fireworks", "ultrakill.fireworks", Utils.FormattedStringFromFormattedText("<color=cyan>FIREWORKS</color>"), true));
            styleDic.Add("ultrakill.friendlyfire", new FormattedStringField(killStylePanel, "Friendly fire", "ultrakill.friendlyfire", Utils.FormattedStringFromFormattedText("FRIENDLY FIRE"), true));
            styleDic.Add("ultrakill.exploded", new FormattedStringField(killStylePanel, "Exploded", "ultrakill.exploded", Utils.FormattedStringFromFormattedText("EXPLODED"), true));
            styleDic.Add("ultrakill.fried", new FormattedStringField(killStylePanel, "Fried", "ultrakill.fried", Utils.FormattedStringFromFormattedText("FRIED"), true));
            styleDic.Add("ultrakill.finishedoff", new FormattedStringField(killStylePanel, "Finished off", "ultrakill.finishedoff", Utils.FormattedStringFromFormattedText("<color=cyan>FINISHED OFF</color>"), true));
            styleDic.Add("ultrakill.mauriced", new FormattedStringField(killStylePanel, "Mauriced", "ultrakill.mauriced", Utils.FormattedStringFromFormattedText("MAURICED"), true));
            styleDic.Add("ultrakill.overkill", new FormattedStringField(killStylePanel, "Overkill", "ultrakill.overkill", Utils.FormattedStringFromFormattedText("OVERKILL"), true));
            styleDic.Add("ultrakill.halfoff", new FormattedStringField(killStylePanel, "Half off", "ultrakill.halfoff", Utils.FormattedStringFromFormattedText("<color=cyan>HALF OFF</color>"), true));
            styleDic.Add("ultrakill.bipolar", new FormattedStringField(killStylePanel, "Bipolar", "ultrakill.bipolar", Utils.FormattedStringFromFormattedText("BIPOLAR"), true));
            styleDic.Add("ultrakill.attripator", new FormattedStringField(killStylePanel, "Attraptor", "ultrakill.attripator", Utils.FormattedStringFromFormattedText("<color=cyan>ATTRAPTOR</color>"), true));
            styleDic.Add("ultrakill.nailbombed", new FormattedStringField(killStylePanel, "Nailbombed (death)", "ultrakill.nailbombed", Utils.FormattedStringFromFormattedText("NAILBOMBED"), true));
            styleDic.Add("ultrakill.catapulted", new FormattedStringField(killStylePanel, "Catapulted", "ultrakill.catapulted", Utils.FormattedStringFromFormattedText("<color=cyan>CATAPULTED</color>"), true));
            styleDic.Add("ultrakill.cannonballed", new FormattedStringField(killStylePanel, "Cannonballed", "ultrakill.cannonballed", Utils.FormattedStringFromFormattedText("CANNONBALLED"), true));
            styleDic.Add("ultrakill.compressed", new FormattedStringField(killStylePanel, "Compressed (blackhole kill)", "ultrakill.compressed", Utils.FormattedStringFromFormattedText("COMPRESSED"), true));
            styleDic.Add("SHREDDED", new FormattedStringField(killStylePanel, "Shredded", "SHREDDED", Utils.FormattedStringFromFormattedText("SHREDDED"), true));
            styleDic.Add("MINCED", new FormattedStringField(killStylePanel, "Minced", "MINCED", Utils.FormattedStringFromFormattedText("MINCED"), true));
            styleDic.Add("CRUSHED", new FormattedStringField(killStylePanel, "Crushed", "CRUSHED", Utils.FormattedStringFromFormattedText("CRUSHED"), true));
            styleDic.Add("FALL", new FormattedStringField(killStylePanel, "Fall", "FALL", Utils.FormattedStringFromFormattedText("FALL"), true));
            styleDic.Add("ZAPPED", new FormattedStringField(killStylePanel, "Zapped", "ZAPPED", Utils.FormattedStringFromFormattedText("ZAPPED"), true));
            styleDic.Add("OUT OF BOUNDS", new FormattedStringField(killStylePanel, "Out of bounds", "OUT OF BOUNDS", Utils.FormattedStringFromFormattedText("OUT OF BOUNDS"), true));
            styleDic.Add("why are you even spawning enemies here", new FormattedStringField(killStylePanel, "why are you even spawning enemies here", "why are you even spawning enemies here", Utils.FormattedStringFromFormattedText("why are you even spawning enemies here"), true));
            styleDic.Add("SCRINDONGULODED", new FormattedStringField(killStylePanel, "Scrindonguloded", "SCRINDONGULODED", Utils.FormattedStringFromFormattedText("SCRINDONGULODED"), true));
            styleDic.Add("SCRONGBONGLED", new FormattedStringField(killStylePanel, "Scrongbongled", "SCRONGBONGLED", Utils.FormattedStringFromFormattedText("SCRONGBONGLED"), true));
            styleDic.Add("SCRONGLED", new FormattedStringField(killStylePanel, "Scrongled", "SCRONGLED", Utils.FormattedStringFromFormattedText("SCRONGLED"), true));
            styleDic.Add("ENVIROKILL", new FormattedStringField(killStylePanel, "Envirokill", "ENVIROKILL", Utils.FormattedStringFromFormattedText("ENVIROKILL"), true));

            //why_are_you_even_spawning_enemies_here

            // HURT RELATED STYLES
            styleDic.Add("ultrakill.headshot", new FormattedStringField(hurtStylePanel, "Headshot", "ultrakill.headshot", Utils.FormattedStringFromFormattedText("HEADSHOT"), true));
            styleDic.Add("ultrakill.bigheadshot", new FormattedStringField(hurtStylePanel, "Big headshot", "ultrakill.bigheadshot", Utils.FormattedStringFromFormattedText("BIG HEADSHOT"), true));
            styleDic.Add("ultrakill.headshotcombo", new FormattedStringField(hurtStylePanel, "Headshot combo", "ultrakill.headshotcombo", Utils.FormattedStringFromFormattedText("<color=cyan>HEADSHOT COMBO</color>"), true));
            styleDic.Add("ultrakill.ricoshot", new FormattedStringField(hurtStylePanel, "Ricoshot", "ultrakill.ricoshot", Utils.FormattedStringFromFormattedText("<color=cyan>RICOSHOT</color>"), true));
            styleDic.Add("ultrakill.conductor", new FormattedStringField(hurtStylePanel, "Conductor", "ultrakill.conductor", Utils.FormattedStringFromFormattedText("<color=cyan>CONDUCTOR</color>"), true));
            styleDic.Add("ultrakill.limbhit", new FormattedStringField(hurtStylePanel, "Limb hit", "ultrakill.limbhit", Utils.FormattedStringFromFormattedText("LIMB HIT"), true));
            styleDic.Add("ultrakill.fistfullofdollar", new FormattedStringField(hurtStylePanel, "Fistfull of dollar", "ultrakill.fistfullofdollar", Utils.FormattedStringFromFormattedText("<color=cyan>FISTFUL OF DOLLAR</color>"), true));
            styleDic.Add("ultrakill.nailbombedalive", new FormattedStringField(hurtStylePanel, "Nailbombed (hurt)", "ultrakill.nailbombedalive", Utils.FormattedStringFromFormattedText("<color=grey>NAILBOMBED</color>"), true));
            styleDic.Add("ultrakill.homerun", new FormattedStringField(hurtStylePanel, "Homerun", "ultrakill.homerun", Utils.FormattedStringFromFormattedText("HOMERUN"), true));
            styleDic.Add("ultrakill.projectileboost", new FormattedStringField(hurtStylePanel, "Projectile boost", "ultrakill.projectileboost", Utils.FormattedStringFromFormattedText("<color=lime>PROJECTILE BOOST</color>"), true));
            styleDic.Add("ultrakill.disrespect", new FormattedStringField(hurtStylePanel, "Disrespect", "ultrakill.disrespect", Utils.FormattedStringFromFormattedText("DISRESPECT"), true));
            styleDic.Add("ultrakill.shotgunhit", new FormattedStringField(hurtStylePanel, "Shotgun hit", "ultrakill.shotgunhit", Utils.FormattedStringFromFormattedText(""), true));
            styleDic.Add("ultrakill.nailhit", new FormattedStringField(hurtStylePanel, "Nail hit", "ultrakill.nailhit", Utils.FormattedStringFromFormattedText(""), true));
            styleDic.Add("ultrakill.explosionhit", new FormattedStringField(hurtStylePanel, "Explosion hit", "ultrakill.explosionhit", Utils.FormattedStringFromFormattedText(""), true));
            styleDic.Add("ultrakill.firehit", new FormattedStringField(hurtStylePanel, "Fire hit", "ultrakill.firehit", Utils.FormattedStringFromFormattedText(""), true));
            
            // MISC STYLES
            styleDic.Add("ultrakill.secret", new FormattedStringField(miscStylePanel, "Secret", "ultrakill.secret", Utils.FormattedStringFromFormattedText("<color=cyan>SECRET</color>"), true));
            styleDic.Add("ultrakill.quickdraw", new FormattedStringField(miscStylePanel, "Quickdraw", "ultrakill.quickdraw", Utils.FormattedStringFromFormattedText("<color=cyan>QUICKDRAW</color>"), true));
            styleDic.Add("ultrakill.parry", new FormattedStringField(miscStylePanel, "Parry", "ultrakill.parry", Utils.FormattedStringFromFormattedText("<color=lime>PARRY</color>"), true));
            styleDic.Add("ultrakill.enraged", new FormattedStringField(miscStylePanel, "Enraged", "ultrakill.enraged", Utils.FormattedStringFromFormattedText("<color=red>ENRAGED</color>"), true));
            styleDic.Add("ultrakill.downtosize", new FormattedStringField(miscStylePanel, "Down to size", "ultrakill.downtosize", Utils.FormattedStringFromFormattedText("<color=cyan>DOWN TO SIZE</color>"), true));
            styleDic.Add("ultrakill.chargeback", new FormattedStringField(miscStylePanel, "Chargeback", "ultrakill.chargeback", Utils.FormattedStringFromFormattedText("CHARGEBACK"), true));
            styleDic.Add("ultrakill.ultra", new FormattedStringField(miscStylePanel, "Ultra prefix", "ultrakill.ultra", Utils.FormattedStringFromFormattedText("<color=orange>ULTRA</color>"), true));
            styleDic.Add("ultrakill.counter", new FormattedStringField(miscStylePanel, "Counter prefix", "ultrakill.counter", Utils.FormattedStringFromFormattedText("<color=red>COUNTER</color>"), true));

            // UNKNOWNS
            unknownPanelSearchButton = new ButtonField(unknownStylePanel, "Search For Unknown Styles", "unknownPanelSearchButton");
            unknownPanelSearchButton.onClick += () =>
            {
                if (StyleHUD.instance == null)
                    return;

                foreach (KeyValuePair<string, string> pair in StyleHUD.instance.idNameDict)
                {
                    if (styleDic.ContainsKey(pair.Key))
                        continue;

                    var field = new FormattedStringField(unknownStylePanel, pair.Key, pair.Key, Utils.FormattedStringFromFormattedText(pair.Value));
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
