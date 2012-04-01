using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("spells")]
    struct Spell
    {
        [PrimaryKey]
        public uint Id;                                           // 0        m_ID
        public uint Category;                                     // 1        m_category
        public uint Dispel;                                       // 2        m_dispelType
        public uint Mechanic;                                     // 3        m_mechanic
        public uint Attributes;                                   // 4        m_attribute
        public uint AttributesEx;                                 // 5        m_attributesEx
        public uint AttributesEx2;                                // 6        m_attributesExB
        public uint AttributesEx3;                                // 7        m_attributesExC
        public uint AttributesEx4;                                // 8        m_attributesExD
        public uint AttributesEx5;                                // 9        m_attributesExE
        public uint AttributesEx6;                                // 10       m_attributesExF
        public uint AttributesExG;                                // 11       3.2.0 (0x20 - totems, 0x4 - paladin auras, etc...)
        public ulong Stances;                                     // 12-13    m_shapeshiftMask
        public ulong StancesNot;                                  // 14-15    m_shapeshiftExclude
        public uint Targets;                                      // 16       m_targets
        public uint TargetCreatureType;                           // 17       m_targetCreatureType
        public uint RequiresSpellFocus;                           // 18       m_requiresSpellFocus
        public uint FacingCasterFlags;                            // 19       m_facingCasterFlags
        public uint CasterAuraState;                              // 20       m_casterAuraState
        public uint TargetAuraState;                              // 21       m_targetAuraState
        public uint CasterAuraStateNot;                           // 22       m_excludeCasterAuraState
        public uint TargetAuraStateNot;                           // 23       m_excludeTargetAuraState
        public uint CasterAuraSpell;                              // 24       m_casterAuraSpell
        public uint TargetAuraSpell;                              // 25       m_targetAuraSpell
        public uint ExcludeCasterAuraSpell;                       // 26       m_excludeCasterAuraSpell
        public uint ExcludeTargetAuraSpell;                       // 27       m_excludeTargetAuraSpell
        public uint CastingTimeIndex;                             // 28       m_castingTimeIndex
        public uint RecoveryTime;                                 // 29       m_recoveryTime
        public uint CategoryRecoveryTime;                         // 30       m_categoryRecoveryTime
        public uint InterruptFlags;                               // 31       m_interruptFlags
        public uint AuraInterruptFlags;                           // 32       m_auraInterruptFlags
        public uint ChannelInterruptFlags;                        // 33       m_channelInterruptFlags
        public uint ProcFlags;                                    // 34       m_procTypeMask
        public uint ProcChance;                                   // 35       m_procChance
        public uint ProcCharges;                                  // 36       m_procCharges
        public uint MaxLevel;                                     // 37       m_maxLevel
        public uint BaseLevel;                                    // 38       m_baseLevel
        public uint SpellLevel;                                   // 39       m_spellLevel
        public uint DurationIndex;                                // 40       m_durationIndex
        public uint PowerType;                                    // 41       m_powerType
        public uint ManaCost;                                     // 42       m_manaCost
        public uint ManaCostPerlevel;                             // 43       m_manaCostPerLevel
        public uint ManaPerSecond;                                // 44       m_manaPerSecond
        public uint ManaPerSecondPerLevel;                        // 45       m_manaPerSecondPerLevel
        public uint RangeIndex;                                   // 46       m_rangeIndex
        private float Speed;                                      // 47       m_speed
        private uint ModalNextSpell;                              // 48       m_modalNextSpell not used
        public uint StackAmount;                                  // 49       m_cumulativeAura
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        [Array(2)]
        public uint[] Totem;                                      // 50-51    m_totem
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        [Array(8)]
        public int[] Reagent;                                     // 52-59    m_reagent
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        [Array(8)]
        public uint[] ReagentCount;                               // 60-67    m_reagentCount
        public int EquippedItemClass;                             // 68       m_equippedItemClass (value)
        public int EquippedItemSubClassMask;                      // 69       m_equippedItemSubclass (mask)
        public int EquippedItemInventoryTypeMask;                 // 70       m_equippedItemInvTypes (mask)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] Effect;                     				  // 71-73    m_effect
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public int[] EffectDieSides;                              // 74-76    m_effectDieSides
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public float[] EffectRealPointsPerLevel;                  // 77-79    m_effectRealPointsPerLevel
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public int[] EffectBasePoints;                            // 80-82    m_effectBasePoints (don't must be used in spell/auras explicitly, must be used cached Spell::m_currentBasePoints)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectMechanic;                             // 83-85    m_effectMechanic
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectImplicitTargetA;                      // 86-88    m_implicitTargetA
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectImplicitTargetB;                      // 89-91    m_implicitTargetB
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectRadiusIndex;                          // 92-94    m_effectRadiusIndex - spellradius.dbc
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectApplyAuraName;                        // 95-97    m_effectAura
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectAmplitude;                            // 98-100   m_effectAuraPeriod
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public float[] EffectMultipleValue;                       // 101-103  m_effectAmplitude
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectChainTarget;                          // 104-106  m_effectChainTargets
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectItemType;                             // 107-109  m_effectItemType
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public int[] EffectMiscValue;                             // 110-112  m_effectMiscValue
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public int[] EffectMiscValueB;                            // 113-115  m_effectMiscValueB
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectTriggerSpell;                         // 116-118  m_effectTriggerSpell
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public float[] EffectPointsPerComboPoint;                 // 119-121  m_effectPointsPerCombo
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectSpellClassMaskA;                      // 122-124  m_effectSpellClassMaskA, effect 0
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectSpellClassMaskB;                      // 125-127  m_effectSpellClassMaskB, effect 1
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public uint[] EffectSpellClassMaskC;                      // 128-130  m_effectSpellClassMaskC, effect 2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        [Array(2)]
        public uint[] SpellVisual;                                // 131-132  m_spellVisualID
        public uint SpellIconID;                                  // 133      m_spellIconID
        public uint ActiveIconID;                                 // 134      m_activeIconID
        public uint SpellPriority;                                // 135      m_spellPriority not used
        [DBCString(true)]
        public uint SpellName;                                // 136-151  m_name_lang
        [DBCString(true)]
        public uint Rank;                                     // 153-168  m_nameSubtext_lang
        [DBCString(true)]
        public uint Description;                              // 170-185  m_description_lang not used
        [DBCString(true)]
        public uint ToolTip;                                  // 187-202  m_auraDescription_lang not used
        public uint ManaCostPercentage;                           // 204      m_manaCostPct
        public uint StartRecoveryCategory;                        // 205      m_startRecoveryCategory
        public uint StartRecoveryTime;                            // 206      m_startRecoveryTime
        public uint MaxTargetLevel;                               // 207      m_maxTargetLevel
        public uint SpellFamilyName;                              // 208      m_spellClassSet
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        [Array(3)]
        public uint[] SpellFamilyFlags;                           // 209-211  m_spellClassMask NOTE: size is 12 bytes!!!
        public uint MaxAffectedTargets;                           // 212      m_maxTargets
        public uint DmgClass;                                     // 213      m_defenseType
        public uint PreventionType;                               // 214      m_preventionType
        private uint StanceBarOrder;                               // 215      m_stanceBarOrder not used
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        public float[] DmgMultiplier;              				  // 216-218  m_effectChainAmplitude
        public uint MinFactionId;                                 // 219      m_minFactionID not used
        public uint MinReputation;                                // 220      m_minReputation not used
        public uint RequiredAuraVision;                           // 221      m_requiredAuraVision not used
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        [Array(2)]
        public uint[] TotemCategory;                              // 222-223  m_requiredTotemCategoryID
        public int AreaGroupId;                                   // 224      m_requiredAreaGroupId
        public uint SchoolMask;                                   // 225      m_schoolMask
        public uint RuneCostID;                                   // 226      m_runeCostID
        private uint SpellMissileID;                              // 227      m_spellMissileID not used
        private uint PowerDisplayId;                              // 228      PowerDisplay.dbc, new in 3.1
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.SpellEffects)]
        [Array(Constants.SpellEffects)]
        private float[] Unk_320_4;                                // 229-231  3.2.0
        private uint SpellDescriptionVariableID;                  // 232      3.2.0
        public uint SpellDifficultyId;                            // 233      3.3.0                           // 239      3.3.0
    }
}
