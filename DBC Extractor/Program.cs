using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DbcExtractor
{
    class Program
    {
        public static string prefix = "./dbc/";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Extract full DBC structure? (y/n)");
                Boolean AsLocalized = Console.ReadLine() == "y" ? false : true;

                Extract(AsLocalized, Locale.Default);

                if (!AsLocalized)
                {
                    string orig_prefix = prefix;

                    foreach (Locale loc in Constants.SupportedLocales)
                    {
                        if (loc == Locale.Default)
                            continue;

                        string dir = Path.Combine(orig_prefix, loc.ToString());
                        if (!Directory.Exists(dir))
                            continue;

                        Console.WriteLine("Directory {0} exists and may contain localized DBCs. Extract? (y/n)", dir);
                        if (Console.ReadLine().Trim() == "y")
                        {
                            prefix = dir;
                            Extract(true, loc);
                        }
                    }
                }

                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static void Extract(bool AsLocalized, Locale ExtractAs)
        {
            string filename = AsLocalized ? "Localization" : "Data";
            if (ExtractAs != Locale.Default)
                filename += "_" + ExtractAs.ToString();

            List<DBC> DBCs = new List<DBC>
            {
                //DBC.Open<AreaTable>                 ("AreaTable.dbc"),
                //DBC.Open<Achievement>               ("Achievement.dbc"),
                //DBC.Open<AchievementCategory>       ("Achievement_Category.dbc"),
                //DBC.Open<AchievementCriteria>       ("Achievement_Criteria.dbc"),
                //DBC.Open<CharTitles>                ("CharTitles.dbc"),
                //DBC.Open<ChrClasses>                ("ChrClasses.dbc"),
                //DBC.Open<ChrRaces>                  ("ChrRaces.dbc"),
                //DBC.Open<CreatureFamily>            ("CreatureFamily.dbc"),
                //DBC.Open<CreatureType>              ("CreatureType.dbc"),
                DBC.Open<CurrencyTypes>             ("CurrencyTypes.dbc"),
                //DBC.Open<Faction>                   ("Faction.dbc"),
                //DBC.Open<GlyphProperties>           ("GlyphProperties.dbc"),
                //DBC.Open<ItemClass>                 ("ItemClass.dbc"),
                //DBC.Open<ItemPetFood>               ("ItemPetFood.dbc"),
                //DBC.Open<ItemSet>                   ("ItemSet.dbc"),
                //DBC.Open<ItemSubClass>              ("ItemSubClass.dbc"),
                //DBC.Open<LFGDungeonGroup>           ("LFGDungeonGroup.dbc"),
                //DBC.Open<LFGDungeons>               ("LFGDungeons.dbc"),
                //DBC.Open<Map>                       ("Map.dbc"),
                //DBC.Open<QuestInfo>                 ("QuestInfo.dbc"),
                //DBC.Open<QuestSort>                 ("QuestSort.dbc"),
                //DBC.Open<Resistances>               ("Resistances.dbc"),
                //DBC.Open<SkillLine>                 ("SkillLine.dbc"),
                //DBC.Open<Spell>                     ("Spell.dbc"),
                //DBC.Open<SpellIcon>                 ("SpellIcon.dbc"),
                //DBC.Open<Talent>                    ("Talent.dbc"),
                //DBC.Open<TalentTab>                 ("TalentTab.dbc"),
                //DBC.Open<WMOAreaTable>              ("WMOAreaTable.dbc"),
            };

            Console.WriteLine("Extracting...");

            var Writer = new StreamWriter("DBC_" + filename + ".sql");
            foreach (DBC dbc in DBCs)
            {
                Writer.Write(dbc.ToSQL(AsLocalized));
                Console.WriteLine("DBC {0} Extracted.", Path.GetFileName(dbc.Filename));
            }
            Writer.Close();

            if (!AsLocalized)
            {
                var w = new StreamWriter("DBC_" + filename + ".cs");
                foreach (DBC dbc in DBCs)
                {
                    if (dbc.EntryType.GetCustomAttributes(typeof(AsEnumAttribute), true).Length != 0)
                        w.Write(dbc.ToEnum());
                    //Console.WriteLine("DBC {0} Extracted.", Path.GetFileName(dbc.Filename));
                }
                w.Close();
            }

            var w1 = new StreamWriter("DBC_" + filename + ".xml");
            foreach (DBC dbc in DBCs)
            {
                if (dbc.EntryType.GetCustomAttributes(typeof(AsEnumAttribute), true).Length != 0)
                    w1.Write(dbc.ToXML());
                //Console.WriteLine("DBC {0} Extracted.", Path.GetFileName(dbc.Filename));
            }
            w1.Close();
        }
    }
}
