using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace opcodemerge
{
    class Program
    {
        static Tuple<string, uint> GetPair(List<Tuple<string, uint>> dict, uint value)
        {
            foreach (var pair in dict)
            {
                if (pair.Item2 == value)
                    return pair;
            }

            return null;
        }

        static Tuple<string, uint> GetPair(List<Tuple<string, uint>> dict, string key)
        {
            foreach (var pair in dict)
            {
                if (pair.Item1 == key)
                    return pair;
            }

            return null;
        }

        static void usage()
        {
            Console.WriteLine("Usage: opcodemerge 'our file' 'their file' [flags]");
            Console.WriteLine("    flags:");
            Console.WriteLine("       -D             ignore our opcodes with wrong build");
            Console.WriteLine("       -d             ignore their opcodes with wrong build");
            Console.WriteLine("       -b [build]     use this build (required for d,D)");
            Console.WriteLine("       -o [file]      log file");
            Console.WriteLine("       -h             insert hex opcode values");
            Console.WriteLine("       -n             fix name conflicts");
            Console.WriteLine("Example: opcodemerge opcodes.cpp opcodes_their.cpp -b 12340 -d");
        }

        static Dictionary<string, string> s_opcodeAnalogues = new Dictionary<string, string>
        {
            { "MSG_MOVE_SET_RAW_POSITION_ACK", "CMSG_MOVE_CHARM_PORT_CHEAT" },
            { "OBSOLETE_DROP_ITEM", "CMSG_UNCLAIM_LICENSE" },
            { "SMSG_INSPECT", "SMSG_INSPECT_RESULTS_UPDATE" },
            { "SMSG_EQUIPMENT_SET_SAVED", "SMSG_EQUIPMENT_SET_ID" },
            { "CMSG_EQUIPMENT_SET_DELETE", "CMSG_DELETEEQUIPMENT_SET" },
            { "CMSG_INSTANCE_LOCK_WARNING_RESPONSE", "CMSG_INSTANCE_LOCK_RESPONSE" },
            { "CMSG_UNUSED2", "CMSG_DEBUG_PASSIVE_AURA" },
            { "SMSG_INSTANCE_LOCK_WARNING_QUERY", "SMSG_PENDING_RAID_LOCK" },
            { "SMSG_DAMAGE_DONE_OBSOLETE", "CMSG_PERFORM_ACTION_SET" },
            { "SMSG_UNIT_SPELLCAST_START", "SMSG_RESUME_CAST_BAR" },
            { "SMSG_REMOVE_FROM_PVP_QUEUE_RESULT", "SMSG_REMOVED_FROM_PVP_QUEUE" },
            { "SMSG_QUESTUPDATE_ADD_ITEM", "SMSG_QUESTUPDATE_ADD_ITEM_OBSOLETE" },
            { "CMSG_PLAYER_DIFFICULTY_CHANGE", "CMSG_CHANGEPLAYER_DIFFICULTY" },
            { "SMSG_PLAYER_DIFFICULTY_CHANGE", "SMSG_CHANGEPLAYER_DIFFICULTY_RESULT" },
            { "SMSG_GAMEOBJECT_SPAWN_ANIM_OBSOLETE", "SMSG_INSTANCE_ENCOUNTER" },
            { "SMSG_QUEST_FORCE_REMOVE", "SMSG_QUEST_FORCE_REMOVED" },
            { "SMSG_BATTLEFIELD_WIN_OBSOLETE", "SMSG_FORCE_SET_VEHICLE_REC_ID" },
            { "SMSG_BATTLEFIELD_LOSE_OBSOLETE", "CMSG_SET_VEHICLE_REC_ID_ACK" },
            { "SMSG_STANDSTATE_CHANGE_FAILURE_OBSOLETE", "SMSG_COMBAT_EVENT_FAILED" },
            { "CMSG_MEETINGSTONE_CHEAT", "CMSG_TEST_DROP_RATE" },
            { "SMSG_MEETINGSTONE_SETQUEUE", "SMSG_TEST_DROP_RATE_RESULT" },
            { "CMSG_MEETINGSTONE_INFO", "CMSG_LFG_GET_STATUS" },
            { "SMSG_MAIL_SHOW_FROM_GOSSIP", "SMSG_SHOW_MAILBOX" },
            { "SMSG_MEETINGSTONE_IN_PROGRESS", "SMSG_RESET_RANGED_COMBAT_TIMER" },
            { "SMSG_MEETINGSTONE_MEMBER_ADDED", "SMSG_CHAT_NOT_IN_PARTY" },
            { "MSG_MOVE_TOGGLE_GRAVITY_CHEAT", "MSG_DEV_SHOWLABEL" },
            { "SMSG_LFG_ROLE_CHOSEN", "SMSG_ROLE_CHOSEN" },
            { "SMSG_MOVE_SET_FLIGHT", "SMSG_MOVE_SET_CAN_TRANSITION_BETWEEN_SWIM_AND_FLY" },
            { "SMSG_MOVE_UNSET_FLIGHT", "SMSG_MOVE_UNSET_CAN_TRANSITION_BETWEEN_SWIM_AND_FLY" },
            { "CMSG_MOVE_FLIGHT_ACK", "CMSG_MOVE_SET_CAN_TRANSITION_BETWEEN_SWIM_AND_FLY_ACK" },
            { "UMSG_UPDATE_ARENA_TEAM_OBSOLETE", "MSG_MOVE_UPDATE_CAN_TRANSITION_BETWEEN_SWIM_AND_FLY" },
            { "CMSG_SEARCH_LFG_JOIN", "CMSG_LFG_SEARCH_JOIN" },
            { "CMSG_SEARCH_LFG_LEAVE", "CMSG_LFG_SEARCH_LEAVE" },
            { "SMSG_UPDATE_LFG_LIST", "SMSG_LFG_SEARCH_RESULTS" },
            { "CMSG_LFG_PROPOSAL_RESULT", "CMSG_LFG_PROPOSAL_RESPONSE" },
            { "CMSG_LFG_SET_BOOT_VOTE", "CMSG_LFG_BOOT_PLAYER_VOTE" },
            { "CMSG_LFD_PLAYER_LOCK_INFO_REQUEST", "CMSG_LFG_GET_PLAYER_INFO" },
            { "CMSG_LFD_PARTY_LOCK_INFO_REQUEST", "CMSG_LFG_GET_PARTY_INFO" },
            { "SMSG_INSPECT_TALENT", "SMSG_INSPECT_RESULTS" },
            { "SMSG_MOVE_ABANDON_TRANSPORT", "CMSG_CALENDAR_EVENT_INVITE_NOTES" },
            { "SMSG_CALENDAR_UPDATE_INVITE_LIST", "SMSG_CALENDAR_EVENT_INVITE_NOTES" },
            { "SMSG_CALENDAR_UPDATE_INVITE_LIST2", "SMSG_CALENDAR_EVENT_INVITE_NOTES_ALERT" },
            { "SMSG_CALENDAR_UPDATE_INVITE_LIST3", "SMSG_CALENDAR_RAID_LOCKOUT_UPDATED" },
            //{ "CMSG_QUERY_VEHICLE_STATUS", "CMSG_SET_BREATH" },
            { "UMSG_UNKNOWN_1189", "CMSG_QUERY_VEHICLE_STATUS" },
            { "SMSG_PLAYER_VEHICLE_DATA", "SMSG_SET_VEHICLE_REC_ID" },
            { "CMSG_PLAYER_VEHICLE_ENTER", "CMSG_RIDE_VEHICLE_INTERACT" },
            { "CMSG_EJECT_PASSENGER", "CMSG_CONTROLLER_EJECT_PASSENGER" },
            { "UMSG_UNKNOWN_1196", "CMSG_CHANGE_GDF_ARENA_RATING" },
            { "UMSG_UNKNOWN_1197", "CMSG_SET_ARENA_TEAM_RATING_BY_INDEX" },
            { "UMSG_UNKNOWN_1198", "CMSG_SET_ARENA_TEAM_WEEKLY_GAMES" },
            { "UMSG_UNKNOWN_1199", "CMSG_SET_ARENA_TEAM_SEASON_GAMES" },
            { "UMSG_UNKNOWN_1200", "CMSG_SET_ARENA_MEMBER_WEEKLY_GAMES" },
            { "UMSG_UNKNOWN_1201", "CMSG_SET_ARENA_MEMBER_SEASON_GAMES" },
            { "SMSG_ITEM_REFUND_INFO_RESPONSE", "SMSG_SET_ITEM_PURCHASE_DATA" },
            { "CMSG_ITEM_REFUND_INFO", "CMSG_GET_ITEM_PURCHASE_DATA" },
            { "CMSG_ITEM_REFUND", "CMSG_ITEM_PURCHASE_REFUND" },
            { "SMSG_ITEM_REFUND_RESULT", "SMSG_ITEM_PURCHASE_REFUND_RESULT" },
            { "CMSG_CORPSE_MAP_POSITION_QUERY", "CMSG_CORPSE_TRANSPORT_QUERY" },
            { "SMSG_CORPSE_MAP_POSITION_QUERY_RESPONSE", "SMSG_CORPSE_TRANSPORT_QUERY" },
            { "UMSG_UNKNOWN_1208", "CMSG_UNUSED5" },
            { "UMSG_UNKNOWN_1209", "CMSG_UNUSED6" },
            { "CMSG_CALENDAR_CONTEXT_EVENT_SIGNUP", "CMSG_CALENDAR_EVENT_SIGNUP" },
            { "SMSG_CALENDAR_ACTION_PENDING", "SMSG_CALENDAR_CLEAR_PENDING_ACTION" },
            { "SMSG_EQUIPMENT_SET_LIST", "SMSG_LOAD_EQUIPMENT_SET" },
            { "CMSG_EQUIPMENT_SET_SAVE", "CMSG_SAVE_EQUIPMENT_SET" },
            { "CMSG_UPDATE_PROJECTILE_POSITION", "CMSG_ON_MISSILE_TRAJECTORY_COLLISION" },
            { "SMSG_SET_PROJECTILE_POSITION", "SMSG_NOTIFY_MISSILE_TRAJECTORY_COLLISION" },
            { "SMSG_TALENTS_INFO", "SMSG_TALENT_UPDATE" },
            { "CMSG_LEARN_PREVIEW_TALENTS", "CMSG_LEARN_TALENT_GROUP" },
            { "CMSG_LEARN_PREVIEW_TALENTS_PET", "CMSG_PET_LEARN_TALENT_GROUP" },
            { "UMSG_UNKNOWN_1219", "CMSG_SET_ACTIVE_TALENT_GROUP_OBSOLETE" },
            { "UMSG_UNKNOWN_1220", "CMSG_GM_GRANT_ACHIEVEMENT" },
            { "UMSG_UNKNOWN_1221", "CMSG_GM_REMOVE_ACHIEVEMENT" },
            { "UMSG_UNKNOWN_1222", "CMSG_GM_SET_CRITERIA_FOR_PLAYER" },
            { "SMSG_ARENA_OPPONENT_UPDATE", "SMSG_DESTROY_ARENA_UNIT" },
            { "SMSG_ARENA_TEAM_CHANGE_FAILED_QUEUED", "SMSG_ARENA_TEAM_CHANGE_FAILED" },
            { "UMSG_UNKNOWN_1225", "CMSG_PROFILEDATA_REQUEST" },
            { "UMSG_UNKNOWN_1226", "SMSG_PROFILEDATA_RESPONSE" },
            { "UMSG_UNKNOWN_1227", "CMSG_START_BATTLEFIELD_CHEAT" },
            { "UMSG_UNKNOWN_1228", "CMSG_END_BATTLEFIELD_CHEAT" },
            { "SMSG_MULTIPLE_PACKETS", "SMSG_COMPOUND_MOVE" },
            { "SMSG_MOVE_SET_LEVITATING", "SMSG_MOVE_GRAVITY_DISABLE" },
            { "CMSG_MOVE_SET_LEVITATING_ACK", "CMSG_MOVE_GRAVITY_DISABLE_ACK" },
            { "SMSG_MOVE_UNSET_LEVITATING", "SMSG_MOVE_GRAVITY_ENABLE" },
            { "CMSG_MOVE_UNSET_LEVITATING_ACK", "CMSG_MOVE_GRAVITY_ENABLE_ACK" },
            { "SMSG_MOVE_LEVITATING_", "MSG_MOVE_GRAVITY_CHNG" },
            { "SMSG_SPLINE_MOVE_SET_LEVITATING", "SMSG_SPLINE_MOVE_GRAVITY_DISABLE" },
            { "SMSG_SPLINE_MOVE_UNSET_LEVITATING", "SMSG_SPLINE_MOVE_GRAVITY_ENABLE" },
            { "CMSG_EQUIPMENT_SET_USE", "CMSG_USE_EQUIPMENT_SET" },
            { "SMSG_EQUIPMENT_SET_USE_RESULT", "SMSG_USE_EQUIPMENT_SET_RESULT" },
            { "UMSG_UNKNOWN_1239", "CMSG_FORCE_ANIM" },
            { "SMSG_UNKNOWN_1240", "SMSG_FORCE_ANIM" },
            { "UMSG_UNKNOWN_1243", "CMSG_PVP_QUEUE_STATS_REQUEST" },
            { "UMSG_UNKNOWN_1244", "SMSG_PVP_QUEUE_STATS" },
            { "UMSG_UNKNOWN_1245", "CMSG_SET_PAID_SERVICE_CHEAT" },
            { "SMSG_BATTLEFIELD_MGR_ENTRY_INVITE", "SMSG_BATTLEFIELD_MANAGER_ENTRY_INVITE" },
            { "CMSG_BATTLEFIELD_MGR_ENTRY_INVITE_RESPONSE", "CMSG_BATTLEFIELD_MANAGER_ENTRY_INVITE_RESPONSE" },
            { "SMSG_BATTLEFIELD_MGR_ENTERED", "SMSG_BATTLEFIELD_MANAGER_ENTERING" },
            { "SMSG_BATTLEFIELD_MGR_QUEUE_INVITE", "SMSG_BATTLEFIELD_MANAGER_QUEUE_INVITE" },
            { "CMSG_BATTLEFIELD_MGR_QUEUE_INVITE_RESPONSE", "CMSG_BATTLEFIELD_MANAGER_QUEUE_INVITE_RESPONSE" },
            { "CMSG_BATTLEFIELD_MGR_QUEUE_REQUEST", "CMSG_BATTLEFIELD_MANAGER_QUEUE_REQUEST" },
            { "SMSG_BATTLEFIELD_MGR_QUEUE_REQUEST_RESPONSE", "SMSG_BATTLEFIELD_MANAGER_QUEUE_REQUEST_RESPONSE" },
            { "SMSG_BATTLEFIELD_MGR_EJECT_PENDING", "SMSG_BATTLEFIELD_MANAGER_EJECT_PENDING" },
            { "SMSG_BATTLEFIELD_MGR_EJECTED", "SMSG_BATTLEFIELD_MANAGER_EJECTED" },
            { "CMSG_BATTLEFIELD_MGR_EXIT_REQUEST", "CMSG_BATTLEFIELD_MANAGER_EXIT_REQUEST" },
            { "SMSG_BATTLEFIELD_MGR_STATE_CHANGE", "SMSG_BATTLEFIELD_MANAGER_STATE_CHANGED" },
            { "UMSG_UNKNOWN_1257", "CMSG_BATTLEFIELD_MANAGER_ADVANCE_STATE" },
            { "UMSG_UNKNOWN_1258", "CMSG_BATTLEFIELD_MANAGER_SET_NEXT_TRANSITION_TIME" },
            { "UMSG_UNKNOWN_1260", "CMSG_XPGAIN" },
            { "SMSG_TOGGLE_XP_GAIN", "SMSG_XPGAIN" },
            { "SMSG_GMRESPONSE_DB_ERROR", "SMSG_GMTICKET_RESPONSE_ERROR" },
            { "SMSG_GMRESPONSE_RECEIVED", "SMSG_GMTICKET_GET_RESPONSE" },
            { "CMSG_GMRESPONSE_RESOLVE", "CMSG_GMTICKET_RESOLVE_RESPONSE" },
            { "SMSG_GMRESPONSE_STATUS_UPDATE", "SMSG_GMTICKET_RESOLVE_RESPONSE" },
            { "UMSG_UNKNOWN_1266", "SMSG_GMTICKET_CREATE_RESPONSE_TICKET" },
            { "UMSG_UNKNOWN_1267", "CMSG_GM_CREATE_TICKET_RESPONSE" },
            { "UMSG_UNKNOWN_1268", "CMSG_SERVERINFO" },
            { "UMSG_UNKNOWN_1269", "SMSG_SERVERINFO" },
            { "CMSG_WORLD_STATE_UI_TIMER_UPDATE", "CMSG_UI_TIME_REQUEST" },
            { "SMSG_WORLD_STATE_UI_TIMER_UPDATE", "SMSG_UI_TIME" },
            { "UMSG_UNKNOWN_1273", "MSG_VIEW_PHASE_SHIFT" },
            { "UMSG_UNKNOWN_1275", "CMSG_DEBUG_SERVER_GEO" },
            { "SMSG_UNKNOWN_1276", "SMSG_DEBUG_SERVER_GEO" },
            { "SMSG_LOOT_SLOT_CHANGED", "SMSG_LOOT_UPDATE" },
            { "UMSG_UNKNOWN_1278", "UMSG_UPDATE_GROUP_INFO" },
            { "CMSG_QUERY_QUESTS_COMPLETED", "CMSG_QUERY_GET_ALL_QUESTS" },
            { "SMSG_QUERY_QUESTS_COMPLETED_RESPONSE", "SMSG_ALL_QUESTS_COMPLETED" },
            { "CMSG_GM_REPORT_LAG", "CMSG_GMLAGREPORT_SUBMIT" },
            { "UMSG_UNKNOWN_1283", "CMSG_AFK_MONITOR_INFO_REQUEST" },
            { "UMSG_UNKNOWN_1284", "SMSG_AFK_MONITOR_INFO_RESPONSE" },
            { "UMSG_UNKNOWN_1285", "CMSG_AFK_MONITOR_INFO_CLEAR" },
            { "SMSG_CORPSE_IS_NOT_IN_INSTANCE", "SMSG_AREA_TRIGGER_NO_CORPSE" },
            { "UMSG_UNKNOWN_1287", "CMSG_GM_NUKE_CHARACTER" },
            { "CMSG_SET_ALLOW_LOW_LEVEL_RAID1", "CMSG_LOW_LEVEL_RAID" },
            { "CMSG_SET_ALLOW_LOW_LEVEL_RAID2", "CMSG_LOW_LEVEL_RAID_USER" },
            { "SMSG_UPDATE_ITEM_ENCHANTMENTS", "SMSG_SOCKET_GEMS" },
            { "UMSG_UNKNOWN_1292", "CMSG_SET_CHARACTER_MODEL" },
            { "SMSG_REDIRECT_CLIENT", "SMSG_CONNECT_TO" },
            { "CMSG_REDIRECTION_FAILED", "CMSG_CONNECT_TO_FAILED" },
            { "SMSG_UNKNOWN_1295", "SMSG_SUSPEND_COMMS" },
            { "CMSG_UNKNOWN_1296", "CMSG_SUSPEND_COMMS_ACK" },
            { "SMSG_FORCE_SEND_QUEUED_PACKETS", "SMSG_RESUME_COMMS" },
            { "CMSG_REDIRECTION_AUTH_PROOF", "CMSG_AUTH_CONTINUED_SESSION" },
            { "UMSG_UNKNOWN_1299", "CMSG_DROP_NEW_CONNECTION" },
            { "SMSG_COMBAT_LOG_MULTIPLE", "SMSG_SEND_ALL_COMBAT_LOG" },
            { "SMSG_LFG_OPEN_FROM_GOSSIP", "SMSG_OPEN_LFG_DUNGEON_FINDER" },
            { "SMSG_UNKNOWN_1302", "SMSG_MOVE_SET_COLLISION_HGT" },
            { "CMSG_UNKNOWN_1303", "CMSG_MOVE_SET_COLLISION_HGT_ACK" },
            { "SMSG_UNKNOWN_1304", "MSG_MOVE_SET_COLLISION_HGT" },
            { "UMSG_UNKNOWN_1305", "CMSG_CLEAR_RANDOM_BG_WIN_TIME" },
            { "UMSG_UNKNOWN_1306", "CMSG_CLEAR_HOLIDAY_BG_WIN_TIME" },
            { "SMSG_UNKNOWN_1308", "SMSG_COMMENTATOR_SKIRMISH_QUEUE_RESULT1" },
            { "SMSG_UNKNOWN_1309", "SMSG_COMMENTATOR_SKIRMISH_QUEUE_RESULT2" },
            { "SMSG_UNKNOWN_1310", "SMSG_COMPRESSED_UNKNOWN_1310" },
            { "CMSG_RETURN_TO_GRAVEYARD", "CMSG_PORT_GRAVEYARD" },
            { "SMSG_CORPSE_NOT_IN_INSTANCE", "SMSG_AREA_TRIGGER_NO_CORPSE" },
        };

        static void Main(string[] args)
        {
            bool ignoreTheirBuild = false;
            bool ignoreOurBuild = false;
            ushort build = 0;
            string their;
            string our;
            StreamWriter writer = null;
            bool insertHex = false;
            bool fixNameConflicts = false;

            try
            {
                our = args[0];
                their = args[1];

                for (int i = 2; i < args.Length; ++i)
                {
                    switch (args[i])
                    {
                        case "-d":
                            ignoreTheirBuild = true;
                            break;
                        case "-D":
                            ignoreOurBuild = true;
                            break;
                        case "-b":
                            build = ushort.Parse(args[++i]);
                            break;
                        case "-o":
                            Console.SetOut(writer = new StreamWriter(args[++i]));
                            break;
                        case "-h":
                            insertHex = true;
                            break;
                        case "-n":
                            fixNameConflicts = true;
                            break;
                    }
                }

                if ((ignoreTheirBuild || ignoreOurBuild) && build == 0)
                    throw new Exception();
            }
            catch
            {
                usage();
                return;
            }

            var ourOpcodes = new List<Tuple<string, uint>>();
            var theirOpcodes = new List<Tuple<string, uint>>();
            const uint firstUnk = (uint)ushort.MaxValue + 1;
            uint unkOpc = firstUnk;

            Action<string, bool, List<Tuple<string, uint>>> LoadFile = (filename, ignoreBuild, dict) =>
            {
                var regex = new Regex(@"([\w\d]+)\s*= ?([\da-fA-Fxhu]+|UNKNOWN_OPCODE),");
                var lines = File.ReadAllLines(filename);
                int i = 0;
                foreach (var line in lines)
                {
                    if (ignoreBuild && line.IndexOf(build.ToString()) == -1)
                        continue;

                    var match = regex.Match(line);
                    if (match.Success)
                    {
                        ulong val;
                        bool unk = false;
                        if (match.Groups[2].Value == "UNKNOWN_OPCODE")
                        {
                            unk = true;
                            val = ++unkOpc;
                        }
                        else
                            val = match.Groups[2].Value.ToNumeric<ulong>();
                        string opc = match.Groups[1].Value;
                        string opc2;
                        if (s_opcodeAnalogues.TryGetValue(opc, out opc2))
                            opc = opc2;
                        if (val < 0xFFFF || unk)
                        {
                            if (GetPair(dict, (uint)val) != null)
                                Console.WriteLine("Error: Duplicate opcode {0} in file '{1}'", val, filename);
                            else if (GetPair(dict, opc) == null)
                                dict.Add(Tuple.Create(opc, (uint)val));
                            else
                                Console.WriteLine("Error: Multiple defined opcode in file '{0}': " + match.Groups[1].Value, filename);
                        }
                        else
                            Console.WriteLine("Error: Opcode value failure: {0}", opc);
                    }
                    else
                        ++i;
                }
                Console.WriteLine("Skipped {0} lines while loading {1}", i, filename);
            };

            LoadFile(our, ignoreOurBuild, ourOpcodes);
            LoadFile(their, ignoreTheirBuild, theirOpcodes);

            var ourContent = File.ReadAllText(our);
            bool modified = false;

            var skipOpcodes = new List<uint>();

            for (uint i = 0; i < ushort.MaxValue; ++i)
            {
                if (skipOpcodes.Contains(i))
                    continue;

                var ourPair = GetPair(ourOpcodes, i);
                var theirPair = GetPair(theirOpcodes, i);

                if (ourPair == null && theirPair == null)
                    continue;

                if (ourPair == null)
                {
                    ourPair = GetPair(ourOpcodes, theirPair.Item1);
                    if (ourPair != null && ourPair.Item2 < firstUnk)
                        Console.WriteLine("Opcode {0} value conflict: our {1}, their {2}", theirPair.Item1, ourPair.Item2, theirPair.Item2);
                    else
                    {
                        Console.Write("Their unique opcode: {0} = {1}", theirPair.Item1, theirPair.Item2);

                        string newCont = Regex.Replace(ourContent, "(" + theirPair.Item1 + @"\s*= ?)([\d\w]+),",
                            "${1}" + (insertHex ? "0x" + theirPair.Item2.ToString("X4") : theirPair.Item2.ToString() + ","));
                        if (newCont != ourContent)
                        {
                            modified = true;
                            ourContent = newCont;
                            Console.WriteLine(" (added)");
                        }
                        else
                        {
                            var color = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" (NOT ADDED)");
                            Console.ForegroundColor = color;
                        }
                    }
                }
                //else
                //    ourOpcodes.Remove(ourPair.Item1);

                if (theirPair == null)
                {
                    theirPair = GetPair(theirOpcodes, ourPair.Item1);
                    if (theirPair != null && theirPair.Item2 < firstUnk)
                        Console.WriteLine("Opcode {0} value conflict: our {1}, their {2}", theirPair.Item1, ourPair.Item2, theirPair.Item2);
                    /*else
                        Console.WriteLine("Our unique opcode: {0} = {1},", ourPair.Item1, ourPair.Item2);*/
                }
                //else
                //    theirOpcodes.Remove(theirPair.Item1);

                if (theirPair != null && ourPair != null)
                {
                    // i.g.:
                    // their: SMSG_NEW_WORLD, 1
                    // our: SMSG_LOGIN_WORLD, 1
                    // actual: SMSG_NEW_WORLD = 1, SMSG_LOGIN_WORLD = 2,
                    if (theirPair.Item1 != ourPair.Item1)
                    {
                        /*var t = GetPair(theirOpcodes, ourPair.Item1);
                        var o = GetPair(ourOpcodes, theirPair.Item1);
                        if (t != null && o != null && t.Item2 != o.Item2 && t.Item2 == ourPair.Item2)
                        {
                            Console.WriteLine("Opcodes swapped: {0} and {1}", theirPair.Item1, ourPair.Item1);
                        }
                        else*/
                        {
                            Console.Write("Opcode {0} name conflict: our {1}, their {2}", theirPair.Item2, ourPair.Item1, theirPair.Item1);

                            if (fixNameConflicts)
                            {
                                if (GetPair(ourOpcodes, theirPair.Item1) == null)
                                {
                                    var tmp = Regex.Replace(ourContent, " " + ourPair.Item1 + @"(\s*)=\s*" + ourPair.Item2 + ",",
                                        " " + theirPair.Item1 + @"$1/*Conflict Fixed*/= " + ourPair.Item2 + ",");
                                    if (tmp != ourContent)
                                    {
                                        ourContent = tmp;
                                        modified = true;
                                        Console.WriteLine(" (Conflict Fixed)");
                                    }
                                    else
                                        Console.WriteLine(" (CONFLICT NOT FIXED)");
                                }
                                else
                                    Console.WriteLine(" (CONFLICT NOT FIXED, OPCODE EXISTS)");
                            }
                            else
                                Console.WriteLine();
                        }
                    }
                }

                //Console.WriteLine("");
            }

            if (modified)
                File.WriteAllText(our + ".new", ourContent.Replace("/*Conflict Fixed*/", ""));

            if (writer != null)
                writer.Close();
        }
    }
}
