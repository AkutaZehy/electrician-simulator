using Godot;
using System;
using System.Collections.Generic;

public static class LevelRegistry
{
    public struct LevelConfig
    {
        public string DeviceName;
        public string[] Dialogues;
        public string[] Hints;
        public Vector2I GridSize;
        public int[] Input;
        public int[] Target;
        public int Resistance;
        public bool ProbeEnabled;
        public Dictionary<Vector2I, (string True, string Visual)> NodeOverrides;
        public List<Tuple<Vector2I, Vector2I>> BrokenLinks;
    }

    public static List<LevelConfig> GetLevels()
    {
        var levels = new List<LevelConfig>();

        // Level 1
        levels.Add(new LevelConfig
        {
            DeviceName = "CLOCK",
            Dialogues = new[]
            {
                "Time to mix drinks and change lives.",
                "Oh, no, time to fix circuits and change lives.",
                "Rules are simple:",
                "Connect the route and pair the [shake rate=20 level=10]SIGNAL[/shake].",
                "[color=gray][Customer][/color] Is it really that difficult to fix this piece of junk?",
                "Keep calm, I am a professional electrician.",
                "[color=gray][SYSTEM][/color] Don't forget to press the [shake rate=20 level=10]PWR[/shake] when the route is connected.",
                ""
            },
            Hints = new[] { "Left-click the Green Node and Hold.\nDrag to connect nodes.", "Red Node is the output." },
            GridSize = new Vector2I(4, 3),
            Input = new[] { 3, 3, 3 },
            Target = new[] { 3, 3, 3 },
            Resistance = 99,
            ProbeEnabled = false,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(3, 2), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>
            {
                new Tuple<Vector2I, Vector2I>(new Vector2I(0, 0), new Vector2I(0, 1)),
                new Tuple<Vector2I, Vector2I>(new Vector2I(1, 1), new Vector2I(2, 1)),
                new Tuple<Vector2I, Vector2I>(new Vector2I(2, 1), new Vector2I(2, 2)),
                new Tuple<Vector2I, Vector2I>(new Vector2I(3, 1), new Vector2I(3, 2)),
            }
        });

        // Level 2
        levels.Add(new LevelConfig
        {
            DeviceName = "WEIGHT SCALE",
            Dialogues = new[]
            {
                "A weight scale.",
                "Not quite difficult task.",
                ""
            },
            Hints = new[] { "There's a yellow node there. Guess what it does?" },
            GridSize = new Vector2I(3, 2),
            Input = new[] { 1, 2, 3, 2, 1 },
            Target = new[] { 2, 4, 5, 4, 2 },
            Resistance = 99,
            ProbeEnabled = false,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(2, 0), ("MUL", "MUL") },
                { new Vector2I(2, 1), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>()
        });

        // Level 3
        levels.Add(new LevelConfig
        {
            DeviceName = "LAMP",
            Dialogues = new[]
            {
                "A table lamp using incandescent bulbs,",
                "A slightly outdated tool for lighting.",
                ""
            },
            Hints = new[] { "Test the different nodes.", "The secret is the [shake rate=20 level=10]order[/shake]." },
            GridSize = new Vector2I(3, 3),
            Input = new[] { 1, 2, 1, 2 },
            Target = new[] { 4, 5, 4, 5 },
            Resistance = 99,
            ProbeEnabled = false,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(2, 0), ("ADD", "ADD") },
                { new Vector2I(1, 2), ("MUL", "MUL") },
                { new Vector2I(2, 2), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>()
        });

        // Level 4
        levels.Add(new LevelConfig
        {
            DeviceName = "ELECTRIC KETTLE",
            Dialogues = new[]
            {
                "An electric kettle.",
                "Fortunately, it's just the fuse blew out due to working without water.",
                ""
            },
            Hints = new[]
            {
                "Test the new nodes individually, it is not hard to find the order.",
                "If you're unsure about the function of a certain node, you can jump back to the level and continue testing."
            },
            GridSize = new Vector2I(4, 3),
            Input = new[] { 1, 2, 1 },
            Target = new[] { 5, 1, 3 },
            Resistance = 99,
            ProbeEnabled = false,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(1, 0), ("ADD", "ADD") },
                { new Vector2I(1, 1), ("OSC", "OSC") },
                { new Vector2I(3, 1), ("DLY", "DLY") },
                { new Vector2I(3, 0), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>()
        });

        // Level 5
        levels.Add(new LevelConfig
        {
            DeviceName = "CHARGER[R=5]",
            Dialogues = new[]
            {
                "[color=gray][Customer][/color] Sir, anything wrong with the charger?",
                "It is a normal charger, but [shake rate=20 level=10]some components appear to be damaged[/shake].",
                "The current isn't very stable, but it might still work.",
                "There are also some spaces reserved for spare components.",
                "[color=gray][SYSTEM][/color] Okay, I can't keep making this up, but from here on, the current will [shake rate=20 level=10]gradually decrease[/shake] as the number of connected nodes increases.",
                ""
            },
            Hints = new[]
            {
                "There is a node broken here, making random noises.\nBypass it.",
                "Make use of spare nodes so that the output won't be overcharged."
            },
            GridSize = new Vector2I(5, 3),
            Input = new[] { 4, 5, 4 },
            Target = new[] { 2, 2, 3 },
            Resistance = 5,
            ProbeEnabled = false,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(1, 0), ("DLY", "DLY") },
                { new Vector2I(3, 1), ("ERR", "NUL") },
                { new Vector2I(4, 2), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>()
        });

        // Level 6
        levels.Add(new LevelConfig
        {
            DeviceName = "REMOTE CONTROL[R=6]",
            Dialogues = new[]
            {
                "A remote control from the last century.",
                "It was full of dust inside, and some of the components were [shake rate=20 level=10]no longer able to see the tags[/shake].",
                "[color=gray][Customer][/color] Sir, is this thing even repairable? Replacing it with a TV is a bit too expensive...",
                "It should be possible, but it will take some time.",
                ""
            },
            Hints = new[] { "Find the broken node first.", "Try the hidden node after bypassing the broken node." },
            GridSize = new Vector2I(4, 4),
            Input = new[] { 4, 1, 3, 2 },
            Target = new[] { 5, 0, 0, 0 },
            Resistance = 6,
            ProbeEnabled = false,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(1, 1), ("OSC", "UNK") },
                { new Vector2I(1, 2), ("ADD", "ADD") },
                { new Vector2I(3, 1), ("INV", "INV") },
                { new Vector2I(3, 2), ("ERR", "NUL") },
                { new Vector2I(3, 0), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>()
        });

        // Level 7
        levels.Add(new LevelConfig
        {
            DeviceName = "CAM_DELTA_027[R=5]",
            Dialogues = new[]
            {
                "[color=gray][Boss][/color] I know you've been fixing some \"antique\" lately, so I got you a probe; you should find it useful.",
                "[color=gray][Boss][/color] You know how to use the probe right huh?",
                "[color=gray][System][/color] Right click on a single node to place a probe, this would be useful for detecting broken and hidden nodes.",
                "Yes sir.",
                "[color=gray][Boss][/color] Then fix this camera for me as soon as possible.",
                ""
            },
            Hints = new[]
                { "Change the probe place-in while keeping the circuit connected, it may yield different results." },
            GridSize = new Vector2I(5, 5),
            Input = new[] { 1, 4, 5, 1, 2 },
            Target = new[] { 0, 1, 3, 1, 0 },
            Resistance = 5,
            ProbeEnabled = true,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(0, 1), ("ERR", "NUL") },
                { new Vector2I(3, 1), ("ERR", "NUL") },
                { new Vector2I(0, 3), ("ADD", "ADD") },
                { new Vector2I(2, 4), ("OSC", "OSC") },
                { new Vector2I(4, 4), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>
            {
                new Tuple<Vector2I, Vector2I>(new Vector2I(1, 0), new Vector2I(1, 1)),
                new Tuple<Vector2I, Vector2I>(new Vector2I(4, 1), new Vector2I(4, 2)),
            }
        });

        // Level 8
        levels.Add(new LevelConfig
        {
            DeviceName = "TEST_PLATE[R=7]",
            Dialogues = new[]
            {
                "Boss told me that we'll have a big order in a few days; the circuitry involved is very complex.",
                "Fortunately, I found this test plate, deep in my office.",
                "[color=gray][System][/color] The hidden node here is just a normal node.",
                ""
            },
            Hints = new[]
            {
                "The level includes all the mechanisms designed in this game. You can test them repeatedly here. ",
                "If you get stuck on the last level or are unsure about the function of some nodes, go back here, you can connect them in different orders here to verify your guesses."
            },
            GridSize = new Vector2I(5, 7),
            Input = new[] { 1, 4, 5, 1, 3 },
            Target = new[] { 1, 4, 5, 1, 3 },
            Resistance = 7,
            ProbeEnabled = true,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(0, 1), ("NUL", "UNK") },
                { new Vector2I(2, 1), ("ADD", "ADD") },
                { new Vector2I(2, 2), ("MUL", "MUL") },
                { new Vector2I(2, 3), ("INV", "INV") },
                { new Vector2I(2, 4), ("DLY", "DLY") },
                { new Vector2I(2, 5), ("OSC", "OSC") },
                { new Vector2I(2, 6), ("ERR", "NUL") },
                { new Vector2I(4, 0), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>()
        });

        // Level 9
        levels.Add(new LevelConfig
        {
            DeviceName = "Signal.[R=?]",
            Dialogues = new[]
            {
                "[shake rate=20 level=10]????[/shake]",
                "[shake rate=20 level=20]?????????[/shake]",
                "Soon you realized, it is not something within the realm of comprehension.",
                "BUT NOT [shake rate=20 level=10]PRACTICE[/shake].",
                ""
            },
            Hints = new[]
            {
                "The level includes all the mechanisms designed in this game. You can test them repeatedly here. ",
                "If you get stuck on the last level or are unsure about the function of some nodes, go back here, you can connect them in different orders here to verify your guesses."
            },
            GridSize = new Vector2I(7, 7),
            Input = new[] { 1, 4, 5, 1, 3, 2, 5, 1, 2 },
            Target = new[] { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
            Resistance = 3,
            ProbeEnabled = true,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(0, 1), ("NUL", "UNK") },
                { new Vector2I(1, 1), ("NUL", "UNK") },
                { new Vector2I(2, 1), ("NUL", "UNK") },
                { new Vector2I(0, 2), ("NUL", "UNK") },
                { new Vector2I(4, 1), ("NUL", "UNK") },
                { new Vector2I(5, 1), ("NUL", "UNK") },
                { new Vector2I(6, 1), ("NUL", "UNK") },
                { new Vector2I(4, 2), ("NUL", "UNK") },
                { new Vector2I(6, 2), ("NUL", "UNK") },
                { new Vector2I(0, 3), ("NUL", "UNK") },
                { new Vector2I(1, 3), ("NUL", "UNK") },
                { new Vector2I(2, 3), ("NUL", "UNK") },
                { new Vector2I(4, 3), ("NUL", "UNK") },
                { new Vector2I(5, 3), ("NUL", "UNK") },
                { new Vector2I(6, 3), ("NUL", "UNK") },
                { new Vector2I(2, 4), ("NUL", "UNK") },
                { new Vector2I(6, 4), ("NUL", "UNK") },
                { new Vector2I(0, 5), ("NUL", "UNK") },
                { new Vector2I(1, 5), ("NUL", "UNK") },
                { new Vector2I(2, 5), ("NUL", "UNK") },
                { new Vector2I(4, 5), ("NUL", "UNK") },
                { new Vector2I(5, 5), ("NUL", "UNK") },
                { new Vector2I(6, 5), ("INV", "UNK") },
                { new Vector2I(6, 6), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>()
        });

        // The end
        levels.Add(new LevelConfig
        {
            DeviceName = "A TRUE ELECTRICIAN.",
            Dialogues = new[]
            {
                "And,",
                "That is the story of a Electrician.",
                "A short story about SIGNAL.",
                "Now you can close the game safely and explore the next one!",
                "Made with Godot 4.6.",
                "Game Design, Music, Graphic, Storyboard: \nAkuta Zehy(Compo).",
                "Font: \nShareTechMono.",
                "Ludum Dare 59\nThank you for being here."
            },
            Hints = new[]
            {
                "I knew it.",
                "To avoid you feeling resentful, having wasted too much time, only to be [shake rate=20 level=10]tricked[/shake] at the end.",
                "Here is the full explanation of every node. If you want to check it.",
                "Green and Red nodes are input and output.",
                "Grey nodes are normal ones, after x steps (R=x), the value drop by 1, until 0.",
                "Levels without explanation: R=99",
                "Yellow nodes are MUL nodes, multiply the value by 2.",
                "Orange nodes are ADD, add 1 to the value.",
                "Max=5, overflowed values will be eliminated.",
                "Purple nodes are DLY, shift the value by 1 step",
                "Blue nodes are INV, change non-zero values to 0, and zero to max.",
                "The most complex, Cyan nodes are OSC.",
                "It deals an AND operation, original value AND the sequence 1-3-5-3...",
                "Some nodes are broken, looks the same as normal ones, only outputing random values.",
                "Some nodes are hidden, they are black, and they could be any one of the nodes above.",
                "That's all.",
                ""
            },
            GridSize = new Vector2I(3, 1),
            Input = new[] { 1 },
            Target = new[] { 1 },
            Resistance = 99,
            ProbeEnabled = true,
            NodeOverrides = new Dictionary<Vector2I, (string, string)>
            {
                { new Vector2I(0, 0), ("INP", "INP") },
                { new Vector2I(2, 0), ("OUT", "OUT") }
            },
            BrokenLinks = new List<Tuple<Vector2I, Vector2I>>()
        });

        return levels;
    }
}